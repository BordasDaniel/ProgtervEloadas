using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK.Graphics.OpenGL4;

namespace MiniRenderer.Graphics
{
    /// <summary>
    /// Off-screen render target made from a color texture and a depth/stencil renderbuffer.
    /// </summary>
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
            // OpenGL attachments have fixed dimensions, so recreate them at the new size.
            Dispose();
            Create(widht, height);
        }

        void Create(int width, int height)
        { 
            // Create and bind the framebuffer that receives the scene pass.
            FboHandle = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FboHandle);

            // The color texture can be sampled by the later post-processing pass.
            ColorTextureHandle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, ColorTextureHandle);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, ColorTextureHandle, 0);

            // Depth testing needs a depth attachment even though only color is sampled afterward.
            _rboHandle = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _rboHandle);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, _rboHandle);
        }

        public void Bind()
        {
            // Subsequent drawing goes to this off-screen target until Unbind is called.
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FboHandle);
        }

        public static void Unbind()
        {
            // Framebuffer 0 is the window's default render target.
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Dispose()
        {
            // Release the framebuffer and all three GPU attachments it owns.
            GL.DeleteFramebuffer(FboHandle);
            GL.DeleteTexture(ColorTextureHandle);
            GL.DeleteRenderbuffer(_rboHandle);
        }
    }
}
