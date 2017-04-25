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
        public int NormalizedHeightsBuffer { get; private set; } = -1;
        public int MoisturesBuffer { get; private set; } = -1;
        public float WaterHeight;

        public int IndicesCount;

        public VAO()
        {
            ID = GL.GenVertexArray();
        }

        public void BindMoisturesBuffer(float[,] moistureMap)
        {
            float[] moistures = new float[moistureMap.GetLength(0) * moistureMap.GetLength(1)];
            for (int i = 0; i < moistureMap.GetLength(0); i++)
                for (int j = 0; j < moistureMap.GetLength(1); j++)
                    moistures[i * moistureMap.GetLength(0) + j] = moistureMap[i, j];
            MoisturesBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, MoisturesBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(moistures.Length * sizeof(float)), moistures, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
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
            TexCoordsBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, TexCoordsBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(texCoords.Length * Vector2.SizeInBytes), texCoords, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
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

        public void BindNormalizedHeightsBuffer(float[,] normalizedHeights)
        {
            float[] heights = new float[normalizedHeights.GetLength(0) * normalizedHeights.GetLength(1)];
            for (int i = 0; i < normalizedHeights.GetLength(0); i++)
                for (int j = 0; j < normalizedHeights.GetLength(1); j++)
                    heights[i * normalizedHeights.GetLength(0) + j] = normalizedHeights[i, j];
            NormalizedHeightsBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, NormalizedHeightsBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(heights.Length * sizeof(float)), heights, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
