using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using OpenTK.Graphics.OpenGL;

namespace Generating.Textures
{
    class CubeMapTexture : Texture
    {
        public CubeMapTexture(string textureName) : base(textureName)
        {
        }

        public override void LoadTexture(string textureName)
        {
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            ID = GL.GenTexture();
            SamplerID = GL.GenSampler();
            GL.BindTexture(TextureTarget.TextureCubeMap, ID);

            string[] faces = { "right.jpg", "left.jpg", "top.jpg", "bottom.jpg", "back.jpg", "front.jpg" };
            for (int i = 0; i < faces.Length; i++)
            {
                Bitmap bitmap = new Bitmap(
                    Assembly.GetExecutingAssembly().GetManifestResourceStream("Generating.Assets.Skybox." + faces[i]));
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, 
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                              OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                bitmap.UnlockBits(data);
            }
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}
