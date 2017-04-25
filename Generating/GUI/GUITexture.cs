using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Generating.GUI
{
    class GuiTexture
    {
        public int TextureID { get; private set; }
        public Vector2 Position { get; private set; }
        public Vector2 Scale { get; private set; }

        public GuiTexture(int textureID, Vector2 position, Vector2 scale)
        {
            TextureID = textureID;
            Position = position;
            Scale = scale;
        }
    }
}
