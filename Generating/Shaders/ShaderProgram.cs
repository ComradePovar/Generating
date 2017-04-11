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
        public Dictionary<int, Shader> Shaders { get; }
        public Dictionary<string, int> Uniforms { get; }
        public Dictionary<string, int> AttribLocation { get; }
        public bool IsActive { get; private set; }

        public Shader this[int id]
        {
            get
            {
                return Shaders[id];
            }
            set
            {
                Shaders[id] = value;
            }
        }

        public ShaderProgram()
        {
            ID = GL.CreateProgram();
            Shaders = new Dictionary<int, Shader>();
            Uniforms = new Dictionary<string, int>();
            AttribLocation = new Dictionary<string, int>();
        }

        public void AttachShaders(params Shader[] shaders)
        {
            foreach (Shader shader in shaders)
            {
                GL.AttachShader(ID, shader.ID);
                this.Shaders.Add(shader.ID, shader);
                foreach (KeyValuePair<string, int> pair in shader.Uniforms)
                    this.Uniforms.Add(pair.Key, pair.Value);
                foreach (KeyValuePair<string, int> pair in shader.AttribLocation)
                    this.AttribLocation[pair.Key] = pair.Value;
            }
        }
        
        public void DetachShaders(params Shader[] shaders)
        {
            foreach (Shader shader in shaders)
            {
                GL.DetachShader(ID, shader.ID);
                this.Shaders.Remove(shader.ID);
                foreach (string attribute in shader.Uniforms.Keys)
                    this.Uniforms.Remove(attribute);
                foreach (string attribute in shader.AttribLocation.Keys)
                    this.AttribLocation.Remove(attribute);
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
            foreach(int shaderId in Shaders.Keys)
            {
                GL.DetachShader(ID, shaderId);
            }

            if (IsActive)
                Stop();
            GL.DeleteProgram(ID);
        }
    }
}
