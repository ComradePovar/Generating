using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Generating.Interfaces;
using Generating.Shaders;
using Generating.Textures;

namespace Generating.SceneObjects
{
    class Terrain : IRenderable, IResizable
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        private Texture2D waterT;
        private Texture2D rock;
        private Texture2D mossyrock;
        private Texture2D dirt;
        private Texture2D sand;
        private Texture2D darkgrass;
        
        public Scene ParentScene { get; }

        private Light light;
        private Fog fog;

        private ShaderProgram shader;
        private VAO vao;
        private Water water;
        
        private float waterHeight;
        
        private Vector4 horizontalPlaneReflection;
        private Vector4 horizontalPlaneRefraction;
        private Vector4 horizontalPlaneWorld;

        private Matrix4 modelMatrix;

        public Terrain(SceneParameters.TerrainParameters args, Scene parentScene)
        {
            this.Width = args.Width;
            this.Height = args.Height;

            rock = new Texture2D("rock.jpg");
            mossyrock = new Texture2D("mossyrock.jpg");
            waterT = new Texture2D("water.jpg");//grass2.jpg || water.jpg
            darkgrass = new Texture2D("darkgrass.jpg");
            dirt = new Texture2D("dirt.jpg");
            sand = new Texture2D("sand.jpg");

            this.ParentScene = parentScene;
            
            InitVAO(args);
            InitShader();
            InitLight(args);
            InitFog(args);
            InitWater(args);


            horizontalPlaneReflection = new Vector4(0.0f, 1.0f, 0.0f, -waterHeight);
            horizontalPlaneRefraction = new Vector4(0.0f, -1.0f, 0.0f, waterHeight + waterHeight * 0.01f);
            horizontalPlaneWorld = new Vector4(0.0f, -1.0f, 0.0f, 100000);
            modelMatrix = Matrix4.Identity;
        }

        private void InitLight(SceneParameters.TerrainParameters args)
        {
            light = new Light(args.LightPosition, args.LightAngle)
            {
                Color = args.LightColor,
                AmbientIntensity = args.AmbientIntensity,
                SpecularIntensity = args.SpecularIntensity,
                SpecularPower = args.SpecularPower
            };
        }

        private void InitFog(SceneParameters.TerrainParameters args)
        {
            fog = new Fog()
            {
                Color = args.FogColor,
                Density = args.FogDensity,
                Start = args.FogStart,
                End = args.FogEnd,
                Type = args.FogType
            };
        }

        private void InitWater(SceneParameters.TerrainParameters args)
        {
            Light waterLight = new Light(args.LightPosition, args.LightAngle)
            {
                Color = args.WaterLightColor,
                SpecularIntensity = args.WaterSpecularIntensity,
                SpecularPower = args.WaterSpecularPower
            };
            water = new Water(waterLight, args, waterHeight);
        }

        private void InitVAO(SceneParameters.TerrainParameters args)
        {
            vao = new VAO();
            TerrainGenerator.Instance.GenerateIsland(vao, args.Width, args.Height, args.Roughness,
                                                     args.TerrainGenerationMin, args.TerrainGenerationMax, 
                                                     args.Scale, out waterHeight);
        }


        private void InitShader()
        {
            shader = new ShaderProgram();
            Shader vertexShader = new Shader("shader.vert", ShaderType.VertexShader);
            Shader fogShader = new Shader("fogShader.vert", ShaderType.VertexShader);
            Shader fragmentShader = new Shader("mainShader.frag", ShaderType.FragmentShader);
            shader.AttachShaders(fogShader, vertexShader, fragmentShader);
            shader.LinkProgram();

            shader.Uniforms["modelMatrix"] = GL.GetUniformLocation(shader.ID, "modelMatrix");
            shader.Uniforms["viewMatrix"] = GL.GetUniformLocation(shader.ID, "viewMatrix");
            shader.Uniforms["projectionMatrix"] = GL.GetUniformLocation(shader.ID, "projectionMatrix");
            shader.Uniforms["normalMatrix"] = GL.GetUniformLocation(shader.ID, "normalMatrix");
            shader.Uniforms["color"] = GL.GetUniformLocation(shader.ID, "color");
            shader.Uniforms["samplers[0]"] = GL.GetUniformLocation(shader.ID, "samplers[0]");
            shader.Uniforms["samplers[1]"] = GL.GetUniformLocation(shader.ID, "samplers[1]");
            shader.Uniforms["samplers[2]"] = GL.GetUniformLocation(shader.ID, "samplers[2]");
            shader.Uniforms["samplers[3]"] = GL.GetUniformLocation(shader.ID, "samplers[3]");
            shader.Uniforms["samplers[4]"] = GL.GetUniformLocation(shader.ID, "samplers[4]");
            shader.Uniforms["samplers[5]"] = GL.GetUniformLocation(shader.ID, "samplers[5]");
            shader.Uniforms["light.color"] = GL.GetUniformLocation(shader.ID, "light.color");
            shader.Uniforms["light.direction"] = GL.GetUniformLocation(shader.ID, "light.direction");
            shader.Uniforms["light.ambientIntensity"] = GL.GetUniformLocation(shader.ID, "light.ambientIntensity");
            shader.Uniforms["light.specularIntensity"] = GL.GetUniformLocation(shader.ID, "light.specularIntensity");
            shader.Uniforms["light.specularPower"] = GL.GetUniformLocation(shader.ID, "light.specularPower");
            shader.Uniforms["eyePos"] = GL.GetUniformLocation(shader.ID, "eyePos");
            shader.Uniforms["fog.color"] = GL.GetUniformLocation(shader.ID, "fog.color");
            shader.Uniforms["fog.start"] = GL.GetUniformLocation(shader.ID, "fog.start");
            shader.Uniforms["fog.end"] = GL.GetUniformLocation(shader.ID, "fog.end");
            shader.Uniforms["fog.density"] = GL.GetUniformLocation(shader.ID, "fog.density");
            shader.Uniforms["fog.type"] = GL.GetUniformLocation(shader.ID, "fog.type");
            shader.Uniforms["plane"] = GL.GetUniformLocation(shader.ID, "plane");

            shader.AttribLocation["inPosition"] = GL.GetAttribLocation(shader.ID, "inPosition");
            shader.AttribLocation["inCoord"] = GL.GetAttribLocation(shader.ID, "inCoord");
            shader.AttribLocation["inNormal"] = GL.GetAttribLocation(shader.ID, "inNormal");
            shader.AttribLocation["inNormalizedHeight"] = GL.GetAttribLocation(shader.ID, "inNormalizedHeight");
            shader.AttribLocation["inMoisture"] = GL.GetAttribLocation(shader.ID, "inMoisture");
        }

        public void BindTerrainRendererSettings()
        {
            shader.Start();

            GL.BindVertexArray(vao.ID);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vao.VerticesBuffer);
            GL.EnableVertexAttribArray(shader.AttribLocation["inPosition"]);
            GL.VertexAttribPointer(shader.AttribLocation["inPosition"], 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vao.TexCoordsBuffer);
            GL.EnableVertexAttribArray(shader.AttribLocation["inCoord"]);
            GL.VertexAttribPointer(shader.AttribLocation["inCoord"], 2, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, vao.IndicesBuffer);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vao.NormalsBuffer);
            GL.EnableVertexAttribArray(shader.AttribLocation["inNormal"]);
            GL.VertexAttribPointer(shader.AttribLocation["inNormal"], 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vao.NormalizedHeightsBuffer);
            GL.EnableVertexAttribArray(shader.AttribLocation["inNormalizedHeight"]);
            GL.VertexAttribPointer(shader.AttribLocation["inNormalizedHeight"], 1, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vao.MoisturesBuffer);
            GL.EnableVertexAttribArray(shader.AttribLocation["inMoisture"]);
            GL.VertexAttribPointer(shader.AttribLocation["inMoisture"], 1, VertexAttribPointerType.Float, false, 0, 0);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, rock.ID);
            GL.BindSampler((int)TextureUnit.Texture0, rock.SamplerID);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, waterT.ID);
            GL.BindSampler((int)TextureUnit.Texture1, waterT.SamplerID);

            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, mossyrock.ID);
            GL.BindSampler((int)TextureUnit.Texture2, mossyrock.SamplerID);

            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2D, dirt.ID);
            GL.BindSampler((int)TextureUnit.Texture3, dirt.SamplerID);

            GL.ActiveTexture(TextureUnit.Texture4);
            GL.BindTexture(TextureTarget.Texture2D, sand.ID);
            GL.BindSampler((int)TextureUnit.Texture4, sand.SamplerID);

            GL.ActiveTexture(TextureUnit.Texture5);
            GL.BindTexture(TextureTarget.Texture2D, darkgrass.ID);
            GL.BindSampler((int)TextureUnit.Texture5, darkgrass.SamplerID);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.PrimitiveRestart);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.ClipPlane0);
            GL.PrimitiveRestartIndex(vao.IndicesCount);

            GL.UniformMatrix4(shader.Uniforms["projectionMatrix"], false, ref Camera.Instance.Projection);
            GL.UniformMatrix4(shader.Uniforms["modelMatrix"], false, ref modelMatrix);
            GL.UniformMatrix4(shader.Uniforms["viewMatrix"], false, ref Camera.Instance.View);
            GL.UniformMatrix4(shader.Uniforms["normalMatrix"], false, ref Camera.Instance.Normal);
            GL.Uniform3(shader.Uniforms["eyePos"], ref Camera.Instance.Eye);
            GL.Uniform1(shader.Uniforms["samplers[0]"], (int)TextureUnit.Texture0);
            GL.Uniform1(shader.Uniforms["samplers[1]"], 1);
            GL.Uniform1(shader.Uniforms["samplers[2]"], 2);
            GL.Uniform1(shader.Uniforms["samplers[3]"], 3);
            GL.Uniform1(shader.Uniforms["samplers[4]"], 4);
            GL.Uniform1(shader.Uniforms["samplers[5]"], 5);
            GL.Uniform4(shader.Uniforms["color"], new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
            GL.Uniform3(shader.Uniforms["light.color"], ref light.Color);
            GL.Uniform3(shader.Uniforms["light.direction"], ref light.Direction);
            GL.Uniform1(shader.Uniforms["light.ambientIntensity"], light.AmbientIntensity);
            GL.Uniform1(shader.Uniforms["light.specularIntensity"], light.SpecularIntensity);
            GL.Uniform1(shader.Uniforms["light.specularPower"], light.SpecularPower);
            GL.Uniform4(shader.Uniforms["fog.color"], ref fog.Color);
            GL.Uniform1(shader.Uniforms["fog.density"], fog.Density);
            GL.Uniform1(shader.Uniforms["fog.start"], fog.Start);
            GL.Uniform1(shader.Uniforms["fog.end"], fog.End);
            GL.Uniform1(shader.Uniforms["fog.type"], (int)fog.Type);
            GL.Uniform4(shader.Uniforms["plane"], horizontalPlaneWorld);
        }

        private void UnbindTerrainRendererSettings()
        {
            shader.Stop();
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.PrimitiveRestart);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.ClipPlane0);
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Render()
        {
            RenderWaterReflection();
            RenderWaterRefraction();
            BindTerrainRendererSettings();
            GL.DrawElements(BeginMode.TriangleStrip, vao.IndicesCount, DrawElementsType.UnsignedInt, 0);
            
            UnbindTerrainRendererSettings();
            water.Render();
        }

        public void RenderWaterReflection()
        {
            water.BindReflectionFrameBuffer();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            float distance = 2 * (Camera.Instance.Eye.Y - waterHeight);
            Camera.Instance.Eye.Y -= distance;
            Camera.Instance.Pitch = -Camera.Instance.Pitch;
            Camera.Instance.UpdateView();

            foreach (IRenderable obj in ParentScene.renderableObjects)
                if (obj != this)
                    obj.Render();


            BindTerrainRendererSettings();
            GL.Uniform4(shader.Uniforms["plane"], horizontalPlaneReflection);
            GL.DrawElements(BeginMode.TriangleStrip, vao.IndicesCount, DrawElementsType.UnsignedInt, 0);
            UnbindTerrainRendererSettings();

            Camera.Instance.Eye.Y += distance;
            Camera.Instance.Pitch = -Camera.Instance.Pitch;
            Camera.Instance.UpdateView();

            water.UnbindFrameBuffer();
        }

        public void RenderWaterRefraction()
        {
            water.BindRefractionFrameBuffer();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (IRenderable obj in ParentScene.renderableObjects)
                if (obj != this)
                    obj.Render();

            BindTerrainRendererSettings();
            GL.Uniform4(shader.Uniforms["plane"], horizontalPlaneRefraction);
            GL.DrawElements(BeginMode.TriangleStrip, vao.IndicesCount, DrawElementsType.UnsignedInt, 0);
            UnbindTerrainRendererSettings();

            water.UnbindFrameBuffer();
        }

        public void Resize(int width, int height)
        {
            water.Resize(width, height);
        }
    }
}
