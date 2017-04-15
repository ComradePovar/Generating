﻿using System;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using Generating.Shaders;


/* TODO:
 * Redbook 8th edition 
 *- 1) Упростить структуру программы;
 *- 2) Загрузка существующей heightmap;
 *- 4) Разбить terrain на чанки;
 *- 10) UI для задания параметров карты;
 *- 9) Доработать алгоритм генерации;
 *- 6) Shadow map;
 *- 5) specular light;
 *- 8) terrain patterns;
 *- 7) correct blending, texture splatting;
 * */
namespace Generating
{
    enum RenderMode { Mesh, Textured }
    class Game : GameWindow
    {
        private int W = 513;
        private int H = 513;
        TerrainGenerator terrainGenerator;
        Camera camera;
        
        float zoom = 1f;
        private float min = 0;
        private float max = 5;
        private float roughness = 15f;
        private float topLeft = 0;
        private float bottomLeft = 0;
        private float bottomRight = 0;
        private float topRight = 0;
        private RenderMode renderMode = RenderMode.Textured;
        private VAO terrain;
        private ShaderProgram shaderProgram;
        private Light light;

        Texture grass;
        Texture rock;
        Texture mud;
        Texture dirt;
        Vector4 FogColor;
        
        public float Roughness { get; set; }
        public float Min { get; set; }
        public float Max { get; set; }

        public Game()
            : base(800, 600, GraphicsMode.Default, "OpenTK")
        {
            VSync = VSyncMode.On;

            shaderProgram = new ShaderProgram();
            Shader vertexShader = new Shader("shader.vert", ShaderType.VertexShader);
            Shader fogShader = new Shader("fogShader.vert", ShaderType.VertexShader);
            Shader fragmentShader = new Shader("mainShader.frag", ShaderType.FragmentShader);
            shaderProgram.AttachShaders(fogShader, vertexShader, fragmentShader);
            shaderProgram.LinkProgram();
            
            shaderProgram.Uniforms["modelViewMatrix"] = GL.GetUniformLocation(shaderProgram.ID, "modelViewMatrix");
            shaderProgram.Uniforms["projectionMatrix"] = GL.GetUniformLocation(shaderProgram.ID, "projectionMatrix");
            shaderProgram.Uniforms["color"] = GL.GetUniformLocation(shaderProgram.ID, "color");
            shaderProgram.Uniforms["samplers[0]"] = GL.GetUniformLocation(shaderProgram.ID, "samplers[0]");
            shaderProgram.Uniforms["samplers[1]"] = GL.GetUniformLocation(shaderProgram.ID, "samplers[1]");
            shaderProgram.Uniforms["samplers[2]"] = GL.GetUniformLocation(shaderProgram.ID, "samplers[2]");
            shaderProgram.Uniforms["light.color"] = GL.GetUniformLocation(shaderProgram.ID, "light.color");
            shaderProgram.Uniforms["light.direction"] = GL.GetUniformLocation(shaderProgram.ID, "light.direction");
            shaderProgram.Uniforms["light.ambientIntensity"] = GL.GetUniformLocation(shaderProgram.ID, "light.ambientIntensity");
            shaderProgram.Uniforms["fog.color"] = GL.GetUniformLocation(shaderProgram.ID, "fog.color");
            shaderProgram.Uniforms["fog.start"] = GL.GetUniformLocation(shaderProgram.ID, "fog.start");
            shaderProgram.Uniforms["fog.end"] = GL.GetUniformLocation(shaderProgram.ID, "fog.end");
            shaderProgram.Uniforms["fog.density"] = GL.GetUniformLocation(shaderProgram.ID, "fog.density");
            shaderProgram.Uniforms["fog.type"] = GL.GetUniformLocation(shaderProgram.ID, "fog.type");

            shaderProgram.AttribLocation["inPosition"] = GL.GetAttribLocation(shaderProgram.ID, "inPosition");
            shaderProgram.AttribLocation["inCoord"] = GL.GetAttribLocation(shaderProgram.ID, "inCoord");
            shaderProgram.AttribLocation["inNormal"] = GL.GetAttribLocation(shaderProgram.ID, "inNormal");
            shaderProgram.AttribLocation["inNormalizedHeight"] = GL.GetAttribLocation(shaderProgram.ID, "inNormalizedHeight");

            terrainGenerator = new TerrainGenerator();
            camera = Camera.Instance;
            light = new Light(-45.0f)
            {
                Color = new Vector3(1.0f, 1.0f, 1.0f),
                AmbientIntensity = 0.25f
            };
            FogColor = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
            rock = new Texture("rock.jpg");
            mud = new Texture("mossyrock.jpg");
            grass = new Texture("grass2.jpg");
            dirt = new Texture("dirt.jpg");
            //darkGrass.SetFiltering(TextureMinFilter.Linear, TextureMagFilter.Linear);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            vao = new VAO();
            terrain = new VAO();

            terrainGenerator.GenerateHeightMap(W, H, topLeft, bottomLeft, bottomRight, topRight, roughness, min, max);
            terrainGenerator.NormalizeHeightMap(terrain);
                

            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            camera.OnResize(Width, Height);
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
           
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //testDraw();
            
            
            RenderMesh();
            RenderTexturedHeightMap();

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
        
        private void RenderTexturedHeightMap()
        {
            if (!isInit && renderMode == RenderMode.Textured)
            {
                terrainGenerator.CreateMesh(zoom, terrain);

                GL.BindVertexArray(terrain.ID);

                GL.BindBuffer(BufferTarget.ArrayBuffer, terrain.VerticesBuffer);
                GL.EnableVertexAttribArray(shaderProgram.AttribLocation["inPosition"]);
                GL.VertexAttribPointer(shaderProgram.AttribLocation["inPosition"], 3, VertexAttribPointerType.Float, false, 0, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, terrain.TexCoordsBuffer);
                GL.EnableVertexAttribArray(shaderProgram.AttribLocation["inCoord"]);
                GL.VertexAttribPointer(shaderProgram.AttribLocation["inCoord"], 2, VertexAttribPointerType.Float, false, 0, 0);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, terrain.IndicesBuffer);

                GL.BindBuffer(BufferTarget.ArrayBuffer, terrain.NormalsBuffer);
                GL.EnableVertexAttribArray(shaderProgram.AttribLocation["inNormal"]);
                GL.VertexAttribPointer(shaderProgram.AttribLocation["inNormal"], 3, VertexAttribPointerType.Float, false, 0, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, terrain.NormalizedHeightsBuffer);
                GL.EnableVertexAttribArray(shaderProgram.AttribLocation["inNormalizedHeight"]);
                GL.VertexAttribPointer(shaderProgram.AttribLocation["inNormalizedHeight"], 1, VertexAttribPointerType.Float, false, 0, 0);


                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, rock.ID);
                GL.BindSampler((int)TextureUnit.Texture0, rock.SamplerID);
                GL.ActiveTexture(TextureUnit.Texture1);
                GL.BindTexture(TextureTarget.Texture2D, grass.ID);
                GL.BindSampler((int)TextureUnit.Texture1, grass.SamplerID);
                GL.ActiveTexture(TextureUnit.Texture2);
                GL.BindTexture(TextureTarget.Texture2D, mud.ID);
                GL.BindSampler((int)TextureUnit.Texture2, mud.SamplerID);

                shaderProgram.Start();
                
                isInit = true;
            }

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.PrimitiveRestart);
            GL.Enable(EnableCap.CullFace);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
            GL.PrimitiveRestartIndex(terrain.IndicesCount);


            GL.UniformMatrix4(shaderProgram.Uniforms["projectionMatrix"], false, ref camera.Projection);
            GL.UniformMatrix4(shaderProgram.Uniforms["modelViewMatrix"], false, ref camera.ModelView);
            GL.Uniform1(shaderProgram.Uniforms["samplers[0]"], (int)TextureUnit.Texture0);
            GL.Uniform1(shaderProgram.Uniforms["samplers[1]"], 1);
            GL.Uniform1(shaderProgram.Uniforms["samplers[2]"], 2);
            GL.Uniform4(shaderProgram.Uniforms["color"], new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
            GL.Uniform3(shaderProgram.Uniforms["light.color"], ref light.Color);
            GL.Uniform3(shaderProgram.Uniforms["light.direction"], ref light.Direction);
            GL.Uniform1(shaderProgram.Uniforms["light.ambientIntensity"], light.AmbientIntensity);
            GL.Uniform4(shaderProgram.Uniforms["fog.color"], ref FogColor);
            GL.Uniform1(shaderProgram.Uniforms["fog.density"], 0.001f);
            GL.Uniform1(shaderProgram.Uniforms["fog.start"], 30.0f);
            GL.Uniform1(shaderProgram.Uniforms["fog.end"], 100.0f);
            GL.Uniform1(shaderProgram.Uniforms["fog.type"], 2);

            GL.DrawElements(BeginMode.TriangleStrip, terrain.IndicesCount, DrawElementsType.UnsignedInt, 0);

            GL.Disable(EnableCap.PrimitiveRestart);
            GL.Disable(EnableCap.Texture2D);
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
        Vector3[] myTriangle = new Vector3[]
             {
            new Vector3(0f, 0f, 0f), //0
            new Vector3(0f, 1f, 0f), //1
            new Vector3(1f, 0f, 0f), //2
            new Vector3(1f, 1f, 0f), //3
            new Vector3(2f, 0f, 0f), //4
            new Vector3(2f, 1f, 0f), //5
            new Vector3(0f, 2f, 0f), //6
            new Vector3(1f, 2f, 0f), //7
            new Vector3(2f, 2f, 0f) //8
             };
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