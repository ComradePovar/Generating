using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Generating
{
    class Texture
    {
        public int ID { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Bitmap bitmap;

        public Texture()
        {

        }
        public Texture(string path)
        {
            LoadTexture(path);
        }

        private void LoadTexture(string path)
        {
            if (!File.Exists("Assets/" + path))
            {
                throw new FileNotFoundException("File not found.");
            }

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            ID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, ID);

            bitmap = new Bitmap("Assets/" + path);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, data.Scan0);

            bitmap.UnlockBits(data);

            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)
            //    TextureWrapMode.Clamp);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)
            //    TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)
                TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)
                TextureMagFilter.Linear);
            
        }
    }
}
