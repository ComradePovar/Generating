using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace Generating
{
    public class SpriteHandler
    {
        public static void Draw(Texture texture, Vector2 position, Vector2 scale, Color color, Vector2 origin)
        {
            Vector2[] vertices = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1)
            };

            GL.BindTexture(TextureTarget.Texture2D, texture.ID);

            GL.Begin(BeginMode.Quads);

            GL.Color3(color);
            for (int i = 0; i < 4; i++)
            {
                GL.TexCoord2(vertices[i]);

                vertices[i].X *= texture.Width;
                vertices[i].Y *= texture.Height;
                vertices[i] -= origin;
                vertices[i].X *= scale.X;
                vertices[i].Y *= scale.Y;
                vertices[i] += position;

                GL.Vertex2(vertices[i]);
            }

            GL.End();
        }

        public static void Begin(int screenWidth, int screenHeight)
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            GL.Ortho(-screenWidth / 2f, screenHeight / 2f, screenHeight / 2f, -screenHeight / 2, 0.0f, 1.0f);
            //Matrix4 projection = Matrix4.CreateOrthographic(screenWidth, screenHeight, 0.0f, 1f);
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadMatrix(ref projection);
        }
    }
}
