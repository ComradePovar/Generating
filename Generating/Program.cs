using System;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Generating
{
    class Game : GameWindow
    {
        private Texture2D texture;
        private View view;
        private int W = 65;
        private int H = 65;
        TerrainGenerator terrainGenerator;
        Camera camera;
        Vector3[] vertBuffer;
        int VBO;
        float zoom = 1f;
        private float min = 0;
        private float max = 5;
        private float roughness = 2;
        private float topLeft = 0;
        private float bottomLeft = 0;
        private float bottomRight = 0;
        private float topRight = 0;

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
            //GL.Enable(EnableCap.Texture2D);
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

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4f, Width / (float)Height, 1.0f, 2000.0f);
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
                }
                else if (buffer.Contains("min "))
                {
                    Min = float.Parse(buffer.Remove(0, 4));
                }
                else if (buffer.Contains("max "))
                {
                    Max = float.Parse(buffer.Remove(0, 4));
                }
                else if (buffer.Contains("topleft "))
                {
                    topLeft = float.Parse(buffer.Remove(0, 8));
                }
                else if (buffer.Contains("botleft "))
                {
                    bottomLeft = float.Parse(buffer.Remove(0, 8));
                }
                else if (buffer.Contains("botright "))
                {
                    bottomRight = float.Parse(buffer.Remove(0, 9));
                }
                else if (buffer.Contains("topright "))
                {
                    topRight = float.Parse(buffer.Remove(0, 9));
                }
                terrainGenerator.GenerateHeightMap(W, H, topLeft, bottomLeft, bottomRight, topRight, roughness, min, max);
                isEdited = true;
            }
        }
        private bool isCalculated = false;
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
           
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Color3(Color.Red);
            //GL.DrawElements(BeginMode.TriangleStrip, W * H + (W - 1) * (H - 2), DrawElementsType.UnsignedInt, ())

            int triangleCount = 0;
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            for (int i = 0; i < W - 1; i++)
                for (int j = 0; j < H - 1; j++)
                {
                    float z = i * zoom;
                    float x = j * zoom;

                    GL.Begin(BeginMode.TriangleStrip);

                    GL.Vertex3(x, terrainGenerator.HeightMap[i, j], z);
                    GL.Vertex3(x + zoom, terrainGenerator.HeightMap[i, j + 1], z);
                    GL.Vertex3(x, terrainGenerator.HeightMap[i + 1, j], z + zoom);
                    GL.Vertex3(x + zoom, terrainGenerator.HeightMap[i + 1, j + 1], z + zoom);

                    GL.End();
                    triangleCount += 2;
                }
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Flush();
            SwapBuffers();

            if (isRendered)
                Console.WriteLine(triangleCount);
            isRendered = false;
            isEdited = false;
        }
        bool isRendered = true;
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