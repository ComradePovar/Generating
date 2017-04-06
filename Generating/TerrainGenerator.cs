using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Generating
{
    public class TerrainGenerator
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public float Roughness { get; set; }
        public float Displacement { get; set; }
        public float[,] HeightMap { get; private set; }

        private static Random rand = new Random();

        public void GenerateHeightMap(int width, int height, float topLeft, float bottomLeft, float bottomRight, float topRight, float roughness)
        {
            Width = width;
            Height = height;
            Roughness = roughness;

            HeightMap = new float[Width, Height];

            DiamondSquareAlgorithm(topLeft, bottomLeft, bottomRight, topRight);
        }

        private void DiamondSquareAlgorithm(float topLeft, float bottomLeft, float bottomRight, float topRight)
        {
            int maxIndex = Width - 1;
            
            //Init corners
            HeightMap[0, 0] = topLeft;
            HeightMap[maxIndex, 0] = bottomLeft;
            HeightMap[maxIndex, maxIndex] = bottomRight;
            HeightMap[0, maxIndex] = topRight;

            for (int squareSize = maxIndex; squareSize >= 2; squareSize /= 2, Roughness /= 2)
            {
                int stepSize = squareSize / 2;

                //Squares
                for (int x = 0; x < maxIndex; x += squareSize)
                {
                    for (int y = 0; y < maxIndex; y += squareSize)
                    {
                        HeightMap[x + stepSize, y + stepSize] = GetRandomFloat(-Roughness, Roughness) +
                                                                (HeightMap[x, y] + HeightMap[x + squareSize, y] + HeightMap[x, y + squareSize] +
                                                                 HeightMap[x + squareSize, y + squareSize]) / 4;
                    }
                }

                //Diamonds
                for (int x = 0; x < maxIndex; x += stepSize)
                {
                    for (int y = (x + stepSize) % squareSize; y < maxIndex; y += squareSize)
                    {
                        HeightMap[x, y] = GetRandomFloat(-Roughness, Roughness) +
                                          (HeightMap[(x - stepSize + Width) % Width, y] + HeightMap[(x + stepSize) % Width, y] +
                                           HeightMap[x, (y + stepSize) % Width] + HeightMap[x, (y - stepSize + Width) % Width]) / 4;
                        if (x == 0)
                            HeightMap[maxIndex, y] = HeightMap[x, y];
                        if (y == 0)
                            HeightMap[x, maxIndex] = HeightMap[x, y];
                    }
                }
            }

        }

        private float GetRandomFloat(float min, float max)
        {
            return (float)rand.NextDouble() * (max - min) + min;
        }
    }
}
