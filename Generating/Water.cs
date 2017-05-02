using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Generating.Textures;
using Generating.Interfaces;
using Generating.Shaders;
using Generating.SceneObjects;

namespace Generating
{
    class Water : IRenderable, IResizable, IDisposable
    {
        private int defaultWindowWidth;
        private int defaultWindowHeight;

        private int tileWidth;
        private int tileHeight;

        private const int REFLECTION_WIDTH = 1280;
        private const int REFLECTION_HEIGHT = 720;

        private const int REFRACTION_WIDTH = 1280;
        private const int REFRACTION_HEIGHT = 720;

        private float time = 0.0f;
        private float waterSpeed = 0.0003f;
        private float waveStrength = 0.02f;

        private ShaderProgram shader;

        private VAO vao;

        private int reflectionFrameBuffer;
        private int reflectionTexture;
        private int reflectionDepthBuffer;
        private int reflectionSampler;


        private int refractionFrameBuffer;
        private int refractionTexture;
        private int refractionDepthTexture;
        private int refractionSampler;

        private Texture2D dudvMapTexture;
        private Texture2D normalMapTexture;
        
        private Matrix4 modelMatrix;
        private Light light;
        

        public Water(Light light, int defaultWindowWidth, int defaultWindowHeight, int tileWidth, int tileHeight, float zoom, float waterHeight)
        {
            dudvMapTexture = new Texture2D("dudvMap.jpg");
            normalMapTexture = new Texture2D("normalMap.jpg");

            this.defaultWindowWidth = defaultWindowWidth;
            this.defaultWindowHeight = defaultWindowHeight;
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            this.light = light;
            this.modelMatrix = Matrix4.Identity;

            InitShader();
            InitVAO(zoom, waterHeight);
            InitReflectionFrameBuffer();
            InitRefractionFrameBuffer();
        }

        private void InitShader()
        {
            shader = new ShaderProgram();
            Shader vertexShader = new Shader("water.vert", ShaderType.VertexShader);
            Shader fragmentShader = new Shader("water.frag", ShaderType.FragmentShader);
            shader.AttachShaders(vertexShader, fragmentShader);
            shader.LinkProgram();

            shader.Uniforms["projectionMatrix"] = GL.GetUniformLocation(shader.ID, "projectionMatrix");
            shader.Uniforms["viewMatrix"] = GL.GetUniformLocation(shader.ID, "viewMatrix");
            shader.Uniforms["modelMatrix"] = GL.GetUniformLocation(shader.ID, "modelMatrix");
            shader.Uniforms["reflectionTexture"] = GL.GetUniformLocation(shader.ID, "reflectionTexture");
            shader.Uniforms["refractionTexture"] = GL.GetUniformLocation(shader.ID, "refractionTexture");
            shader.Uniforms["dudvMapTexture"] = GL.GetUniformLocation(shader.ID, "dudvMapTexture");
            shader.Uniforms["waveStrength"] = GL.GetUniformLocation(shader.ID, "waveStrength");
            shader.Uniforms["tiling"] = GL.GetUniformLocation(shader.ID, "tiling");
            shader.Uniforms["time"] = GL.GetUniformLocation(shader.ID, "time");
            shader.Uniforms["cameraPosition"] = GL.GetUniformLocation(shader.ID, "cameraPosition");
            shader.Uniforms["normalMap"] = GL.GetUniformLocation(shader.ID, "normalMap");
            shader.Uniforms["lightDirection"] = GL.GetUniformLocation(shader.ID, "lightDirection");
            shader.Uniforms["lightColor"] = GL.GetUniformLocation(shader.ID, "lightColor");
            shader.Uniforms["specularIntensity"] = GL.GetUniformLocation(shader.ID, "specularIntensity");
            shader.Uniforms["specularPower"] = GL.GetUniformLocation(shader.ID, "specularPower");
            shader.Uniforms["depthMap"] = GL.GetUniformLocation(shader.ID, "depthMap");

            shader.AttribLocation["inPosition"] = GL.GetAttribLocation(shader.ID, "inPosition");
            shader.AttribLocation["inTexCoords"] = GL.GetAttribLocation(shader.ID, "inTexCoords");
        }

        private void InitVAO(float zoom, float waterHeight)
        {
            vao = new VAO();

            Vector3[] vertices = new Vector3[tileWidth * tileHeight];
            Vector2[] texCoords = new Vector2[tileWidth * tileHeight];
            uint[] indices = new uint[2 * tileWidth * tileHeight];

            float tiling = 3.0f;
            for (int i = 0, ver = 0, ind = 0; i < tileWidth; i++)
            {
                float z = i * zoom;
                for (int j = 0; j < tileHeight; j++)
                {
                    float x = j * zoom;
                    vertices[ver] = new Vector3(x, waterHeight, z);
                    indices[ind++] = (uint)(i * tileWidth + j);
                    if (i != tileWidth - 1)
                        indices[ind++] = (uint)((i + 1) * tileWidth + j);
                    texCoords[ver++] = new Vector2(x * tiling / (tileHeight - 1), z * tiling / (tileWidth - 1));
                }

                indices[ind++] = (uint)indices.Length;
            }

            vao.BindVerticesBuffer(vertices);
            vao.BindTexCoordsBuffer(texCoords);
            vao.BindIndicesBuffer(indices);
        }

        private void InitReflectionFrameBuffer()
        {
            reflectionFrameBuffer = CreateFrameBuffer();
            reflectionTexture = CreateTextureAttachment(REFLECTION_WIDTH, REFLECTION_HEIGHT);
            reflectionDepthBuffer = CreateDepthBufferAttachment(REFLECTION_WIDTH, REFLECTION_HEIGHT);
            reflectionSampler = GL.GenSampler();
            UnbindFrameBuffer();
        }

        private void InitRefractionFrameBuffer()
        {
            refractionFrameBuffer = CreateFrameBuffer();
            refractionTexture = CreateTextureAttachment(REFRACTION_WIDTH, REFRACTION_HEIGHT);
            refractionDepthTexture = CreateDepthTextureAttachment(REFRACTION_WIDTH, REFRACTION_HEIGHT);
            refractionSampler = GL.GenSampler();
            UnbindFrameBuffer();
        }
        private int CreateFrameBuffer()
        {
            int frameBuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
            return frameBuffer;
        }

        private int CreateTextureAttachment(int width, int height)
        {
            int texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Bgra, PixelType.UnsignedByte,
                (IntPtr)null);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, texture, 0);
            return texture;
        }

        private int CreateDepthTextureAttachment(int width, int height)
        {
            int texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32, width, height, 0, PixelFormat.DepthComponent,
                PixelType.Float, (IntPtr)null);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, texture, 0);
            return texture;
        }

        private int CreateDepthBufferAttachment(int width, int height)
        {
            int depthBuffer = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent32, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                RenderbufferTarget.Renderbuffer, depthBuffer);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            return depthBuffer;
        }

        public void BindReflectionFrameBuffer()
        {
            BindFrameBuffer(reflectionFrameBuffer, REFLECTION_WIDTH, REFLECTION_HEIGHT);
        }

        public void BindRefractionFrameBuffer()
        {
            BindFrameBuffer(refractionFrameBuffer, REFRACTION_WIDTH, REFRACTION_HEIGHT);
        }

        private void BindFrameBuffer(int frameBuffer, int width, int height)
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            GL.Viewport(0, 0, width, height);
        }

        public void UnbindFrameBuffer()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, defaultWindowWidth, defaultWindowHeight);
        }

        private void BindWaterRendererSettings()
        {
            shader.Start();
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.PrimitiveRestart);
            GL.PrimitiveRestartIndex(vao.IndicesCount);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, reflectionTexture);
            GL.BindSampler((int)TextureUnit.Texture0, reflectionSampler);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, refractionTexture);
            GL.BindSampler((int)TextureUnit.Texture1, refractionSampler);

            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, dudvMapTexture.ID);
            GL.BindSampler((int)TextureUnit.Texture2, dudvMapTexture.ID);

            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2D, normalMapTexture.ID);
            GL.BindSampler((int)TextureUnit.Texture3, normalMapTexture.ID);

            GL.ActiveTexture(TextureUnit.Texture4);
            GL.BindTexture(TextureTarget.Texture2D, refractionDepthTexture);
            GL.BindSampler((int)TextureUnit.Texture4, refractionDepthTexture);

            time += waterSpeed;
            time %= 1;
            GL.UniformMatrix4(shader.Uniforms["projectionMatrix"], false, ref Camera.Instance.Projection);
            GL.UniformMatrix4(shader.Uniforms["modelMatrix"], false, ref modelMatrix);
            GL.UniformMatrix4(shader.Uniforms["viewMatrix"], false, ref Camera.Instance.View);
            GL.Uniform1(shader.Uniforms["reflectionTexture"], 0);
            GL.Uniform1(shader.Uniforms["refractionTexture"], 1);
            GL.Uniform1(shader.Uniforms["dudvMapTexture"], 2);
            GL.Uniform1(shader.Uniforms["normalMap"], 3);
            GL.Uniform1(shader.Uniforms["depthMap"], 4);
            GL.Uniform1(shader.Uniforms["waveStrength"], waveStrength);
            GL.Uniform1(shader.Uniforms["time"], time);
            GL.Uniform3(shader.Uniforms["cameraPosition"], ref Camera.Instance.Eye);
            GL.Uniform3(shader.Uniforms["lightDirection"], ref light.Direction);
            GL.Uniform3(shader.Uniforms["lightColor"], ref light.Color);
            GL.Uniform1(shader.Uniforms["specularIntensity"], light.SpecularIntensity);
            GL.Uniform1(shader.Uniforms["specularPower"], light.SpecularPower);

            GL.BindVertexArray(vao.ID);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vao.VerticesBuffer);
            GL.EnableVertexAttribArray(shader.AttribLocation["inPosition"]);
            GL.VertexAttribPointer(shader.AttribLocation["inPosition"], 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vao.TexCoordsBuffer);
            GL.EnableVertexAttribArray(shader.AttribLocation["inTexCoords"]);
            GL.VertexAttribPointer(shader.AttribLocation["inTexCoords"], 2, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, vao.IndicesBuffer);
        }

        private void UnbindWaterRendererSettings()
        {
            shader.Stop();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindVertexArray(0);
            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.PrimitiveRestart);
        }
        public void Render()
        {
            BindWaterRendererSettings();
            
            GL.DrawElements(BeginMode.TriangleStrip, vao.IndicesCount, DrawElementsType.UnsignedInt, 0);

            UnbindWaterRendererSettings();
        }

        public void Resize(int width, int height)
        {
            defaultWindowWidth = width;
            defaultWindowHeight = height;
        }

        public void Dispose()
        {
            GL.DeleteFramebuffer(reflectionFrameBuffer);
            GL.DeleteTexture(reflectionTexture);
            GL.DeleteTexture(reflectionDepthBuffer);
            GL.DeleteFramebuffer(refractionFrameBuffer);
            GL.DeleteTexture(refractionTexture);
            GL.DeleteTexture(refractionDepthTexture);

            shader.Dispose();
        }
    }
}
