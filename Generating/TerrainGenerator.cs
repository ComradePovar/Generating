using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Generating
{
    class TerrainGenerator
    {
        public float Zoom { get; private set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Roughness { get; set; }
        public float Displacement { get; set; }
        public float[,] HeightMap { get; private set; }
        public float[,] NormalizedHeightMap { get; private set; }
        public float[,] NormalHeightMap { get; private set; }
        public float Min { get; set; }
        public float Max { get; set; }

        private static Random rand = new Random();

        public void GenerateHeightMap(int width, int height, float topLeft, float bottomLeft, float bottomRight, float topRight, float roughness, float min, float max)
        {
            Width = width;
            Height = height;
            Roughness = roughness;
            Min = min;
            Max = max;

            HeightMap = new float[Width, Height];

            DiamondSquareAlgorithm(topLeft, bottomLeft, bottomRight, topRight);
        }

        private void DiamondSquareAlgorithm(float topLeft, float bottomLeft, float bottomRight, float topRight)
        {
            int maxIndex = Width - 1;
            float displacement;
            //Init corners
            HeightMap[0, 0] = topLeft;
            HeightMap[maxIndex, 0] = bottomLeft;
            HeightMap[maxIndex, maxIndex] = bottomRight;
            HeightMap[0, maxIndex] = topRight;

            for (int squareSize = maxIndex; squareSize >= 2; squareSize /= 2)
            {
                int stepSize = squareSize / 2;
                displacement = (Max - Min) * Roughness * ((float)squareSize / maxIndex);

                //Squares
                for (int x = 0; x < maxIndex; x += squareSize)
                {
                    for (int y = 0; y < maxIndex; y += squareSize)
                    {
                        HeightMap[x + stepSize, y + stepSize] = GetRandomFloat(-displacement, displacement) +
                                                                (HeightMap[x, y] + HeightMap[x + squareSize, y] + HeightMap[x, y + squareSize] +
                                                                 HeightMap[x + squareSize, y + squareSize]) / 4;
                    }
                }

                //Diamonds
                // if the data should wrap then replace Width with maxIndex and uncomment 2 ifs below
                for (int x = 0; x < Width; x += stepSize)
                {
                    for (int y = (x + stepSize) % squareSize; y < Width; y += squareSize)
                    {
                        HeightMap[x, y] = GetRandomFloat(-displacement, displacement) +
                                          (HeightMap[(x - stepSize + Width) % Width, y] + HeightMap[(x + stepSize) % Width, y] +
                                           HeightMap[x, (y + stepSize) % Width] + HeightMap[x, (y - stepSize + Width) % Width]) / 4;
                        //if (x == 0)
                        //    HeightMap[maxIndex, y] = HeightMap[x, y];
                        //if (y == 0)
                        //    HeightMap[x, maxIndex] = HeightMap[x, y];
                    }
                }
            }
        }

        public void CreateMesh(float zoom, VAO terrain)
        {
            Zoom = zoom;
            Vector3[] vertices = new Vector3[Width * Height];
            uint[] indices = new uint[2 * Width * Height];
            Vector2[] texCoords = new Vector2[Width * Height];
            //Vector3[] colors = new Vector3[Width * Height];
            Vector3[] normals;
            for (int i = 0, ver = 0, ind = 0; i < Width; i++)
            {
                float z = i * zoom;
                for (int j = 0; j < Height; j++)
                {
                    float x = j * zoom;

                    vertices[ver] = new Vector3(x, HeightMap[i, j] * zoom, z);
                    indices[ind++] = (uint)(i * Width + j);
                    if (i != Width - 1)
                        indices[ind++] = (uint)((i + 1) * Width + j);
                    //Add colors
                    //if (NormalizedHeightMap[i, j] < 0.33f)
                    //{
                    //        colors[ver] = new Vector3(0f, 132f / 255, 0f);
                    //}
                    //else if (NormalizedHeightMap[i, j] < 0.66f)
                    //    colors[ver] = new Vector3(132f/255, 132f/255, 132f/255);
                    //else if (Math.Abs(NormalizedHeightMap[(i + 1) % Width, j] - NormalizedHeightMap[i, j]) > 0.03 ||
                    //         Math.Abs(NormalizedHeightMap[i, (j + 1) % Height] - NormalizedHeightMap[i, j]) > 0.03f)
                    //            colors[ver] = new Vector3(89f / 255, 65f / 255, 65f / 255);
                    //     else
                    //            colors[ver] = new Vector3(216f/255, 216f/255, 216f/255);
                    texCoords[ver++] = new Vector2(x * 10 / (Height-1), z * 10/(Width-1));/// (Height - 1), z / (Width - 1)); // , z);
                }

                indices[ind++] = (uint)indices.Length;
            }
            terrain.BindVerticesBuffer(vertices);
            terrain.BindIndicesBuffer(indices);
            terrain.BindTexCoordsBuffer(texCoords);
            //terrain.BindColorsBuffer(colors);
            CreateNormalMap(out normals);
            terrain.BindNormalsBuffer(normals);
        }


        /* Triangles: ABC and BDC
         * A ... B
         * .../...
         * C ... D
         * 
         * Normal E = BDE + BCE + CEF + EFH + DEG + EGH
         * A ... B ... C
         * .............
         * D.....E.....F
         * .............
         * G.....H.....I
         * 
         */
        private void CreateNormalMap(out Vector3[] normals)
        {
            Vector3[,,] triangles = new Vector3[2, Width, Height];
            for (int i = 0; i < Width - 1; i++)
                for (int j = 0; j < Height - 1; j++)
                {
                    Vector3 A = HeightVectorAt(i, j);
                    Vector3 B = HeightVectorAt(i, j + 1);
                    Vector3 C = HeightVectorAt(i + 1, j);
                    Vector3 D = HeightVectorAt(i + 1, j + 1);
                    Vector3 triangleABCNormal = GetTriangleNormal(A - B, B - C);
                    Vector3 triangleBCDNormal = GetTriangleNormal(D - C, C - B);
                    triangles[0, i, j] = triangleABCNormal;
                    triangles[1, i, j] = triangleBCDNormal;
                }

            normals = new Vector3[Width * Height];
            for (int i = 0, nor = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++, nor++)
                {
                    if (i != 0 && j != 0)
                    {
                        normals[nor] += triangles[1, i - 1, j - 1];
                    }
                    if (i != 0 && j != Height - 1)
                    {
                        normals[nor] += triangles[0, i - 1, j];
                        normals[nor] += triangles[1, i - 1, j];
                    }
                    if (i != Width - 1 && j != Height - 1)
                    {
                        normals[nor] += triangles[0, i, j];
                    }
                    if (i != Width - 1 && j != 0)
                    {
                        normals[nor] += triangles[0, i, j - 1];
                        normals[nor] += triangles[1, i, j - 1];
                    }
                    normals[nor].Normalize();
                }
            }
        }

        private Vector3 HeightVectorAt(int i, int j)
        {
            Vector3 heightVector = new Vector3(j * Zoom, HeightMap[i, j], i * Zoom);
            return heightVector;
        }
        
        private Vector3 GetTriangleNormal(Vector3 vector1, Vector3 vector2)
        {
            Vector3 triangleNormal = Vector3.Cross(vector2, vector1);
            triangleNormal.Normalize();
            return triangleNormal;
        }
        private Vector3 GetAverageVector(Vector3 vector1, Vector3 vector2)
        {
            return new Vector3((vector1.X + vector2.X) / 2, (vector1.Y + vector2.Y) / 2, (vector1.Z + vector2.Z) / 2);
        }

        public void NormalizeHeightMap(VAO terrain)
        {
            NormalizedHeightMap = new float[Width, Height];
            float max = float.MinValue, min = float.MaxValue;
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                {
                    if (HeightMap[i, j] > max)
                        max = HeightMap[i, j];
                    if (HeightMap[i, j] < min)
                        min = HeightMap[i, j];
                }

            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    NormalizedHeightMap[i, j] = (HeightMap[i, j] - min) / (max - min);
            terrain.BindNormalizedHeightsBuffer(NormalizedHeightMap);
        }
        private float GetRandomFloat(float min, float max)
        {
            return (float)rand.NextDouble() * (max - min) + min;
        }
        
        private int GetRandomInt(int min, int max)
        {
            return rand.Next(min, max);
        }
    }
}
