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
                terrainGenerator.GenerateHeightMap(W, H, 5, 5, 5, 5, 10);
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
            
            camera.UpdateView(OpenTK.Input.Mouse.GetState(), OpenTK.Input.Keyboard.GetState());
        }
        
        private bool isCalculated = false;
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
           
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //SpriteHandler.Begin(Width, Height);
            //view.Transform();
            
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, 0);

            GL.Color3(Color.Red);
            //GL.DrawElements(BeginMode.TriangleStrip, W*H + (W-1)*(H-2), DrawElementsType.UnsignedInt, ())
            for (int i = 0; i < W; i++)
            {
                GL.Begin(BeginMode.LineStrip);
                for (int j = 0; j < H; j++)
                {
                    GL.Vertex3(i, terrainGenerator.HeightMap[i, j], j);
                }
                GL.End();
            }
            for (int i = 0; i < H; i++)
            {
                GL.Begin(BeginMode.LineStrip);
                for (int j = 0; j < W; j++)
                {
                    GL.Vertex3(j, terrainGenerator.HeightMap[j, i], i);
                }
                GL.End();
            }
            GL.Flush();
            //GL.PushMatrix();
            //GL.PopMatrix();
            SwapBuffers();
        }
        static void Main()
        {
            using (Game game = new Game())
            {
                game.Run(30.0);
            }
        }
    }
}