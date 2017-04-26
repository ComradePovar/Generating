using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Generating.Textures;

namespace Generating
{
    class WaterFBO : IDisposable
    {
        private int defaultWidth;
        private int defaultHeight;

        private const int REFLECTION_WIDTH = 1280;
        private const int REFLECTION_HEIGHT = 720;

        private const int REFRACTION_WIDTH = 1280;
        private const int REFRACTION_HEIGHT = 720;

        public int ReflectionFrameBuffer { get; private set; }
        public int ReflectionTexture { get; private set; }
        public int ReflectionDepthBuffer { get; private set; }
        public int ReflectionSampler { get; private set; }


        public int RefractionFrameBuffer { get; private set; }
        public int RefractionTexture { get; private set; }
        public int RefractionDepthTexture { get; private set; }
        public int RefractionSampler { get; private set; }

        public Texture2D DudvMapTexture { get; private set; }

        public WaterFBO(int defaultWidth, int defaultHeight)
        {
            DudvMapTexture = new Texture2D("dudvMap.jpg");
            this.defaultWidth = defaultWidth;
            this.defaultHeight = defaultHeight;
            initReflectionFrameBuffer();
            initRefractionFrameBuffer();
        }

        private void initReflectionFrameBuffer()
        {
            ReflectionFrameBuffer = createFrameBuffer();
            ReflectionTexture = createTextureAttachment(REFLECTION_WIDTH, REFLECTION_HEIGHT);
            ReflectionDepthBuffer = createDepthBufferAttachment(REFLECTION_WIDTH, REFLECTION_HEIGHT);
            ReflectionSampler = GL.GenSampler();
            UnbindFrameBuffer();
        }

        private void initRefractionFrameBuffer()
        {
            RefractionFrameBuffer = createFrameBuffer();
            RefractionTexture = createTextureAttachment(REFRACTION_WIDTH, REFRACTION_HEIGHT);
            RefractionDepthTexture = createDepthTextureAttachment(REFRACTION_WIDTH, REFRACTION_HEIGHT);
            RefractionSampler = GL.GenSampler();
            UnbindFrameBuffer();
        }
        private int createFrameBuffer()
        {
            int frameBuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
            return frameBuffer;
        }
        public int createTextureAttachment(int width, int height)
        {
            int texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Bgra, PixelType.UnsignedByte,
                (IntPtr)null);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, texture, 0);
            return texture;
        }

        public int createDepthTextureAttachment(int width, int height)
        {
            int texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32, width, height, 0, PixelFormat.DepthComponent,
                PixelType.Float, (IntPtr)null);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, texture, 0);
            return texture;
        }

        public int createDepthBufferAttachment(int width, int height)
        {
            int depthBuffer = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent32, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                RenderbufferTarget.Renderbuffer, depthBuffer);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            return depthBuffer;
        }

        public void BindReflectionFrameBuffer()
        {
            BindFrameBuffer(ReflectionFrameBuffer, REFLECTION_WIDTH, REFLECTION_HEIGHT);
        }

        public void BindRefractionFrameBuffer()
        {
            BindFrameBuffer(RefractionFrameBuffer, REFRACTION_WIDTH, REFRACTION_HEIGHT);
        }

        public void BindFrameBuffer(int frameBuffer, int width, int height)
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            GL.Viewport(0, 0, width, height);
        }

        public void UnbindFrameBuffer()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, defaultWidth, defaultHeight);
        }

        public void OnResize(int width, int height)
        {
            defaultWidth = width;
            defaultHeight = height;
        }

        public void Dispose()
        {
            GL.DeleteFramebuffer(ReflectionFrameBuffer);
            GL.DeleteTexture(ReflectionTexture);
            GL.DeleteTexture(ReflectionDepthBuffer);
            GL.DeleteFramebuffer(RefractionFrameBuffer);
            GL.DeleteTexture(RefractionTexture);
            GL.DeleteTexture(RefractionDepthTexture);
        }
    }
}
