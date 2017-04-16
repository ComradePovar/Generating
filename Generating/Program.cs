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
            if (Keyboard[Key.A])
                eye.X += 0.1f;
            if (Keyboard[Key.D])
                eye.X -= 0.1f;
            if (Keyboard[Key.W])
                eye.Y += 0.1f;
            if (Keyboard[Key.S])
                eye.Y -= 0.1f;
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
        Vector3 eye = new Vector3(0f, 0f, 0f);
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
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f),

        };
        uint[] indices = new uint[]
        {
            0, 1, 2, 3, 3, 2, 6, 7, 4, 5, 6, 7, 4, 5, 1, 0, 1, 2, 6, 5
        };
        Vector3[] colors = new Vector3[] {
            new Vector3(0, 0.2f, 0),
            new Vector3(0, 0.2f, 0),
            new Vector3(0, 0.2f, 0),
            new Vector3(0f, 0.2f, 0f),
            new Vector3(0, 0.2f, 0),
            new Vector3(0, 0.2f, 0),
            new Vector3(0, 0.2f, 0),
            new Vector3(0f, 0.2f, 0f),
        };
        Vector3[] normals = new Vector3[] {
            new Vector3(1f, 0f, 0f),
            new Vector3(1f, 0f, 0f),
            new Vector3(0f, 0f, 1f),
            new Vector3(0f, 0f, 1f),
            new Vector3(0f, 0f, 1f),
            new Vector3(0f, 0f, 1f),
            new Vector3(0f, 0f, 1f),
            new Vector3(0f, 0f, 1f),
        };
        float k;
        void DrawTriangle()
        {
            GL.Enable(EnableCap.VertexArray);
            GL.Enable(EnableCap.IndexArray);
            GL.Enable(EnableCap.NormalArray);
            GL.Enable(EnableCap.ColorArray);
            GL.Enable(EnableCap.PrimitiveRestart);
            GL.PrimitiveRestartIndex(16);

            vao.BindVerticesBuffer(vertices);
            vao.BindIndicesBuffer(indices);
            vao.BindColorsBuffer(colors);
            vao.BindNormalsBuffer(normals);

            GL.BindVertexArray(vao.ID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vao.VerticesBuffer);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vao.ColorsBuffer);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vao.NormalsBuffer);
            GL.NormalPointer(NormalPointerType.Float, 0, 0);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, 0);
            
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, vao.IndicesBuffer);
            shader.UseProgram();
            //GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
            //GL.DrawArrays(BeginMode.Triangles, 0, indices.Length);

            //Matrix4 model = Matrix4.CreateTranslation(0, 0, 1);
            Matrix4 model = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(k));
            // eye = new Vector3(2f, 1f, 4f);
            k += 0.01f;
            Matrix4 view = Matrix4.LookAt(eye, new Vector3(0, 0, 0f), new Vector3(0.0f, 1.0f, 0.0f));
            Matrix4 normal = Matrix4.Transpose(Matrix4.Invert(view * model));
            Vector3 direction = new Vector3(-10f, 5f, -10f);
            direction.Normalize();
            direction = -direction;
            Vector3 color = new Vector3(1f, 1f, 1f);

            int modelMatrix = GL.GetUniformLocation(shader.ID, "modelMatrix");
            GL.UniformMatrix4(modelMatrix, false, ref model);
            int viewMatrix = GL.GetUniformLocation(shader.ID, "viewMatrix");
            int projection = GL.GetUniformLocation(shader.ID, "projectionMatrix");
            int normalMatrix = GL.GetUniformLocation(shader.ID, "normalMatrix");
            int lDirection = GL.GetUniformLocation(shader.ID, "lDirection");
            int lColor = GL.GetUniformLocation(shader.ID, "lColor");
            int ambientIntensity = GL.GetUniformLocation(shader.ID, "ambientIntensity");
            int specularPower = GL.GetUniformLocation(shader.ID, "specularPower");
            int specularIntensity = GL.GetUniformLocation(shader.ID, "specularIntensity");
            int eyePos = GL.GetUniformLocation(shader.ID, "eyePos");

            GL.UniformMatrix4(projection, false, ref this.projection);
            GL.Uniform3(lDirection, ref direction);
            GL.Uniform1(ambientIntensity, 1f);
            GL.Uniform3(lColor, ref color);
            GL.Uniform1(specularPower, 128);
            GL.Uniform1(specularIntensity, -100);
            GL.Uniform3(eyePos, ref eye);
            GL.UniformMatrix4(viewMatrix, false, ref view);
            GL.UniformMatrix4(modelMatrix, false, ref model);
            GL.UniformMatrix4(normalMatrix, false, ref normal);
            //GL.UniformMatrix4(viewMatrix, false, ref model);
            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadMatrix(ref modelview);
            GL.DrawElements(BeginMode.Quads, indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
            GL.UniformMatrix4(modelMatrix, false, ref model);
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