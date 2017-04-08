using System;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Generating
{
    enum RenderMode { Mesh, HeightMap, Colored, Textured }
    class Game : GameWindow
    {
        private Texture texture;
        private View view;
        private int W = 65;
        private int H = 65;
        TerrainGenerator terrainGenerator;
        Camera camera;
        Vector3[] vertBuffer;
        int VBO;
        Color[,] ColorMap;
        float zoom = 1f;
        private float min = 0;
        private float max = 5;
        private float roughness = 3;
        private float topLeft = 0;
        private float bottomLeft = 0;
        private float bottomRight = 0;
        private float topRight = 0;
        private RenderMode renderMode = RenderMode.Mesh;

        public float Roughness
        {
            get
            {
                return roughness;
            }
            set
            {
                roughness = value;
            }
        }
        public float Min
        {
            get
            {
                return min;
            }
            set
            {
                min = value;
            }
        }
        public float Max
        {
            get
            {
                return max;
            }
            set
            {
                max = value;
            }
        }
        public Game()
            : base(800, 600, GraphicsMode.Default, "OpenTK")
        {
            VSync = VSyncMode.On;
            //view = new View(Vector3.Zero, 1.5f);
            terrainGenerator = new TerrainGenerator();
            camera = Camera.Instance;
        }
        Matrix4 transform;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!isCalculated)
            {
                terrainGenerator.GenerateHeightMap(W, H, topLeft, bottomLeft, bottomRight, topRight, roughness, min, max);
                terrainGenerator.NormalizeHeightMap();
                
                isCalculated = true;
            }
            //texture = AssetsLoader.LoadTexture("land.png");
            transform = Matrix4.Identity * Matrix4.CreateTranslation(1, 0, 0);
            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4f, Width / (float)Height, 1.0f, 1000.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
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
                if (buffer.Contains("r "))
                {
                    Roughness = float.Parse(buffer.Remove(0, 2));
                    terrainGenerator.GenerateHeightMap(W, H, topLeft, bottomLeft, bottomRight, topRight, roughness, min, max);
                    terrainGenerator.NormalizeHeightMap();
                }
                else if (buffer.Contains("min "))
                {
                    Min = float.Parse(buffer.Remove(0, 4));
                    terrainGenerator.GenerateHeightMap(W, H, topLeft, bottomLeft, bottomRight, topRight, roughness, min, max);
                    terrainGenerator.NormalizeHeightMap();
                }
                else if (buffer.Contains("max "))
                {
                    Max = float.Parse(buffer.Remove(0, 4));
                    terrainGenerator.GenerateHeightMap(W, H, topLeft, bottomLeft, bottomRight, topRight, roughness, min, max);
                    terrainGenerator.NormalizeHeightMap();
                }
                else if (buffer.Contains("topleft "))
                {
                    topLeft = float.Parse(buffer.Remove(0, 8));
                    terrainGenerator.GenerateHeightMap(W, H, topLeft, bottomLeft, bottomRight, topRight, roughness, min, max);
                    terrainGenerator.NormalizeHeightMap();
                }
                else if (buffer.Contains("botleft "))
                {
                    bottomLeft = float.Parse(buffer.Remove(0, 8));
                    terrainGenerator.GenerateHeightMap(W, H, topLeft, bottomLeft, bottomRight, topRight, roughness, min, max);
                    terrainGenerator.NormalizeHeightMap();
                }
                else if (buffer.Contains("botright "))
                {
                    bottomRight = float.Parse(buffer.Remove(0, 9));
                    terrainGenerator.GenerateHeightMap(W, H, topLeft, bottomLeft, bottomRight, topRight, roughness, min, max);
                    terrainGenerator.NormalizeHeightMap();
                }
                else if (buffer.Contains("topright "))
                {
                    topRight = float.Parse(buffer.Remove(0, 9));
                    terrainGenerator.GenerateHeightMap(W, H, topLeft, bottomLeft, bottomRight, topRight, roughness, min, max);
                    terrainGenerator.NormalizeHeightMap();
                }
                else if (buffer.Contains("save heightmap"))
                {
                    SaveHeightMap();
                }
                else if (buffer.Contains("load texture"))
                {
                    texture = new Texture("land2.jpg");
                }
                else if (buffer.Contains("mesh"))
                {
                    renderMode = RenderMode.Mesh;
                }
                else if (buffer.Contains("heightmap"))
                {
                    renderMode = RenderMode.HeightMap;
                }
                else if (buffer.Contains("colored"))
                {
                    renderMode = RenderMode.Colored;
                }
                else if (buffer.Contains("textured"))
                {
                    renderMode = RenderMode.Textured;
                }
                isEdited = true;
            }
        }
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
        private void LoadTexture(string name)
        {
            Bitmap bitmap = new Bitmap("Assets/" + name);
            ColorMap = new Color[W, H];
            for (int i = 0; i < W; i++)
                for (int j = 0; j < H; j++)
                {
                    ColorMap[i, j] = bitmap.GetPixel(i, j);
                }
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
           
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //GL.DrawElements(BeginMode.TriangleStrip, W * H + (W - 1) * (H - 2), DrawElementsType.UnsignedInt, ())
            switch (renderMode)
            {
                case RenderMode.Mesh:
                    RenderMesh();
                    break;
                case RenderMode.HeightMap:
                    RenderHeightMap();
                    break;
                case RenderMode.Colored:
                    RenderColoredHeightMap();
                    break;
                case RenderMode.Textured:
                    RenderTexturedHeightMap();
                    break;
            }
            GL.Flush();
            SwapBuffers();
            
            isEdited = false;
        }
        
        private void RenderMesh()
        {
            float[,] heightMap = terrainGenerator.HeightMap;
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.Color3(Color.Green);
            for (int i = 0; i < W - 1; i++)
                for (int j = 0; j < H - 1; j++)
                {
                    float z = i * zoom;
                    float x = j * zoom;

                    GL.Begin(BeginMode.TriangleStrip);
                    GL.Vertex3(x, heightMap[i, j] * zoom, z);
                    GL.Vertex3(x + zoom, heightMap[i, j + 1] * zoom, z);
                    GL.Vertex3(x, heightMap[i + 1, j] * zoom, z + zoom);
                    GL.Vertex3(x + zoom, heightMap[i + 1, j + 1] * zoom, z + zoom);

                    GL.End();
                }
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
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
        private void RenderColoredHeightMap()
        {
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            for (int i = 0; i < W - 1; i++)
                for (int j = 0; j < H - 1; j++)
                {
                    float z = i * zoom;
                    float x = j * zoom;

                    GL.Begin(BeginMode.TriangleStrip);
                    GL.Color3(ColorMap[i, j]);
                    GL.Vertex3(x, terrainGenerator.HeightMap[i, j] * zoom, z);
                    GL.Color3(ColorMap[i + 1, j]);
                    GL.Vertex3(x + zoom, terrainGenerator.HeightMap[i, j + 1] * zoom, z);
                    GL.Color3(ColorMap[i, j + 1]);
                    GL.Vertex3(x, terrainGenerator.HeightMap[i + 1, j] * zoom, z + zoom);
                    GL.Color3(ColorMap[i + 1, j + 1]);
                    GL.Vertex3(x + zoom, terrainGenerator.HeightMap[i + 1, j + 1] * zoom, z + zoom);

                    GL.End();
                }
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
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