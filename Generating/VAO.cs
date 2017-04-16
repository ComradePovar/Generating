using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Generating
{
    class VAO
    {
        public int ID { get; set; }
        public int VerticesBuffer { get; private set; } = -1;
        public int IndicesBuffer { get; private set; } = -1;
        public int TexCoordsBuffer { get; private set; } = -1;
        public int NormalsBuffer { get; private set; } = -1;
        public int ColorsBuffer { get; private set; } = -1;

        public int IndicesCount;

        public VAO()
        {
            ID = GL.GenVertexArray();
        }

        public void BindVerticesBuffer(Vector3[] vertices)
        {
            VerticesBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VerticesBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * Vector3.SizeInBytes), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void BindIndicesBuffer(uint[] indices)
        {
            IndicesBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndicesBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(uint)), indices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            IndicesCount = indices.Length;
        }


        public void BindTexCoordsBuffer(Vector2[] texCoords)
        {

        }

        public void BindNormalsBuffer(Vector3[] normals)
        {
            NormalsBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, NormalsBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(normals.Length * Vector3.SizeInBytes), normals, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void BindColorsBuffer(Vector3[] colors)
        {
            ColorsBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, ColorsBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(colors.Length * Vector3.SizeInBytes), colors, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}