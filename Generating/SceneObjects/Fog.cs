using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Generating.SceneObjects
{
    enum FogType { Linear, Exp, Exp2 }
    class Fog
    {
        public Vector4 Color;
        public float Density;
        public float Start;
        public float End;
        public FogType Type;
    }
}
