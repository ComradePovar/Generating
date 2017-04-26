using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generating.Textures
{
    abstract class Texture
    {
        public int ID { get; protected set; }
        public int SamplerID { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public Texture() { }
        
        public Texture(string textureName)
        {
            LoadTexture(textureName);
        }

        public abstract void LoadTexture(string textureName);
    }
}
