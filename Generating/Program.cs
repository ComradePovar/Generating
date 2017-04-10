﻿using System;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using Generating.Shaders;

namespace Generating
{
    enum RenderMode { Mesh, HeightMap, Colored, Textured }
    class Game : GameWindow
    {
        private Texture texture;
        private int W = 513;
        private int H = 513;
        TerrainGenerator terrainGenerator;
        Camera camera;
        
        float zoom = 1f;
        private float min = 0;
        private float max = 5;
        private float roughness = 10f;
        private float topLeft = 0;
        private float bottomLeft = 0;
        private float bottomRight = 0;
        private float topRight = 0;
        private RenderMode renderMode = RenderMode.Colored;
        private VAO terrain;
        private ShaderProgram shaderProgram;
        private Light lightTerrain;
        private int modelView;
        private int projection;
        private int normalMatrix;
        private int sampler1;
        private int sampler2;
        private int texContributor1;
        private int texContributor2;
        private int color;
        private int lightColor;
        private int lightDirection;
        private int lightAmbient;
        private int fogDensity;
        private int fogStart;
        private int fogEnd;
        private int fogColor;
        private int fogType;//TODO: Create enum

        private Matrix4 projectionMatrix;
        Texture grass;
        Texture rock;
        Texture snow;
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

            modelView = GL.GetUniformLocation(shaderProgram.ID, "modelViewMatrix");
            projection = GL.GetUniformLocation(shaderProgram.ID, "projectionMatrix");
            normalMatrix = GL.GetUniformLocation(shaderProgram.ID, "normalMatrix");

            color = GL.GetUniformLocation(shaderProgram.ID, "color");
            sampler1 = GL.GetUniformLocation(shaderProgram.ID, "samplers[0]");
            sampler2 = GL.GetUniformLocation(shaderProgram.ID, "samplers[1]");
            lightColor = GL.GetUniformLocation(shaderProgram.ID, "light.color");
            lightDirection = GL.GetUniformLocation(shaderProgram.ID, "light.direction");
            lightAmbient = GL.GetUniformLocation(shaderProgram.ID, "light.ambientIntensity");
            fogColor = GL.GetUniformLocation(shaderProgram.ID, "fog.color");
            fogStart = GL.GetUniformLocation(shaderProgram.ID, "fog.start");
            fogEnd = GL.GetUniformLocation(shaderProgram.ID, "fog.end");
            fogDensity = GL.GetUniformLocation(shaderProgram.ID, "fog.density");
            fogType = GL.GetUniformLocation(shaderProgram.ID, "fog.type");

            terrainGenerator = new TerrainGenerator();
            camera = Camera.Instance;
            lightTerrain = new Light(-45.0f)
            {
                Color = new Vector3(1.0f, 1.0f, 1.0f),
                AmbientIntensity = 0.25f
            };
            FogColor = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
            grass = new Texture("grass2.jpg");
            rock = new Texture("rock.jpg");
            //darkGrass.SetFiltering(TextureMinFilter.Linear, TextureMagFilter.Linear);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            vao = new VAO();
            terrain = new VAO();
            
            if (!isCalculated)
            {
                terrainGenerator.GenerateHeightMap(W, H, topLeft, bottomLeft, bottomRight, topRight, roughness, min, max);
                terrainGenerator.NormalizeHeightMap(terrain);
                
                isCalculated = true;
            }

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

            //GetInput();
            camera.UpdateView(OpenTK.Input.Mouse.GetState(), OpenTK.Input.Keyboard.GetState());
        }
        private bool isEdited = false;
        //private void GetInput()
        //{
        //    if (Keyboard[Key.Tilde] && !isEdited)
        //    {
        //        string buffer = Console.ReadLine();
        //        if (buffer.Contains("r "))
        //        {
        //            Roughness = float.Parse(buffer.Remove(0, 2));
        //            terrainGenerator.GenerateHeightMap(W, H, topLeft, bottomLeft, bottomRight, topRight, roughness, min, max);
        //            terrainGenerator.NormalizeHeightMap();
        //        }
        //        else if (buffer.Contains("min "))
        //        {
        //            Min = float.Parse(buffer.Remove(0, 4));
        //            terrainGenerator.GenerateHeightMap(W, H, topLeft, bottomLeft, bottomRight, topRight, roughness, min, max);
        //            terrainGenerator.NormalizeHeightMap();
        //        }
        //        else if (buffer.Contains("max "))
        //        {
        //            Max = float.Parse(buffer.Remove(0, 4));
        //            terrainGenerator.GenerateHeightMap(W, H, topLeft, bottomLeft, bottomRight, topRight, roughness, min, max);
        //            terrainGenerator.NormalizeHeightMap();
        //        }
        //        else if (buffer.Contains("topleft "))
        //        {
        //            topLeft = float.Parse(buffer.Remove(0, 8));
        //            terrainGenerator.GenerateHeightMap(W, H, topLeft, bottomLeft, bottomRight, topRight, roughness, min, max);
        //            terrainGenerator.NormalizeHeightMap();
        //        }
        //        else if (buffer.Contains("botleft "))
        //        {
        //            bottomLeft = float.Parse(buffer.Remove(0, 8));
        //            terrainGenerator.GenerateHeightMap(W, H, topLeft, bottomLeft, bottomRight, topRight, roughness, min, max);
        //            terrainGenerator.NormalizeHeightMap();
        //        }
        //        else if (buffer.Contains("botright "))
        //        {
        //            bottomRight = float.Parse(buffer.Remove(0, 9));
        //            terrainGenerator.GenerateHeightMap(W, H, topLeft, bottomLeft, bottomRight, topRight, roughness, min, max);
        //            terrainGenerator.NormalizeHeightMap();
        //        }
        //        else if (buffer.Contains("topright "))
        //        {
        //            topRight = float.Parse(buffer.Remove(0, 9));
        //            terrainGenerator.GenerateHeightMap(W, H, topLeft, bottomLeft, bottomRight, topRight, roughness, min, max);
        //            terrainGenerator.NormalizeHeightMap();
        //        }
        //        else if (buffer.Contains("save heightmap"))
        //        {
        //            SaveHeightMap();
        //        }
        //        else if (buffer.Contains("load texture"))
        //        {
        //            texture = new Texture("land2.jpg");
        //        }
        //        else if (buffer.Contains("mesh"))
        //        {
        //            renderMode = RenderMode.Mesh;
        //        }
        //        else if (buffer.Contains("heightmap"))
        //        {
        //            renderMode = RenderMode.HeightMap;
        //        }
        //        else if (buffer.Contains("colored"))
        //        {
        //            renderMode = RenderMode.Colored;
        //        }
        //        else if (buffer.Contains("textured"))
        //        {
        //            renderMode = RenderMode.Textured;
        //        } else if(buffer.Contains("lightpos "))
        //        {
        //            lightTerrain.SetLightPos(float.Parse(buffer.Remove(0, 9)));
        //        }
        //        isEdited = true;
        //    }
        //}
        private bool isCalculated = false;
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
            

            switch (renderMode)
            {
                case RenderMode.Mesh:
                    RenderMesh();
                    break;
                case RenderMode.HeightMap:
                    RenderHeightMap();
                    break;
                case RenderMode.Colored:
                    RenderGreenHeightMap();
                    break;
                case RenderMode.Textured:
                    RenderTexturedHeightMap();
                    break;
            }
            SwapBuffers();
            
            isEdited = false;
        }
        
        private void RenderMesh()
        {
            if (isCalculated)
            {
                terrainGenerator.CreateMesh(zoom, terrain);
                isCalculated = false;
            }
            GL.Color3(Color.Red);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            GL.Enable(EnableCap.VertexArray);
            GL.Enable(EnableCap.PrimitiveRestart);
            GL.PrimitiveRestartIndex(terrain.IndicesCount);
            GL.BindBuffer(BufferTarget.ArrayBuffer, terrain.VerticesBuffer);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, terrain.IndicesBuffer);
            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, 0);
            GL.DrawElements(BeginMode.TriangleStrip, terrain.IndicesCount, DrawElementsType.UnsignedInt, 0);
            GL.Disable(EnableCap.VertexArray);
            GL.Disable(EnableCap.PrimitiveRestart);
        }
        private void RenderHeightMap()
        {
            float[,] normalizedHeightMap = terrainGenerator.NormalizedHeightMap;
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            for (int i = 0; i < W - 1; i++)
                for (int j = 0; j < H - 1; j++)
                {
                    float z = i * zoom;
                    float x = j * zoom;

                    GL.Begin(BeginMode.TriangleStrip);
                    GL.Color3(normalizedHeightMap[i, j], normalizedHeightMap[i, j], normalizedHeightMap[i, j]);
                    GL.Vertex3(x, 0, z);
                    GL.Color3(normalizedHeightMap[i, j+1], normalizedHeightMap[i, j+1],normalizedHeightMap[i, j+1]);
                    GL.Vertex3(x + zoom, 0, z);
                    GL.Color3(normalizedHeightMap[i+1, j], normalizedHeightMap[i+1, j], normalizedHeightMap[i+1, j]);
                    GL.Vertex3(x, 0, z + zoom);
                    GL.Color3(normalizedHeightMap[i+1, j+1], normalizedHeightMap[i+1, j+1], normalizedHeightMap[i+1, j+1]);
                    GL.Vertex3(x + zoom, 0, z + zoom);

                    GL.End();
                }
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }
        
        private void RenderGreenHeightMap()
        {
            if (isCalculated)
            {
                terrainGenerator.CreateMesh(zoom, terrain);
                isCalculated = false;
            }
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.VertexArray);
            GL.Enable(EnableCap.IndexArray);
            GL.Enable(EnableCap.NormalArray);
           // GL.Enable(EnableCap.ColorArray);
            GL.Enable(EnableCap.PrimitiveRestart);
            GL.PrimitiveRestartIndex(terrain.IndicesCount);

            GL.BindVertexArray(terrain.ID);
            //GL.BindTexture(TextureTarget.Texture2D, grass.ID);

            GL.BindBuffer(BufferTarget.ArrayBuffer, terrain.VerticesBuffer);
            //GL.VertexPointer(3, VertexPointerType.Float, 0, 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, terrain.TexCoordsBuffer);
            //GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, terrain.IndicesBuffer);

            //GL.BindBuffer(BufferTarget.ArrayBuffer, terrain.ColorsBuffer);
            //GL.ColorPointer(3, ColorPointerType.Float, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, terrain.NormalsBuffer);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, terrain.NormalizedHeightsBuffer);
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 1, VertexAttribPointerType.Float, false, 0, 0);


            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, rock.ID);
            GL.BindSampler((int)TextureUnit.Texture0, rock.SamplerID);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, grass.ID);
            GL.BindSampler((int)TextureUnit.Texture1, grass.SamplerID);
            shaderProgram.Start();
            Matrix4 testMatrix = Matrix4.Identity;
            testMatrix.Invert();
            testMatrix.Transpose();

            GL.UniformMatrix4(projection, false, ref camera.Projection);
            GL.UniformMatrix4(modelView, false, ref camera.ModelView);
            GL.UniformMatrix4(normalMatrix, false, ref testMatrix);
            GL.Uniform1(sampler1, 0);
            GL.Uniform1(sampler2, 1);
            GL.Uniform4(color, new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
            GL.Uniform3(lightColor, ref lightTerrain.Color);
            GL.Uniform3(lightDirection, ref lightTerrain.Direction);
            GL.Uniform1(lightAmbient, lightTerrain.AmbientIntensity);
            GL.Uniform4(fogColor, ref FogColor);
            GL.Uniform1(fogDensity, 0.005f);
            GL.Uniform1(fogStart, 30.0f);
            GL.Uniform1(fogEnd, 100.0f);
            GL.Uniform1(fogType, 2);

            GL.DrawElements(BeginMode.TriangleStrip, terrain.IndicesCount, DrawElementsType.UnsignedInt, 0);
            shaderProgram.Stop();

            GL.Disable(EnableCap.VertexArray);
            GL.Disable(EnableCap.IndexArray);
            GL.Disable(EnableCap.ColorArray);
            GL.Disable(EnableCap.PrimitiveRestart);
            GL.Disable(EnableCap.NormalArray);
        }
        private void RenderTexturedHeightMap()
        {
            float[,] heightMap = terrainGenerator.HeightMap;
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.BindTexture(TextureTarget.Texture2D, texture.ID);
            GL.Enable(EnableCap.Texture2D);
            float stepTexCoord = 1.0f / W;
            for (int i = 0; i < W - 1; i++)
                for (int j = 0; j < H - 1; j++)
                {
                    float z = i * zoom;
                    float x = j * zoom;

                    GL.Begin(BeginMode.TriangleStrip);

                    GL.TexCoord2(i * stepTexCoord, j * stepTexCoord);
                    GL.Vertex3(x, heightMap[i, j] * zoom, z);

                    GL.TexCoord2((i + 1) * stepTexCoord, j * stepTexCoord);
                    GL.Vertex3(x + zoom, heightMap[i, j + 1] * zoom, z);

                    GL.TexCoord2(i * stepTexCoord, (j + 1) * stepTexCoord);
                    GL.Vertex3(x, heightMap[i + 1, j] * zoom, z + zoom);

                    GL.TexCoord2((i + 1) * stepTexCoord, (j + 1) * stepTexCoord);
                    GL.Vertex3(x + zoom, heightMap[i + 1, j + 1] * zoom, z + zoom);

                    GL.End();
                }
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