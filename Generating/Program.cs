using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Drawing;
using System.Drawing.Imaging;

namespace Generating
{
    class Game : GameWindow
    {
        private static int ScreenW = 10;
        private static int ScreenH = 10;
        private static float[,] brokenLine;
        private static int N = 100000;

        public Game()
            : base(800, 800, GraphicsMode.Default, "OpenTK")
        {
            VSync = VSyncMode.On;
        }


        Matrix4 projection;
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)Width / Height, 1, 1000);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (Keyboard[Key.Escape])
                Exit();
        }
        int textureId = 0;
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
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            Bitmap bmp = new Bitmap("Assets/land.jpg");
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
            shader = new ShaderProgram();
            vao = new VAO();
        }
        ShaderProgram shader;
        VAO vao;
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0f, 4f, 0),
            new Vector3(4f, 2f, 0),
            new Vector3(8f, 3f, 0),
            new Vector3(12, 1, 0),
            new Vector3(0, 3, 4),
            new Vector3(4f, 5f, 4),
            new Vector3(8f, 8f, 4),
            new Vector3(12f, 2f, 4),
            new Vector3(0, 7, 8),
            new Vector3(4f, 10f, 8),
            new Vector3(8f, 12f, 8),
            new Vector3(12, 6, 8),
            new Vector3(0, 4, 12),
            new Vector3(4f, 6f, 12),
            new Vector3(8f, 8f, 12),
            new Vector3(12, 3, 12),

        };
        uint[] indices = new uint[]
        {
            0, 4, 1, 5, 2, 6, 3, 7, 16,
            4, 8, 5, 9, 6, 10, 7, 11, 16,
            8, 12, 9, 13, 10, 14, 11, 15
        };
        Vector3[] colors = new Vector3[] { new Vector3(0, 0, 1f), new Vector3(0, 1f, 0), new Vector3(1f, 0, 0), new Vector3(0, 1, 0f), new Vector3(0, 0, 1f) };
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //GL.Enable(EnableCap.Texture2D);
            //GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Replace);
            DrawTriangle();

           // DrawQuad();

            //Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);
            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadMatrix(ref modelview);
            
            SwapBuffers();
        }
        float angle1 = 0;
        float angle2 = 0;
        void DrawQuad()
        {
            GL.Begin(BeginMode.Quads);


            //GL.Color3(System.Drawing.Color.Red);
            //GL.TexCoord2(0, 1);
            GL.Vertex3(0.0f, -1.0f, 0f);
            //GL.Color3(1.0f, 0.0f, 0.0f);
            //GL.Color3(System.Drawing.Color.Green);
            //GL.TexCoord2(1, 1);
            GL.Vertex3(2.0f, -1.0f, 0f);
            //GL.Color3(0.2f, 0.9f, 1.0f);
            //GL.Color3(System.Drawing.Color.Blue);
            //GL.TexCoord2(1, 0);
            GL.Vertex3(2.0f, 1.0f, 0f);
            //GL.Color3(Color.Yellow);
            //GL.TexCoord2(0, 0);
            GL.Vertex3(0.0f, 1.0f, 0f);


            GL.End();
        }
        void DrawTriangle()
        {
            GL.Enable(EnableCap.VertexArray);
            GL.Enable(EnableCap.IndexArray);
            GL.Enable(EnableCap.PrimitiveRestart);
            GL.PrimitiveRestartIndex(16);

            vao.BindVerticesBuffer(vertices);
            vao.BindIndicesBuffer(indices);

            GL.BindVertexArray(vao.ID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vao.VerticesBuffer);
            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, vao.IndicesBuffer);
            shader.UseProgram();
            //GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
            //GL.DrawArrays(BeginMode.Triangles, 0, indices.Length);

            int modelView = GL.GetUniformLocation(shader.ID, "modelViewMatrix");
            int projection = GL.GetUniformLocation(shader.ID, "projectionMatrix");
            GL.UniformMatrix4(projection, false, ref this.projection);

            

            
            //
            //GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Matrix4 modelview = Matrix4.LookAt(new Vector3(8, 40, -8), new Vector3(3, 0, 3), new Vector3(0.0f, 1.0f, 0.0f));
            Matrix4 current = modelview;
            GL.UniformMatrix4(modelView, false, ref current);
            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadMatrix(ref modelview);
            GL.DrawElements(BeginMode.TriangleStrip, indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);

            //GL.Begin(BeginMode.Triangles);


            //GL.Color3(System.Drawing.Color.Red);
            //GL.TexCoord2(0, 1);
            //GL.Vertex3(-2.0f, -1.0f, 0f);
            ////GL.Color3(1.0f, 0.0f, 0.0f);
            //GL.Color3(System.Drawing.Color.Green);
            //GL.TexCoord2(1, 1);
            //GL.Vertex3(0.0f, -1.0f, 0f);
            ////GL.Color3(0.2f, 0.9f, 1.0f);
            //GL.Color3(System.Drawing.Color.Blue);
            //GL.TexCoord2(0.5, 0);
            //GL.Vertex3(-1.0f, 1.0f, 0f);

            //GL.End();
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