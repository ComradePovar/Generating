using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generating.Textures;
using Generating.Shaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Generating
{
    class Skybox
    {
        private CubeMapTexture texture;
        private ShaderProgram skyboxShader;
        private VAO cube;
        private Matrix4 modelMatrix;

        public Skybox(int skyboxSize)
        {
            texture = new CubeMapTexture("skybox");
            modelMatrix = Matrix4.Identity;

            InitShader();
            InitCube(skyboxSize + skyboxSize / 2);
        }

        private void InitCube(int size)
        {
            Vector3[] vertices =
            {
                //back
                new Vector3(-size, size, -size),
                new Vector3(-size, -size, -size),
                new Vector3(size, -size, -size),
                new Vector3(size, -size, -size),
                new Vector3(size, size, - size),
                new Vector3(-size, size, -size),

                //right
                new Vector3(-size, -size, size),
                new Vector3(-size, -size, -size),
                new Vector3(-size, size, -size),
                new Vector3(-size, size, -size),
                new Vector3(-size, size, size),
                new Vector3(-size, -size, size),

                //left
                new Vector3(size, -size, -size),
                new Vector3(size, -size, size),
                new Vector3(size, size, size),
                new Vector3(size, size, size),
                new Vector3(size, size, -size),
                new Vector3(size, -size, -size),

                //front
                new Vector3(-size, -size, size),
                new Vector3(-size, size, size),
                new Vector3(size, size, size),
                new Vector3(size, size, size),
                new Vector3(size, -size, size),
                new Vector3(-size, -size, size),

                //top
                new Vector3(-size, size, -size),
                new Vector3(size, size, -size),
                new Vector3(size, size, size),
                new Vector3(size, size, size),
                new Vector3(-size, size, size),
                new Vector3(-size, size, -size),

                //bottom
                new Vector3(-size, -size, -size),
                new Vector3(-size, -size, size),
                new Vector3(size, -size, size),
                new Vector3(size, -size, size),
                new Vector3(size, -size, -size),
                new Vector3(-size, -size, -size)
            };
            cube = new VAO();
            cube.BindVerticesBuffer(vertices);
        }
        private void InitShader()
        {
            skyboxShader = new ShaderProgram();
            Shader vertex = new Shader("skybox.vert", ShaderType.VertexShader);
            Shader fragment = new Shader("skybox.frag", ShaderType.FragmentShader);
            skyboxShader.AttachShaders(vertex, fragment);
            skyboxShader.LinkProgram();

            skyboxShader.AttribLocation["inPosition"] = GL.GetAttribLocation(skyboxShader.ID, "inPosition");

            skyboxShader.Uniforms["projectionMatrix"] = GL.GetUniformLocation(skyboxShader.ID, "projectionMatrix");
            skyboxShader.Uniforms["viewMatrix"] = GL.GetUniformLocation(skyboxShader.ID, "viewMatrix");
            skyboxShader.Uniforms["modelMatrix"] = GL.GetUniformLocation(skyboxShader.ID, "modelMatrix");
            skyboxShader.Uniforms["skyboxTexture"] = GL.GetUniformLocation(skyboxShader.ID, "skyboxTexture");
        }

        public void Render(Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            viewMatrix.M14 = viewMatrix.M14 = viewMatrix.M14 = 0;
            modelMatrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(0.005f));

            skyboxShader.Start();
            GL.Enable(EnableCap.TextureCubeMap);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.BindVertexArray(cube.ID);

            GL.BindBuffer(BufferTarget.ArrayBuffer, cube.VerticesBuffer);
            GL.EnableVertexAttribArray(skyboxShader.AttribLocation["inPosition"]);
            GL.VertexAttribPointer(skyboxShader.AttribLocation["inPosition"], 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, texture.ID);

            GL.UniformMatrix4(skyboxShader.Uniforms["projectionMatrix"], false, ref projectionMatrix);
            GL.UniformMatrix4(skyboxShader.Uniforms["viewMatrix"], false, ref viewMatrix);
            GL.UniformMatrix4(skyboxShader.Uniforms["modelMatrix"], false, ref modelMatrix);
            GL.Uniform1(skyboxShader.Uniforms["skyboxTexture"], 0);

            GL.DrawArrays(BeginMode.Triangles, 0, cube.VerticesCount);

            GL.DisableVertexAttribArray(skyboxShader.AttribLocation["inPosition"]);
            GL.BindTexture(TextureTarget.TextureCubeMap, 0);
            GL.BindVertexArray(0);
            skyboxShader.Stop();
        }
    }
}
