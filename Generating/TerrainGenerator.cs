using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Drawing.Imaging;
using System.Drawing;
using OpenTK.Graphics;

namespace Generating
{
    class TerrainGenerator
    {
        private static TerrainGenerator instance;
        public static TerrainGenerator Instance
        {
            get
            {
                if (instance == null)
                    instance = new TerrainGenerator();
                return instance;
            }
        }

        private static Random rand = new Random();

        private TerrainGenerator() { }
        public void GenerateIsland(VAO terrain, int width, int height, float roughness, float min, float max, float zoom, out float waterHeight)
        {
            float[,] heightMap = DiamondSquareAlgorithm(0, 0, 0, 0, width, height, roughness, min, max);
            float[,] normalizedHeightMap = GetNormalizedArray(heightMap);

            float waterUpperBound = 0.18f;
            CreateIslandLandscape(heightMap, normalizedHeightMap, waterUpperBound, out waterHeight);
            float[,] moistureMap = CreateMoistureMap(width, height, roughness, min, max, normalizedHeightMap);

            CreateMesh(zoom, heightMap, terrain);
            Vector3[] normalMap = CreateNormalMap(zoom, heightMap);
            terrain.BindNormalizedHeightsBuffer(normalizedHeightMap);
            terrain.BindNormalsBuffer(normalMap);
            terrain.BindMoisturesBuffer(moistureMap);
        }

        private void CreateIslandLandscape(float[,] heightMap, float[,] normalizedHeightMap, float waterUpperBound, out float waterHeight)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);
            Vector2 center = new Vector2(width / 2, height / 2);
            float maxDistanceToCenter = (float)Math.Sqrt(Math.Pow(center.X, 2) + Math.Pow(center.Y, 2));
            float islandRadius = (float)width / 2;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    //круг
                    //float distanceToIsland = (float)Math.Sqrt(Math.Pow(i - islandRadius, 2) + Math.Pow(j - islandRadius, 2));
                    //float distanceToCenter = (float)Math.Sqrt(Math.Pow(i - center.X, 2) + Math.Pow(j - center.Y, 2));
                    //float maxWidth = islandRadius;
                    //float delta = distanceToIsland / maxWidth;
                    //float influence = delta * delta;
                    //influence = influence > 1f ? 0f : 1.0f - influence;
                    //normalizedHeightMap[i, j] *= influence;
                    //heightMap[i, j] *= influence;

                    //квадрат
                    float distanceToIsland = (float)Math.Max(Math.Abs(i - islandRadius), Math.Abs(j - islandRadius));
                    float maxWidth = islandRadius - 10;
                    float delta = distanceToIsland / maxWidth;
                    float influence = delta * delta;
                    influence = influence > 1.0f ? 0.0f : 1.0f - influence;
                    normalizedHeightMap[i, j] *= influence;
                    heightMap[i, j] *= influence;

                    //дефолт
                }
            }

            float max = float.MinValue;
            float min = float.MaxValue;
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    if (heightMap[i, j] > max)
                        max = heightMap[i, j];
                    if (heightMap[i, j] < min)
                        min = heightMap[i, j];
                }
            waterHeight = waterUpperBound * (max - min) + min;
        }

        //void Save(string name, float[,] array, Color4 c)
        //{
        //    Bitmap bitmap = new Bitmap(Width, Height);
        //    for (int i = 0; i < Width; i++)
        //        for (int j = 0; j < Height; j++)
        //        {
        //            Color4 color = new Color4(array[i, j], array[i, j], array[i, j], 1);
        //            color.R *= c.R;
        //            color.G *= c.G;
        //            color.B *= c.B;
        //            bitmap.SetPixel(i, j, (Color)color);
        //        }
        //    bitmap.Save("Assets/" + name, System.Drawing.Imaging.ImageFormat.Jpeg);
        //}

        private float[,] CreateMoistureMap(int width, int height, float roughness, float min, float max, float[,] normalizedHeightMap)
        {
            float[,] moistureMap = DiamondSquareAlgorithm(0, 0, 0, 0, width, height, roughness, min, max);
            moistureMap = GetNormalizedArray(moistureMap);
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    moistureMap[i, j] *= (normalizedHeightMap[i, j]) == 0.0f ? 0.0f : (1.0f - normalizedHeightMap[i, j]);
                }
            return moistureMap;
        }

        private float[,] GetNormalizedArray(float[,] array)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            float max = float.MinValue;
            float min = float.MaxValue;
            float[,] normalizedArray = new float[array.GetLength(0), array.GetLength(1)];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    if (array[i, j] > max)
                        max = array[i, j];
                    if (array[i, j] < min)
                        min = array[i, j];
                }

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    normalizedArray[i, j] = (array[i, j] - min) / (max - min);
            return normalizedArray;
        }

        private float[,] DiamondSquareAlgorithm(float topLeft, float bottomLeft, float bottomRight, float topRight, int width, int height, float roughness, float min, float max)
        {
            float[,] heightMap = new float[width, height];
            int maxIndex = width - 1;
            float displacement;
            //Init corners
            heightMap[0, 0] = topLeft;
            heightMap[maxIndex, 0] = bottomLeft;
            heightMap[maxIndex, maxIndex] = bottomRight;
            heightMap[0, maxIndex] = topRight;

            for (int squareSize = maxIndex; squareSize >= 2; squareSize /= 2)
            {
                int stepSize = squareSize / 2;
                displacement = (max - min) * roughness * ((float)squareSize / maxIndex);

                //Squares
                for (int x = 0; x < maxIndex; x += squareSize)
                {
                    for (int y = 0; y < maxIndex; y += squareSize)
                    {
                        heightMap[x + stepSize, y + stepSize] = Math.Abs(GetRandomFloat(-displacement, displacement) +
                                                                (heightMap[x, y] + heightMap[x + squareSize, y] + heightMap[x, y + squareSize] +
                                                                 heightMap[x + squareSize, y + squareSize]) / 4);
                    }
                }

                //Diamonds
                // if the data should wrap then replace Width with maxIndex and uncomment 2 ifs below
                for (int x = 0; x < width; x += stepSize)
                {
                    for (int y = (x + stepSize) % squareSize; y < width; y += squareSize)
                    {
                        heightMap[x, y] = Math.Abs(GetRandomFloat(-displacement, displacement) +
                                          (heightMap[(x - stepSize + width) % width, y] + heightMap[(x + stepSize) % width, y] +
                                           heightMap[x, (y + stepSize) % width] + heightMap[x, (y - stepSize + width) % width]) / 4);
                        //if (x == 0)
                        //    HeightMap[maxIndex, y] = HeightMap[x, y];
                        //if (y == 0)
                        //    HeightMap[x, maxIndex] = HeightMap[x, y];
                    }
                }
            }

            return heightMap;
        }

        private void CreateMesh(float zoom, float[,] heightMap, VAO terrain)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);
            Vector3[] vertices = new Vector3[width * height];
            uint[] indices = new uint[2 * width * height];
            Vector2[] texCoords = new Vector2[width * height];
            for (int i = 0, ver = 0, ind = 0; i < width; i++)
            {
                float z = i * zoom;
                for (int j = 0; j < height; j++)
                {
                    float x = j * zoom;

                    vertices[ver] = new Vector3(x, heightMap[i, j] * zoom, z);
                    indices[ind++] = (uint)(i * width + j);
                    if (i != width - 1)
                        indices[ind++] = (uint)((i + 1) * width + j);
                    texCoords[ver++] = new Vector2(x * 15 / (height - 1), z * 15 / (width - 1)); // 15 * for 513x513 map
                }

                indices[ind++] = (uint)indices.Length;
            }

            terrain.BindVerticesBuffer(vertices);
            terrain.BindIndicesBuffer(indices);
            terrain.BindTexCoordsBuffer(texCoords);
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
        private Vector3[] CreateNormalMap(float zoom, float[,] heightMap)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);
            Vector3[,,] triangles = new Vector3[2, width, height];
            for (int i = 0; i < width - 1; i++)
                for (int j = 0; j < height - 1; j++)
                {
                    Vector3 A = HeightVectorAt(i, j, zoom, heightMap);
                    Vector3 B = HeightVectorAt(i, j + 1, zoom, heightMap);
                    Vector3 C = HeightVectorAt(i + 1, j, zoom, heightMap);
                    Vector3 D = HeightVectorAt(i + 1, j + 1, zoom, heightMap);
                    Vector3 triangleABCNormal = GetTriangleNormal(A - B, B - C);
                    Vector3 triangleBCDNormal = GetTriangleNormal(D - C, C - B);
                    triangles[0, i, j] = triangleABCNormal;
                    triangles[1, i, j] = triangleBCDNormal;
                }

            Vector3[] normals = new Vector3[width * height];
            for (int i = 0, nor = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++, nor++)
                {
                    if (i != 0 && j != 0)
                    {
                        normals[nor] += triangles[1, i - 1, j - 1];
                    }
                    if (i != 0 && j != height - 1)
                    {
                        normals[nor] += triangles[0, i - 1, j];
                        normals[nor] += triangles[1, i - 1, j];
                    }
                    if (i != width - 1 && j != height - 1)
                    {
                        normals[nor] += triangles[0, i, j];
                    }
                    if (i != width - 1 && j != 0)
                    {
                        normals[nor] += triangles[0, i, j - 1];
                        normals[nor] += triangles[1, i, j - 1];
                    }
                    normals[nor].Normalize();
                }
            }

            return normals;
        }

        private Vector3 HeightVectorAt(int i, int j, float zoom, float[,] heightMap)
        {
            return new Vector3(j * zoom, heightMap[i, j], i * zoom);
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
