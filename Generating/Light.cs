using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Generating
{
    class Light
    {
        public Vector3 LightPos;
        public float AmbientIntensity;
        public Vector3 Color;
        public Vector3 Direction;

        public Light(float angle)
        {
            LightPos = new Vector3(-30, 30, 30);
            Direction = LightPos;
            Direction.Normalize();
            Direction = -Direction;
        }

    }
}
