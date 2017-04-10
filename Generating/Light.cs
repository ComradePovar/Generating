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
        public float Angle;
        public float AmbientIntensity;
        public Vector3 Color;
        public Vector3 Direction;

        public Light(float angle)
        {
            Angle = MathHelper.DegreesToRadians(angle);
            LightPos = new Vector3((float)Math.Cos(Angle) * 32.5f + 32.5f, Math.Abs((float)Math.Sin(Angle) * 65), -24.5f);
            Direction = LightPos;
            Direction.Normalize();
            Direction = -Direction;
        }

        public void SetLightPos(float val)
        {
            LightPos = new Vector3(-val, 30f, val);
            Direction = LightPos;
            Direction.Normalize();
            Direction = -Direction;
        }

    }
}
