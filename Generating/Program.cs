using System;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using Generating.Shaders;
using Generating.GUI;
using Generating.Textures;
using Generating.SceneObjects;


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

        private ShaderProgram guiShader;

        Matrix4 model = Matrix4.Identity;

        Scene scene;
        
        public float Roughness { get; set; }
        public float Min { get; set; }
        public float Max { get; set; }


        public Game()
            : base(800, 600, GraphicsMode.Default, "OpenTK")
        {
            VSync = VSyncMode.On;

            guiShader = new ShaderProgram();
            Shader guiVertexShader = new Shader("gui.vert", ShaderType.VertexShader);
            Shader guiFragmentShader = new Shader("gui.frag", ShaderType.FragmentShader);
            guiShader.AttachShaders(guiVertexShader, guiFragmentShader);
            guiShader.LinkProgram();

            guiShader.Uniforms["modelMatrix"] = GL.GetUniformLocation(guiShader.ID, "modelMatrix");
            guiShader.Uniforms["guiTexture"] = GL.GetUniformLocation(guiShader.ID, "guiTexture");

            guiShader.AttribLocation["inPosition"] = GL.GetAttribLocation(guiShader.ID, "inPosition");
            
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            scene = new Scene(W, H, Width, Height);

            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            scene.Resize(Width, Height);
            Camera.Instance.Resize(Width, Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            
            Camera.Instance.UpdateView(OpenTK.Input.Mouse.GetState(), OpenTK.Input.Keyboard.GetState());
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Enable(EnableCap.DepthTest);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            scene.Render();

            SwapBuffers();
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