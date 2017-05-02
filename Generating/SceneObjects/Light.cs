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
        public Vector3 LightPos;
        public float AmbientIntensity;
        public Vector3 Color;
        public Vector3 Direction;
        public float SpecularIntensity;
        public float SpecularPower;

        public Light(Vector3 lightPos, float angle)
        {
            LightPos = lightPos;
            Direction = LightPos;
            Direction.Normalize();
            Direction = -Direction;
        }

    }
}
