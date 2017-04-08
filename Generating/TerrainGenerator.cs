﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Generating
{
    class TerrainGenerator
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public float Roughness { get; set; }
        public float Displacement { get; set; }
        public float[,] HeightMap { get; private set; }
        public float[,] NormalizedHeightMap { get; private set; }
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
            Vector3[] vertices = new Vector3[Width * Height];
            uint[] indices = new uint[2 * Width * Height];
            for (int i = 0, ver = 0, ind = 0; i < Width; i++)
            {
                float z = i * zoom;
                for (int j = 0; j < Height; j++)
                {
                    float x = j * zoom;

                    vertices[ver++] = new Vector3(x, HeightMap[i, j] * zoom, z);
                    indices[ind++] = (uint)(i * Width + j);
                    if (i != Width - 1)
                        indices[ind++] = (uint)((i + 1) * Width + j);
                }

                indices[ind++] = (uint)indices.Length;
            }
            terrain.BindVerticesBuffer(vertices);
            terrain.BindIndicesBuffer(indices);
        }

        public void NormalizeHeightMap()
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
