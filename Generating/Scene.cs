using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generating.Interfaces;
using Generating.SceneObjects;
using OpenTK.Graphics.OpenGL;

namespace Generating
{
    class Scene : IRenderable, IResizable
    {
        public List<IRenderable> renderableObjects { get; }
        public List<IResizable> resizableObjects { get; }

        public Scene(int width, int height, int windowWidth, int windowHeight)
        {
            Terrain terrain = new Terrain(width, height, windowWidth, windowHeight, this);
            Skybox skybox = new Skybox(width);

            renderableObjects = new List<IRenderable>(new List<IRenderable>() { terrain, skybox });
            resizableObjects = new List<IResizable>(new List<IResizable>() { terrain });
        }

        public void Render()
        {
            foreach (IRenderable obj in renderableObjects)
                obj.Render();
        }

        public void Resize(int width, int height)
        {
            foreach (IResizable obj in resizableObjects)
                obj.Resize(width, height);
        }
    }
}
