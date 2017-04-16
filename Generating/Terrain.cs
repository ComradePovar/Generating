using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generating
{
    class Terrain
    {
        public float[,] HeightMap { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public VAO VerticesData { get; set; } = new VAO();
    }
}
