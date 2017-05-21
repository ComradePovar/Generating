using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Generating.SceneObjects
{
    class Light
    {
        private Vector3 lightPos;
        public Vector3 LightPos
        {
            get
            {
                return lightPos;
            }
            set
            {
                lightPos = value;
                Direction = -lightPos.Normalized();
            }
        }
        public float AmbientIntensity;
        public Vector3 Color;
        public Vector3 Direction;
        public float SpecularIntensity;
        public float SpecularPower;

        public Light(Vector3 lightPos)
        {
            this.lightPos = lightPos;
            Direction = -this.lightPos.Normalized();
        }

    }
}
