using System;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using Generating.Shaders;
using Generating.GUI;
using Generating.Textures;


/* TODO:
 * Redbook 8th edition 
 *- 1) Упростить структуру программы;
 *- 2) Загрузка существующей heightmap;
 *- 4) Разбить terrain на чанки;
 *- 10) UI для задания параметров карты;
 *- 9) Доработать алгоритм генерации;
 *- 6) Shadow map;
 *- 8) terrain patterns;
 *- 7) correct blending, texture splatting;
 * */
namespace Generating
{
    enum RenderMode { Mesh, Textured }
    class Game : GameWindow
    {
        private int W = 1025;
        private int H = 1025;
        TerrainGenerator terrainGenerator;
        Camera camera;
        
        float zoom = 1f;
        private float min = 0;
        private float max = 10;
        private float roughness = 18f;
        private float topLeft = 0;
        private float bottomLeft = 0;
        private float bottomRight = 0;
        private float topRight = 0;
        private RenderMode renderMode = RenderMode.Textured;
        private VAO terrain;
        private VAO waterPlane;
        private Skybox skybox;
        private ShaderProgram terrainShader;
        private ShaderProgram waterShader;
        private ShaderProgram guiShader;
        private Light light;
        public float specularIntensity = 1;
        public float specularPower = 2;
        public float wave = 0.02f;
        public float tiling = 1;
        public float waterSpeed = 0.0003f;
        public float time = 0;

        Texture2D water;
        Texture2D rock;
        Texture2D mossyrock;
        Texture2D dirt;
        Texture2D sand;
        Texture2D darkgrass;
        Vector4 FogColor;
        Vector4 HorizontalPlaneReflection;
        Vector4 HorizontalPlaneRefraction;
        Vector4 HorizontalPlaneWorld;
        Matrix4 model = Matrix4.Identity;
        WaterFBO fbo;
        Gui guiReflect;
        Gui guiRefract;
        
        public float Roughness { get; set; }
        public float Min { get; set; }
        public float Max { get; set; }


        public Game()
            : base(800, 600, GraphicsMode.Default, "OpenTK")
        {
            VSync = VSyncMode.On;
            
            waterShader = new ShaderProgram();
            Shader waterVertexShader = new Shader("water.vert", ShaderType.VertexShader);
            Shader waterFragmentShader = new Shader("water.frag", ShaderType.FragmentShader);
            waterShader.AttachShaders(waterVertexShader, waterFragmentShader);
            waterShader.LinkProgram();

            guiShader = new ShaderProgram();
            Shader guiVertexShader = new Shader("gui.vert", ShaderType.VertexShader);
            Shader guiFragmentShader = new Shader("gui.frag", ShaderType.FragmentShader);
            guiShader.AttachShaders(guiVertexShader, guiFragmentShader);
            guiShader.LinkProgram();

            terrainShader = new ShaderProgram();
            Shader vertexShader = new Shader("shader.vert", ShaderType.VertexShader);
            Shader fogShader = new Shader("fogShader.vert", ShaderType.VertexShader);
            Shader fragmentShader = new Shader("mainShader.frag", ShaderType.FragmentShader);
            terrainShader.AttachShaders(fogShader, vertexShader, fragmentShader);
            terrainShader.LinkProgram();

            waterShader.Uniforms["projectionMatrix"] = GL.GetUniformLocation(waterShader.ID, "projectionMatrix");
            waterShader.Uniforms["viewMatrix"] = GL.GetUniformLocation(waterShader.ID, "viewMatrix");
            waterShader.Uniforms["modelMatrix"] = GL.GetUniformLocation(waterShader.ID, "modelMatrix");
            waterShader.Uniforms["reflectionTexture"] = GL.GetUniformLocation(waterShader.ID, "reflectionTexture");
            waterShader.Uniforms["refractionTexture"] = GL.GetUniformLocation(waterShader.ID, "refractionTexture");
            waterShader.Uniforms["dudvMapTexture"] = GL.GetUniformLocation(waterShader.ID, "dudvMapTexture");
            waterShader.Uniforms["waveStrength"] = GL.GetUniformLocation(waterShader.ID, "waveStrength");
            waterShader.Uniforms["tiling"] = GL.GetUniformLocation(waterShader.ID, "tiling");
            waterShader.Uniforms["time"] = GL.GetUniformLocation(waterShader.ID, "time");
            waterShader.Uniforms["cameraPosition"] = GL.GetUniformLocation(waterShader.ID, "cameraPosition");

            waterShader.AttribLocation["inPosition"] = GL.GetAttribLocation(waterShader.ID, "inPosition");

            guiShader.Uniforms["modelMatrix"] = GL.GetUniformLocation(guiShader.ID, "modelMatrix");
            guiShader.Uniforms["guiTexture"] = GL.GetUniformLocation(guiShader.ID, "guiTexture");

            guiShader.AttribLocation["inPosition"] = GL.GetAttribLocation(guiShader.ID, "inPosition");

            terrainShader.Uniforms["modelMatrix"] = GL.GetUniformLocation(terrainShader.ID, "modelMatrix");
            terrainShader.Uniforms["viewMatrix"] = GL.GetUniformLocation(terrainShader.ID, "viewMatrix");
            terrainShader.Uniforms["projectionMatrix"] = GL.GetUniformLocation(terrainShader.ID, "projectionMatrix");
            terrainShader.Uniforms["normalMatrix"] = GL.GetUniformLocation(terrainShader.ID, "normalMatrix");
            terrainShader.Uniforms["color"] = GL.GetUniformLocation(terrainShader.ID, "color");
            terrainShader.Uniforms["samplers[0]"] = GL.GetUniformLocation(terrainShader.ID, "samplers[0]");
            terrainShader.Uniforms["samplers[1]"] = GL.GetUniformLocation(terrainShader.ID, "samplers[1]");
            terrainShader.Uniforms["samplers[2]"] = GL.GetUniformLocation(terrainShader.ID, "samplers[2]");
            terrainShader.Uniforms["samplers[3]"] = GL.GetUniformLocation(terrainShader.ID, "samplers[3]");
            terrainShader.Uniforms["samplers[4]"] = GL.GetUniformLocation(terrainShader.ID, "samplers[4]");
            terrainShader.Uniforms["samplers[5]"] = GL.GetUniformLocation(terrainShader.ID, "samplers[5]");
            terrainShader.Uniforms["light.color"] = GL.GetUniformLocation(terrainShader.ID, "light.color");
            terrainShader.Uniforms["light.direction"] = GL.GetUniformLocation(terrainShader.ID, "light.direction");
            terrainShader.Uniforms["light.ambientIntensity"] = GL.GetUniformLocation(terrainShader.ID, "light.ambientIntensity");
            terrainShader.Uniforms["light.specularIntensity"] = GL.GetUniformLocation(terrainShader.ID, "light.specularIntensity");
            terrainShader.Uniforms["light.specularPower"] = GL.GetUniformLocation(terrainShader.ID, "light.specularPower");
            terrainShader.Uniforms["eyePos"] = GL.GetUniformLocation(terrainShader.ID, "eyePos");
            terrainShader.Uniforms["fog.color"] = GL.GetUniformLocation(terrainShader.ID, "fog.color");
            terrainShader.Uniforms["fog.start"] = GL.GetUniformLocation(terrainShader.ID, "fog.start");
            terrainShader.Uniforms["fog.end"] = GL.GetUniformLocation(terrainShader.ID, "fog.end");
            terrainShader.Uniforms["fog.density"] = GL.GetUniformLocation(terrainShader.ID, "fog.density");
            terrainShader.Uniforms["fog.type"] = GL.GetUniformLocation(terrainShader.ID, "fog.type");
            terrainShader.Uniforms["plane"] = GL.GetUniformLocation(terrainShader.ID, "plane");

            terrainShader.AttribLocation["inPosition"] = GL.GetAttribLocation(terrainShader.ID, "inPosition");
            terrainShader.AttribLocation["inCoord"] = GL.GetAttribLocation(terrainShader.ID, "inCoord");
            terrainShader.AttribLocation["inNormal"] = GL.GetAttribLocation(terrainShader.ID, "inNormal");
            terrainShader.AttribLocation["inNormalizedHeight"] = GL.GetAttribLocation(terrainShader.ID, "inNormalizedHeight");
            terrainShader.AttribLocation["inMoisture"] = GL.GetAttribLocation(terrainShader.ID, "inMoisture");

            terrainGenerator = new TerrainGenerator();
            camera = Camera.Instance;
            camera.scene = this;
            fbo = new WaterFBO(ClientRectangle.Width, ClientRectangle.Height);
            
            light = new Light(-45.0f)
            {
                Color = new Vector3(1.0f, 1.0f, 1.0f),
                AmbientIntensity = 0.25f,
                SpecularIntensity = 20,
                SpecularPower = 10
            };
            FogColor = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
            rock = new Texture2D("rock.jpg");
            mossyrock = new Texture2D("mossyrock.jpg");
            water = new Texture2D("water.jpg");//grass2.jpg || water.jpg
            darkgrass = new Texture2D("darkgrass.jpg");
            dirt = new Texture2D("dirt.jpg");
            sand = new Texture2D("sand.jpg");
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            vao = new VAO();
            terrain = new VAO();
            waterPlane = new VAO();
            skybox = new Skybox(W);

            terrainGenerator.GenerateHeightMap(terrain, W, H, topLeft, bottomLeft, bottomRight, topRight, roughness, min, max);
            terrainGenerator.NormalizeHeightMap(terrain);
            HorizontalPlaneReflection = new Vector4(0.0f, 1.0f, 0.0f, -terrain.WaterHeight);
            HorizontalPlaneRefraction = new Vector4(0.0f, -1.0f, 0.0f, terrain.WaterHeight);
            HorizontalPlaneWorld = new Vector4(0.0f, -1.0f, 0.0f, 100000);

            myTriangle = new Vector3[W * H];
            for (int i = 0; i < W; i++)
                for (int j = 0; j < H; j++)
                    myTriangle[i * W + j] = new Vector3(j * zoom, terrain.WaterHeight, i * zoom);

            waterPlane.BindVerticesBuffer(myTriangle);
            
            guiReflect = new Gui(fbo.ReflectionTexture, new Vector3(-0.7f, 0.5f, 0.0f), new Vector3(0.25f, 0.25f, 0.0f));
            guiRefract = new Gui(fbo.RefractionTexture, new Vector3(0.7f, 0.5f, 0.0f), new Vector3(0.25f, 0.25f, 0.0f));
            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            camera.OnResize(Width, Height);
            fbo.OnResize(Width, Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            GetInput();
            camera.UpdateView(OpenTK.Input.Mouse.GetState(), OpenTK.Input.Keyboard.GetState());
        }
        private bool isEdited = false;
        private void GetInput()
        {
            if (Keyboard[Key.Tilde] && !isEdited)
            {
                string buffer = Console.ReadLine();
                if (buffer.Contains("save heightmap"))
                {
                    SaveHeightMap();
                }
                else if (buffer.Contains("mesh"))
                {
                    renderMode = RenderMode.Mesh;
                    isInit = false;
                }
                else if (buffer.Contains("textured"))
                {
                    renderMode = RenderMode.Textured;
                }
                isEdited = true;
            }
        }
        private bool isInit = false;
        private void SaveHeightMap()
        {
            Bitmap bitmap = new Bitmap(W, H);
            float[,] normalizedHeightMap = terrainGenerator.NormalizedHeightMap;
            for (int i = 0; i < W; i++)
                for (int j = 0; j < H; j++)
                {
                    Color4 color = new Color4(normalizedHeightMap[i, j], normalizedHeightMap[i, j], normalizedHeightMap[i, j], 1);
                    bitmap.SetPixel(i, j, (Color)color);
                }
            bitmap.Save("Assets/HeightMap.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            //testDraw();
            GL.Enable(EnableCap.DepthTest);
            //RenderMesh();
            fbo.BindReflectionFrameBuffer();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            float distance = 2*(camera.Eye.Y - terrain.WaterHeight);
            camera.Eye.Y -= distance;
            camera.Pitch = -camera.Pitch;
            camera.Update();

            skybox.Render(camera.Projection, camera.ModelView);
            RenderTexturedHeightMap(HorizontalPlaneReflection);
            
            fbo.BindRefractionFrameBuffer();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            camera.Eye.Y += distance;
            camera.Pitch = -camera.Pitch;
            camera.Update();

            skybox.Render(camera.Projection, camera.ModelView);
            RenderTexturedHeightMap(HorizontalPlaneRefraction);

            fbo.UnbindFrameBuffer();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            RenderTexturedHeightMap(HorizontalPlaneWorld);
            RenderWater();
            skybox.Render(camera.Projection, camera.ModelView);
            RenderGui(guiReflect);
            RenderGui(guiRefract);
            SwapBuffers();
            
            isEdited = false;
        }
        
        private void RenderMesh()
        {
            if (!isInit && renderMode == RenderMode.Mesh)
            {
                terrainGenerator.CreateMesh(zoom, terrain);

                GL.Color3(Color.Red);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

                GL.Enable(EnableCap.VertexArray);
                GL.Enable(EnableCap.PrimitiveRestart);
                GL.PrimitiveRestartIndex(terrain.IndicesCount);
                GL.BindBuffer(BufferTarget.ArrayBuffer, terrain.VerticesBuffer);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, terrain.IndicesBuffer);
                GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, 0);

                isInit = true;
            }
            GL.Enable(EnableCap.VertexArray);
            GL.Enable(EnableCap.PrimitiveRestart);

            GL.DrawElements(BeginMode.TriangleStrip, terrain.IndicesCount, DrawElementsType.UnsignedInt, 0);

            GL.Disable(EnableCap.VertexArray);
            GL.Disable(EnableCap.PrimitiveRestart);
        }
        private void RenderGui(Gui gui)
        {
            guiShader.Start();
            GL.Enable(EnableCap.Texture2D);
            //GL.Disable(EnableCap.DepthTest);
            GL.BindVertexArray(gui.Vao.ID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, gui.Vao.VerticesBuffer);
            GL.EnableVertexAttribArray(guiShader.AttribLocation["inPosition"]);
            GL.VertexAttribPointer(guiShader.AttribLocation["inPosition"], 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, gui.TextureID);

            GL.Uniform1(guiShader.Uniforms["guiTexture"], 0);
            GL.UniformMatrix4(guiShader.Uniforms["modelMatrix"], false, ref gui.ModelMatrix);

            GL.DrawArrays(BeginMode.TriangleStrip, 0, 4);

            guiShader.Stop();
            GL.Disable(EnableCap.Texture2D);
            //GL.Enable(EnableCap.DepthTest);
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
        private void RenderTexturedHeightMap(Vector4 horizontalPlane)
        {
            //if (!isInit && renderMode == RenderMode.Textured)
            {
                if (!isInit)
                    terrainGenerator.CreateMesh(zoom, terrain);

                terrainShader.Start();
                GL.BindVertexArray(terrain.ID);

                GL.BindBuffer(BufferTarget.ArrayBuffer, terrain.VerticesBuffer);
                GL.EnableVertexAttribArray(terrainShader.AttribLocation["inPosition"]);
                GL.VertexAttribPointer(terrainShader.AttribLocation["inPosition"], 3, VertexAttribPointerType.Float, false, 0, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, terrain.TexCoordsBuffer);
                GL.EnableVertexAttribArray(terrainShader.AttribLocation["inCoord"]);
                GL.VertexAttribPointer(terrainShader.AttribLocation["inCoord"], 2, VertexAttribPointerType.Float, false, 0, 0);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, terrain.IndicesBuffer);

                GL.BindBuffer(BufferTarget.ArrayBuffer, terrain.NormalsBuffer);
                GL.EnableVertexAttribArray(terrainShader.AttribLocation["inNormal"]);
                GL.VertexAttribPointer(terrainShader.AttribLocation["inNormal"], 3, VertexAttribPointerType.Float, false, 0, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, terrain.NormalizedHeightsBuffer);
                GL.EnableVertexAttribArray(terrainShader.AttribLocation["inNormalizedHeight"]);
                GL.VertexAttribPointer(terrainShader.AttribLocation["inNormalizedHeight"], 1, VertexAttribPointerType.Float, false, 0, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, terrain.MoisturesBuffer);
                GL.EnableVertexAttribArray(terrainShader.AttribLocation["inMoisture"]);
                GL.VertexAttribPointer(terrainShader.AttribLocation["inMoisture"], 1, VertexAttribPointerType.Float, false, 0, 0);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, rock.ID);
                GL.BindSampler((int)TextureUnit.Texture0, rock.SamplerID);

                GL.ActiveTexture(TextureUnit.Texture1);
                GL.BindTexture(TextureTarget.Texture2D, water.ID);
                GL.BindSampler((int)TextureUnit.Texture1, water.SamplerID);

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

                
                isInit = true;
            }

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.PrimitiveRestart);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.ClipPlane0);
            GL.PrimitiveRestartIndex(terrain.IndicesCount);


            GL.UniformMatrix4(terrainShader.Uniforms["projectionMatrix"], false, ref camera.Projection);
            GL.UniformMatrix4(terrainShader.Uniforms["modelMatrix"], false, ref model);
            GL.UniformMatrix4(terrainShader.Uniforms["viewMatrix"], false, ref camera.ModelView);
            GL.UniformMatrix4(terrainShader.Uniforms["normalMatrix"], false, ref camera.Normal);
            GL.Uniform3(terrainShader.Uniforms["eyePos"], ref camera.Eye);
            GL.Uniform1(terrainShader.Uniforms["samplers[0]"], (int)TextureUnit.Texture0);
            GL.Uniform1(terrainShader.Uniforms["samplers[1]"], 1);
            GL.Uniform1(terrainShader.Uniforms["samplers[2]"], 2);
            GL.Uniform1(terrainShader.Uniforms["samplers[3]"], 3);
            GL.Uniform1(terrainShader.Uniforms["samplers[4]"], 4);
            GL.Uniform1(terrainShader.Uniforms["samplers[5]"], 5);
            GL.Uniform4(terrainShader.Uniforms["color"], new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
            GL.Uniform3(terrainShader.Uniforms["light.color"], ref light.Color);
            GL.Uniform3(terrainShader.Uniforms["light.direction"], ref light.Direction);
            GL.Uniform1(terrainShader.Uniforms["light.ambientIntensity"], light.AmbientIntensity);
            GL.Uniform1(terrainShader.Uniforms["light.specularIntensity"], specularIntensity);
            GL.Uniform1(terrainShader.Uniforms["light.specularPower"], specularPower);
            GL.Uniform4(terrainShader.Uniforms["fog.color"], ref FogColor);
            GL.Uniform1(terrainShader.Uniforms["fog.density"], 0.001f);
            GL.Uniform1(terrainShader.Uniforms["fog.start"], 30.0f);
            GL.Uniform1(terrainShader.Uniforms["fog.end"], 100.0f);
            GL.Uniform1(terrainShader.Uniforms["fog.type"], 2);
            GL.Uniform4(terrainShader.Uniforms["plane"], horizontalPlane);

            GL.DrawElements(BeginMode.TriangleStrip, terrain.IndicesCount, DrawElementsType.UnsignedInt, 0);
            //terrainShader.Stop();
            GL.Disable(EnableCap.PrimitiveRestart);
            GL.Disable(EnableCap.Texture2D);
        }
        private void RenderWater()
        {
            waterShader.Start();
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.PrimitiveRestart);
            GL.PrimitiveRestartIndex(terrain.IndicesCount);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, fbo.ReflectionTexture);
            GL.BindSampler((int)TextureUnit.Texture0, fbo.ReflectionSampler);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, fbo.RefractionTexture);
            GL.BindSampler((int)TextureUnit.Texture1, fbo.RefractionSampler);

            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, fbo.DudvMapTexture.ID);
            GL.BindSampler((int)TextureUnit.Texture2, fbo.DudvMapTexture.ID);

            time += waterSpeed;
            time %= 1;
            GL.UniformMatrix4(waterShader.Uniforms["projectionMatrix"], false, ref camera.Projection);
            GL.UniformMatrix4(waterShader.Uniforms["modelMatrix"], false, ref model);
            GL.UniformMatrix4(waterShader.Uniforms["viewMatrix"], false, ref camera.ModelView);
            GL.Uniform1(waterShader.Uniforms["reflectionTexture"], 0);
            GL.Uniform1(waterShader.Uniforms["refractionTexture"], 1);
            GL.Uniform1(waterShader.Uniforms["dudvMapTexture"], 2);
            GL.Uniform1(waterShader.Uniforms["waveStrength"], wave);
            GL.Uniform1(waterShader.Uniforms["tiling"], tiling);
            GL.Uniform1(waterShader.Uniforms["time"], time);
            GL.Uniform3(waterShader.Uniforms["cameraPosition"], ref camera.Eye);

            GL.BindBuffer(BufferTarget.ArrayBuffer, waterPlane.VerticesBuffer);
            GL.EnableVertexAttribArray(terrainShader.AttribLocation["inPosition"]);
            GL.VertexAttribPointer(terrainShader.AttribLocation["inPosition"], 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, terrain.IndicesBuffer);
            GL.DrawElements(BeginMode.TriangleStrip, terrain.IndicesCount, DrawElementsType.UnsignedInt, 0);
            waterShader.Stop();

        }
        private void testDraw()
        {
            vao.BindVerticesBuffer(myTriangle);
            vao.BindIndicesBuffer(indices);
            GL.Color3(Color.Green);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            GL.Enable(EnableCap.PrimitiveRestart);
            GL.Enable(EnableCap.VertexArray);
            GL.PrimitiveRestartIndex(10);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vao.VerticesBuffer);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, vao.IndicesBuffer);
            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, 0);
            GL.DrawElements(BeginMode.TriangleStrip, indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.Disable(EnableCap.VertexArray);
        }
        VAO vao;
        Vector3[] myTriangle;
        uint[] indices = new uint[]
        {
            0, 1, 2, 3, 4, 5, 10, 1, 6, 3, 7, 5, 8
        };
        static void Main()
        {
            Game game = new Game();
            using (game)
            {
                game.Run(30.0);
            }
        }
    }
}