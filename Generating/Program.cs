using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace StarterKit
{
    class Game : GameWindow
    {
        private static int ScreenW = 10;
        private static int ScreenH = 10;
        private static float[,] brokenLine;
        private static int N = 100000;

        public Game()
            : base(800, 600, GraphicsMode.Default, "OpenTK")
        {
            VSync = VSyncMode.On;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            brokenLine = new float[2, N + 1];
            brokenLine[1, 0] = -3.5f;
            brokenLine[1, N] = -3.5f;
            for (int i = 0; i < brokenLine.GetLength(1); i++)
            {
                brokenLine[0, i] = -5f + i * (float)(ScreenW) / N;
            }
            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            
            Matrix4 projection = Matrix4.CreateOrthographic(ScreenW, ScreenH, -1, 1);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (Keyboard[Key.Escape])
                Exit();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);


            DrawBrokenDiagonal();

            SwapBuffers();
        }

        private bool isCalculated = false;
        private void DrawBrokenDiagonal()
        {
            if (!isCalculated)
            {
                BrokenDiagonal(0, N, -3.5f, -3.5f);
                isCalculated = true;
            }
            GL.Color3(System.Drawing.Color.Red);
            GL.Begin(BeginMode.Polygon);

            for (int i = 0; i < brokenLine.GetLength(1); i++)
            {
                Vector3d vec = new Vector3d(brokenLine[0, i], brokenLine[1, i], 0);
                GL.Vertex3(vec);
            }
            GL.Vertex3(5.0f, -5f, 0);
            GL.Vertex3(-5.0f, -5f, 0);
            GL.End();
        }

        private float roughness = 0.7f;
        private Random rand = new Random();
        private void BrokenDiagonal(int left, int right, float valL, float valR)
        {
            if (right - left == 1)
                return;
            int midPoint = (right + left) / 2;
            float lineLength = brokenLine[0, right] - brokenLine[0, left];
            brokenLine[1, midPoint] = (valR + valL) / 2 + Math.Abs(Random(lineLength * -roughness, lineLength * roughness));
            BrokenDiagonal(left, midPoint, brokenLine[1, left], brokenLine[1, midPoint]);
            BrokenDiagonal(midPoint, right, brokenLine[1, midPoint], brokenLine[1, right]);
        }
        private float Random(float low, float high)
        {
            return (float)rand.NextDouble() * (high - low) + low;
        }

        void DrawTriangle()
        {
            GL.Begin(BeginMode.Triangles);

            //GL.Color3(1.0f, 1.0f, 0.0f);
            GL.Color3(System.Drawing.Color.Red);
            GL.Vertex3(-1.0f, -1.0f, 0f);
            //GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Color3(System.Drawing.Color.Green);
            GL.Vertex3(1.0f, -1.0f, 0f);
            //GL.Color3(0.2f, 0.9f, 1.0f);
            GL.Color3(System.Drawing.Color.Blue);
            GL.Vertex3(0.0f, 1.0f, 0f);

            GL.End();
        }
        void DrawRectangle()
        {
            GL.Begin(BeginMode.Polygon);

            GL.Color3(System.Drawing.Color.Black);
            GL.Vertex3(-1.0f, -1.0f, 0f);
            GL.Vertex3(-1.0f, 1.0f, 0f);
            GL.Color3(System.Drawing.Color.DarkRed);
            GL.Vertex3(1.0f, 1.0f, 0f);
            GL.Vertex3(1.0f, -1.0f, 0f);

            GL.End();

        }
        [STAThread]
        static void Main()
        {
            // The 'using' idiom guarantees proper resource cleanup.
            // We request 30 UpdateFrame events per second, and unlimited
            // RenderFrame events (as fast as the computer can handle).
            using (Game game = new Game())
            {
                game.Run(30.0, 0.5);
            }
        }
    }
}