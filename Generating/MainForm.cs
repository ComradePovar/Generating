using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Input;
using Generating.Shaders;
using Generating.SceneObjects;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


namespace Generating
{
    public partial class MainForm : Form
    {
        private bool isGLControlFocused = true;
        private bool isLoaded = false;
        private Point glControlCenter;
        private Point prevCursorPosition;
        private bool isSceneParamsChanged;
        
        private SceneParameters sceneParameters;

        Matrix4 model = Matrix4.Identity;

        Scene scene;

        public float Roughness { get; set; }
        public float Min { get; set; }
        public float Max { get; set; }
        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InitDefaultSceneParameters();
            scene = new Scene(sceneParameters);
            
            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
            isLoaded = true;
            glControl_Resize(this, null);
        }

        private void glControl_Load(object sender, EventArgs e)
        {
            glControl.MakeCurrent();

            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
        }

        private void glControl_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl.ClientSize.Width, glControl.ClientSize.Height);

            if (isLoaded)
            {
                glControlCenter = new Point(glControl.Width / 2 + glControl.Location.X, glControl.Height / 2 + glControl.Location.Y);
                prevCursorPosition = Cursor.Position;
                scene.Resize(glControl.ClientSize.Width, glControl.ClientSize.Height);
                Camera.Instance.Resize(glControl.ClientSize.Width, glControl.ClientSize.Height);
            }
        }

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            if (isGLControlFocused)
            { 
                Camera.Instance.UpdateView(Mouse.GetState(), Keyboard.GetState());
            }
            glControl.MakeCurrent();

            GL.Enable(EnableCap.DepthTest);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            scene.Render();

            glControl.SwapBuffers();
            glControl.Invalidate();
        }

        private void glControl_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            isGLControlFocused = !isGLControlFocused;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isSceneParamsChanged)
                scene = new Scene(sceneParameters);
            isSceneParamsChanged = false;
        }

        private void InitDefaultSceneParameters()
        {
            sceneParameters = new SceneParameters();
            sceneParameters.TerrainArgs.Width = 1025;
            sceneParameters.TerrainArgs.Height = 1025;
            sceneParameters.TerrainArgs.Scale = 1.0f;

            sceneParameters.TerrainArgs.WindowWidth = Width;
            sceneParameters.TerrainArgs.WindowHeight = Height;

            sceneParameters.TerrainArgs.TerrainGenerationMin = 0.0f;
            sceneParameters.TerrainArgs.TerrainGenerationMax = 10.0f;
            sceneParameters.TerrainArgs.Roughness = 18.0f;

            sceneParameters.TerrainArgs.LightPosition = new Vector3(-70.0f, 100.0f, 70.0f);
            sceneParameters.TerrainArgs.LightColor = new Vector3(1.0f, 1.0f, 1.0f);
            sceneParameters.TerrainArgs.LightAngle = -45.0f;
            sceneParameters.TerrainArgs.AmbientIntensity = 0.25f;
            sceneParameters.TerrainArgs.SpecularIntensity = 1;
            sceneParameters.TerrainArgs.SpecularPower = 2;

            sceneParameters.TerrainArgs.FogColor = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
            sceneParameters.TerrainArgs.FogDensity = 0.001f;
            sceneParameters.TerrainArgs.FogStart = 30.0f;
            sceneParameters.TerrainArgs.FogEnd = 100.0f;
            sceneParameters.TerrainArgs.FogType = FogType.Exp2;

            sceneParameters.TerrainArgs.WaterSpecularIntensity = 0.1f;
            sceneParameters.TerrainArgs.WaterSpecularPower = 2;
            sceneParameters.TerrainArgs.WaterLightColor = new Vector3(1.0f, 1.0f, 1.0f);
            sceneParameters.TerrainArgs.WaterSpecularIntensity = 0.1f;
            sceneParameters.TerrainArgs.WaterSpecularPower = 2;


            sceneParameters.CameraArgs.Eye = Vector3.Zero;
            sceneParameters.CameraArgs.Target = Vector3.UnitZ;
            sceneParameters.CameraArgs.UpVector = Vector3.UnitY;
            sceneParameters.CameraArgs.MovementSpeed = 5.5f;
            sceneParameters.CameraArgs.RotationSpeed = 0.001f;
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            float value = float.Parse(textBox1.Text);
            if (sceneParameters.TerrainArgs.Scale != value)
            {
                isSceneParamsChanged = true;
                sceneParameters.TerrainArgs.Scale = value;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            float value = float.Parse(textBox2.Text);
            if (sceneParameters.TerrainArgs.Roughness != value)
            {
                isSceneParamsChanged = true;
                sceneParameters.TerrainArgs.Roughness = value;
            }
        }
    }
}