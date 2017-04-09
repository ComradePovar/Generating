using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Generating
{
    class Texture
    {
        public int ID { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public TextureMinFilter MinFilter { get; private set; }
        public TextureMagFilter MagFilter { get; private set; }

        public Bitmap bitmap;
        private int samplerID;
        
        public Texture(string textureName)
        {
            LoadTexture(textureName);
        }

        private void LoadTexture(string textureName)
        {
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            ID = GL.GenTexture();
            samplerID = GL.GenSampler();
            GL.BindTexture(TextureTarget.Texture2D, ID);

            bitmap = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Generating.Assets." + textureName));
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, data.Scan0);

            bitmap.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)
                TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)
                TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)
                TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)
                TextureMagFilter.Linear);
            samplerID = GL.GenSampler();
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private void SetFiltering(TextureMinFilter min, TextureMagFilter mag)
        {
            //TODO: doesn't works
            GL.SamplerParameter(samplerID, SamplerParameter.TextureMagFilter, (int)mag);
            GL.SamplerParameter(samplerID, SamplerParameter.TextureMinFilter, (int)min);
            this.MinFilter = min;
            this.MagFilter = mag;
        }
    }
}
