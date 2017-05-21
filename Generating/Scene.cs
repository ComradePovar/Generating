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
        public Terrain Terrain { get; }
        public Skybox Skybox { get; }

        public Scene(SceneParameters args)
        {
            Camera.InitCamera(args.CameraArgs);
            Terrain = new Terrain(args.TerrainArgs, this);
            Skybox = new Skybox((int)(args.TerrainArgs.Size * args.TerrainArgs.Scale));

            renderableObjects = new List<IRenderable>(new List<IRenderable>() { Terrain, Skybox });
            resizableObjects = new List<IResizable>(new List<IResizable>() { Terrain });
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
