using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK.Graphics.OpenGL4;

namespace MiniRenderer.Graphics
{
    public class Framebuffer : IDisposable
    {
        public int FboHandle { get; private set; }
        public int ColorTextureHandle { get; private set; }
        public int _rboHandle { get; private set; }

        public Framebuffer(int width, int height)
        {
            Create(width, height);
        }

        public void Resize(int widht, int height)
        {
            Dispose();
            Create(widht, height);
        }

        void Create(int width, int height)
        { 
            FboHandle = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FboHandle);

            ColorTextureHandle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, ColorTextureHandle);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, ColorTextureHandle, 0);

            _rboHandle = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _rboHandle);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, _rboHandle);
        }

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FboHandle);
        }

        public static void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Dispose()
        {
            GL.DeleteFramebuffer(FboHandle);
            GL.DeleteTexture(ColorTextureHandle);
            GL.DeleteRenderbuffer(_rboHandle);
        }
    }
}
