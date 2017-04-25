using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Generating.GUI
{
    class Gui
    {
        public VAO Vao { get; private set; }
        public int TextureID { get; private set; }
        public Matrix4 ModelMatrix;

        public Gui(int textureID, Vector3 position, Vector3 scale)
        {
            Vector3[] guiVertices = new Vector3[]
            {
                new Vector3(-1.0f, 1.0f, 0.0f),
                new Vector3(-1.0f, -1.0f, 0.0f),
                new Vector3(1.0f, 1.0f, 0.0f),
                new Vector3(1.0f, -1.0f, 0.0f)
            };
            Vao = new VAO();
            TextureID = textureID;
            Vao.BindVerticesBuffer(guiVertices);
            ModelMatrix = Matrix4.CreateScale(scale);
            ModelMatrix *= Matrix4.CreateTranslation(position);
        }
    }
}
