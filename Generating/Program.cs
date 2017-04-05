using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenTK.Platform;
using System.Drawing;

namespace Generating
{
    class Game : GameWindow
    {
        private Texture2D texture;
        private View view;
        private int W = 1000;
        private int H = 1000;
        public Game()
            : base(800, 600, GraphicsMode.Default, "OpenTK")
        {
            VSync = VSyncMode.On;
            GL.Enable(EnableCap.Texture2D);
            view = new View(Vector3.Zero, 1.5f);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            texture = AssetsLoader.LoadTexture("land.png");
            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            //GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            //Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4f, Width / (float)Height, 1.0f, 200.0f);
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadMatrix(ref projection);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            view.Update();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
           
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            SpriteHandler.Begin(Width, Height);
            view.Transform();

            Random rand = new Random(1);
            for (int i = 0; i < 20; i++)
                for (int j = 0; j < 20; j++)
                {
                    int c = rand.Next(1, 4);
                    switch (c)
                    {
                        case 1:
                            SpriteHandler.Draw(texture, Vector2.Zero, new Vector2(1f, 1f), Color.LightGray, new Vector2(10f * j, 10f * i));
                            break;
                        case 2:
                            SpriteHandler.Draw(texture, Vector2.Zero, new Vector2(1f, 1f), Color.Gray, new Vector2(10f * j, 10f * i));
                            break;
                        case 3:
                            SpriteHandler.Draw(texture, Vector2.Zero, new Vector2(1f, 1f), Color.DarkGray, new Vector2(10f * j, 10f * i));
                            break;

                    }
                }

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