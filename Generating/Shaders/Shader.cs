using System.Reflection;
using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Generating.Shaders
{
    class Shader : IDisposable
    {
        public int ID { get; private set; }
        public ShaderType Type { get; private set; }

        public Shader(string fileName, ShaderType type)
        {
            this.Type = type;
            string source;
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Generating.Shaders." + fileName))
            using (StreamReader reader = new StreamReader(stream))
            {
                source = reader.ReadToEnd();
            }

            ID = GL.CreateShader(type);
            GL.ShaderSource(ID, source);
            GL.CompileShader(ID);
            int compileStatus;
            GL.GetShader(ID, ShaderParameter.CompileStatus, out compileStatus);
            if (compileStatus == 0)
            {
                throw new Exception(GL.GetShaderInfoLog(ID)); //TODO: create custom exception
            }
        }

        public void Dispose()
        {
            GL.DeleteShader(ID);
        }
    }
}