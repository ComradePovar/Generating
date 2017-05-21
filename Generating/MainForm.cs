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
        private Vector3 lightPos;
        
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
                scene.Resize(glControl.ClientSize.Width, glControl.ClientSize.Height);
                Camera.Instance.Resize(glControl.ClientSize.Width, glControl.ClientSize.Height);
                sceneParameters.TerrainArgs.WindowWidth = glControl.ClientSize.Width;
                sceneParameters.TerrainArgs.WindowHeight = glControl.ClientSize.Height;
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

        private void InitDefaultSceneParameters()
        {
            sceneParameters = new SceneParameters();

            sceneParameters.TerrainArgs.Size = 1025;
            sceneParameters.TerrainArgs.Scale = 1.0f;
            tbSize.Text = "1025";
            tbScale.Text = "1.0";

            sceneParameters.TerrainArgs.WindowWidth = glControl.ClientSize.Width;
            sceneParameters.TerrainArgs.WindowHeight = glControl.ClientSize.Height;

            sceneParameters.TerrainArgs.TerrainGenerationMin = 0.0f;
            sceneParameters.TerrainArgs.TerrainGenerationMax = 10.0f;
            sceneParameters.TerrainArgs.Roughness = 18.0f;
            tbRoughness.Text = "18.0";


            sceneParameters.TerrainArgs.LightPosition = new Vector3(-70.0f, 100.0f, 70.0f);
            sceneParameters.TerrainArgs.LightColor = new Vector3(1.0f, 1.0f, 1.0f);
            sceneParameters.TerrainArgs.AmbientIntensity = 0.25f;
            sceneParameters.TerrainArgs.SpecularIntensity = 1;
            sceneParameters.TerrainArgs.SpecularPower = 2;
            tbLightX.Text = "-70.0";
            tbLightY.Text = "100.0";
            tbLightZ.Text = "70.0";
            lightPos = sceneParameters.TerrainArgs.LightPosition;
            tbLightAmbientIntensity.Text = "0.25";
            tbLightSpecIntensity.Text = "1.0";
            tbLightSpecPower.Text = "2.0";

            sceneParameters.TerrainArgs.FogColor = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
            sceneParameters.TerrainArgs.FogDensity = 0.001f;
            sceneParameters.TerrainArgs.FogStart = 30.0f;
            sceneParameters.TerrainArgs.FogEnd = 100.0f;
            sceneParameters.TerrainArgs.FogType = FogType.Exp2;
            tbFogDensity.Text = "0.001";
            tbFogStart.Text = "30.0";
            tbFogEnd.Text = "100.0";
            cbFogType.Text = "Exp^2";

            sceneParameters.TerrainArgs.WaterSpecularIntensity = 0.1f;
            sceneParameters.TerrainArgs.WaterSpecularPower = 2;
            sceneParameters.TerrainArgs.WaterLightColor = new Vector3(1.0f, 1.0f, 1.0f);
            sceneParameters.TerrainArgs.WaterWaveStrength = 0.02f;
            sceneParameters.TerrainArgs.WaterSpeed = 0.0003f;
            tbWaterSpecIntensity.Text = "0.1";
            tbWaterSpecPower.Text = "2.0";
            tbWaveStrength.Text = "0.02";
            tbWaterSpeed.Text = "0.0003";


            sceneParameters.CameraArgs.Eye = Vector3.Zero;
            sceneParameters.CameraArgs.Target = Vector3.UnitZ;
            sceneParameters.CameraArgs.UpVector = Vector3.UnitY;
            sceneParameters.CameraArgs.MovementSpeed = 5.5f;
            sceneParameters.CameraArgs.RotationSpeed = 0.001f;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            scene = new Scene(sceneParameters);
        }

        private void tbSize_Leave(object sender, EventArgs e)
        {
            if (int.TryParse(tbSize.Text, out int value))
            {
                if (sceneParameters.TerrainArgs.Size != value)
                {
                    sceneParameters.TerrainArgs.Size = value;
                }
            }
        }

        private void tbScale_Leave(object sender, EventArgs e)
        {
            if (float.TryParse(tbScale.Text, out float value))
            {
                if (sceneParameters.TerrainArgs.Scale != value)
                {
                    sceneParameters.TerrainArgs.Scale = value;
                }
            }
        }

        private void tbRoughness_Leave(object sender, EventArgs e)
        {
            if (float.TryParse(tbRoughness.Text, out float value))
            {
                if (sceneParameters.TerrainArgs.Roughness != value)
                {
                    sceneParameters.TerrainArgs.Roughness = value;
                }
            }
        }

        private void tbFogDensity_Leave(object sender, EventArgs e)
        {
            if (float.TryParse(tbFogDensity.Text, out float value))
            {
                if (sceneParameters.TerrainArgs.FogDensity != value)
                {
                    sceneParameters.TerrainArgs.FogDensity = value;
                    scene.Terrain.SetFog(sceneParameters.TerrainArgs);
                }
            }
        }

        private void tbFogStart_Leave(object sender, EventArgs e)
        {
            if (float.TryParse(tbFogStart.Text, out float value))
            {
                if (sceneParameters.TerrainArgs.FogStart != value)
                {
                    sceneParameters.TerrainArgs.FogStart = value;
                    scene.Terrain.SetFog(sceneParameters.TerrainArgs);
                }
            }
        }

        private void tbFogEnd_Leave(object sender, EventArgs e)
        {
            if (float.TryParse(tbFogEnd.Text, out float value))
            {
                if (sceneParameters.TerrainArgs.FogEnd != value)
                {
                    sceneParameters.TerrainArgs.FogEnd = value;
                    scene.Terrain.SetFog(sceneParameters.TerrainArgs);
                }
            }
        }

        private void cbFogType_Leave(object sender, EventArgs e)
        {
            FogType value = (FogType)cbFogType.SelectedIndex;
            if ((FogType)cbFogType.SelectedIndex != sceneParameters.TerrainArgs.FogType)
            {
                sceneParameters.TerrainArgs.FogType = value;
                scene.Terrain.SetFog(sceneParameters.TerrainArgs);
            }
        }

        private void tbWaterSpecIntensity_TextChanged(object sender, EventArgs e)
        {
            if (float.TryParse(tbWaterSpecIntensity.Text, out float value))
            {
                if (sceneParameters.TerrainArgs.WaterSpecularIntensity != value){
                    sceneParameters.TerrainArgs.WaterSpecularIntensity = value;
                    scene.Terrain.SetWaterArgs(sceneParameters.TerrainArgs);
                }
            }
        }

        private void tbWaterSpecPower_TextChanged(object sender, EventArgs e)
        {
            if (float.TryParse(tbWaterSpecPower.Text, out float value))
            {
                if (sceneParameters.TerrainArgs.WaterSpecularPower != value)
                {
                    sceneParameters.TerrainArgs.WaterSpecularPower = value;
                    scene.Terrain.SetWaterArgs(sceneParameters.TerrainArgs);
                }
            }
        }

        private void tbWaterSpeed_TextChanged(object sender, EventArgs e)
        {
            if (float.TryParse(tbWaterSpeed.Text, out float value))
            {
                if (sceneParameters.TerrainArgs.WaterSpeed != value)
                {
                    sceneParameters.TerrainArgs.WaterSpeed = value;
                    scene.Terrain.SetWaterArgs(sceneParameters.TerrainArgs);
                }
            }
        }

        private void tbWaveStrength_TextChanged(object sender, EventArgs e)
        {
            if (float.TryParse(tbWaveStrength.Text, out float value))
            {
                if (sceneParameters.TerrainArgs.WaterWaveStrength != value)
                {
                    sceneParameters.TerrainArgs.WaterWaveStrength = value;
                    scene.Terrain.SetWaterArgs(sceneParameters.TerrainArgs);
                }
            }
        }

        private void tbLightAmbientIntensity_TextChanged(object sender, EventArgs e)
        {
            if (float.TryParse(tbLightAmbientIntensity.Text, out float value))
            {
                if (sceneParameters.TerrainArgs.AmbientIntensity != value)
                {
                    sceneParameters.TerrainArgs.AmbientIntensity = value;
                    scene.Terrain.SetLight(sceneParameters.TerrainArgs);
                }
            }
        }

        private void tbLightSpecIntensity_TextChanged(object sender, EventArgs e)
        {
            if (float.TryParse(tbLightSpecIntensity.Text, out float value))
            {
                if (sceneParameters.TerrainArgs.SpecularIntensity != value)
                {
                    sceneParameters.TerrainArgs.SpecularIntensity = value;
                    scene.Terrain.SetLight(sceneParameters.TerrainArgs);
                }
            }
        }

        private void tbLightSpecPower_TextChanged(object sender, EventArgs e)
        {
            if (float.TryParse(tbLightSpecPower.Text, out float value))
            {
                if (sceneParameters.TerrainArgs.SpecularPower != value)
                {
                    sceneParameters.TerrainArgs.SpecularPower = value;
                    scene.Terrain.SetLight(sceneParameters.TerrainArgs);
                }
            }
        }

        private void tbLightX_TextChanged(object sender, EventArgs e)
        {
            if (float.TryParse(tbLightX.Text, out float value))
            {
                if (sceneParameters.TerrainArgs.LightPosition.X != value)
                {
                    lightPos.X = value;
                    sceneParameters.TerrainArgs.LightPosition = lightPos;
                    scene.Terrain.SetLight(sceneParameters.TerrainArgs);
                }
            }
        }

        private void tbLightY_TextChanged(object sender, EventArgs e)
        {
            if (float.TryParse(tbLightY.Text, out float value))
            {
                if (sceneParameters.TerrainArgs.LightPosition.Y != value)
                {
                    lightPos.Y = value;
                    sceneParameters.TerrainArgs.LightPosition = lightPos;
                    scene.Terrain.SetLight(sceneParameters.TerrainArgs);
                }
            }
        }

        private void tbLightZ_TextChanged(object sender, EventArgs e)
        {
            if (float.TryParse(tbLightZ.Text, out float value))
            {
                if (sceneParameters.TerrainArgs.LightPosition.Z != value)
                {
                    lightPos.Z = value;
                    sceneParameters.TerrainArgs.LightPosition = lightPos;
                    scene.Terrain.SetLight(sceneParameters.TerrainArgs);
                }
            }
        }
        
    }
}