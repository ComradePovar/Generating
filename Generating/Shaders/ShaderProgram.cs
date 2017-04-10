using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Generating.Shaders
{
    class ShaderProgram : IDisposable
    {
        public int ID { get; private set; }
        public List<int> shadersID;
        public bool IsActive { get; private set; }

        public ShaderProgram()
        {
            ID = GL.CreateProgram();
            shadersID = new List<int>();
        }

        public void AttachShaders(params Shader[] shaders)
        {
            foreach (Shader shader in shaders)
            {
                GL.AttachShader(ID, shader.ID);
                shadersID.Add(shader.ID);
            }
        }
        
        public void DetachShader(params Shader[] shaders)
        {
            foreach (Shader shader in shaders)
            {
                GL.DetachShader(ID, shader.ID);
                shadersID.Remove(shader.ID);
            }
        }

        public void LinkProgram()
        {
            GL.LinkProgram(ID);
            int linkStatus = 0;
            GL.GetProgram(ID, ProgramParameter.LinkStatus, out linkStatus);
            if (linkStatus == 0)
            {
                throw new Exception(GL.GetProgramInfoLog(ID)); //TODO: create custom exception
            }
        }

        public void Start()
        {
            GL.UseProgram(ID);
            IsActive = true;
        }

        public void Stop()
        {
            GL.UseProgram(0);
            IsActive = false;
        }

        public void Dispose()
        {
            foreach(int shaderId in shadersID)
            {
                GL.DetachShader(ID, shaderId);
            }

            if (IsActive)
                Stop();
            GL.DeleteProgram(ID);
        }
    }
}
