using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Generating
{
    class ShaderProgram
    {
        public int ID;
        public int VertexShader;
        public int FragmentShader;

        public ShaderProgram()
        {
            ID = GL.CreateProgram();
            AddShader("vertex.txt", "fragment.txt");

            GL.AttachShader(ID, VertexShader);
            GL.AttachShader(ID, FragmentShader);
            GL.LinkProgram(ID);
        }

        public void AddShader(string vertexShaderPath, string fragmentShaderPath)
        {
            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            string sourceVertex = System.IO.File.ReadAllText(vertexShaderPath);
            string sourceFragment = System.IO.File.ReadAllText(fragmentShaderPath);
            GL.ShaderSource(VertexShader, sourceVertex);
            GL.ShaderSource(FragmentShader, sourceFragment);

            GL.CompileShader(VertexShader);
            GL.CompileShader(FragmentShader);
            int compileStatusVertex = 0;
            int compileStatusFragment = 0;
            GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out compileStatusVertex);
            GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out compileStatusFragment);
            if (compileStatusFragment != 1 && compileStatusVertex != 1)
            {
                Console.WriteLine(GL.GetShaderInfoLog(VertexShader));
                Console.WriteLine(GL.GetShaderInfoLog(FragmentShader));
                throw new Exception();
            }
        }

        public void UseProgram()
        {
            GL.UseProgram(ID);
        }
    }
    class Shader
    {

    }
}
