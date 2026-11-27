using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using MiniRenderer.Interfaces;

namespace MiniRenderer.Graphics
{
    public class OpenGLRenderer : IRenderer
    {
        Shader _cubeShader = null!;
        Shader _postShader = null!;
        Framebuffer _fbo = null!;

        int _cubeVao, _cubeVbo;
        int _quadVao, _quadVbo;

        public void Init(int width, int height)
        {
            _cubeShader = new("../Shaders/cube.vert", "../Shaders/cube.frag");
            _postShader = new("../Shaders/post.vert", "../Shaders/post.frag");
            _fbo = new(width, height);

            SetupCubeGeometry();
            SetupQuadGeometry();
        }

        public void Resize(int width, int height)
        {
            if (width > 0 && height > 0)
            {
                GL.Viewport(0, 0, width, height);
                _fbo.Resize(width, height);
            }
        }

        public void BeginFrame()
        {
            // 1. phase: Draw onto the Framebuffer inside the RAM
            
            _fbo.Bind();
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0.12f, 0.14f, 0.18f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void DrawCube(Matrix4 model, Matrix4 view, Matrix4 projection)
        {
            _cubeShader.Use();
            _cubeShader.SetMatrix4("uModel", model);
            _cubeShader.SetMatrix4("uView", view);
            _cubeShader.SetMatrix4("uProjection", projection);

            GL.BindVertexArray(_cubeVao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }

        public void EndFrame()
        {
            // 2. phase: Switch to the real screen and project the texture
            
            Framebuffer.Unbind();
            GL.Disable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _postShader.Use();
            _postShader.SetInt("uScreenTexture", 0);
            
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _fbo.ColorTextureHandle);

            GL.BindVertexArray(_quadVao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }

        void SetupCubeGeometry()
        {  
            // 36 high points: X, Y, Z   +   R, G, B 
           float[] vertices = {
            -0.5f,-0.5f,-0.5f, 1,0,0,  0.5f,-0.5f,-0.5f, 1,0,0,  0.5f, 0.5f,-0.5f, 1,0,0,
             0.5f, 0.5f,-0.5f, 1,0,0, -0.5f, 0.5f,-0.5f, 1,0,0, -0.5f,-0.5f,-0.5f, 1,0,0,
            -0.5f,-0.5f, 0.5f, 0,1,0,  0.5f,-0.5f, 0.5f, 0,1,0,  0.5f, 0.5f, 0.5f, 0,1,0,
             0.5f, 0.5f, 0.5f, 0,1,0, -0.5f, 0.5f, 0.5f, 0,1,0, -0.5f,-0.5f, 0.5f, 0,1,0,
            -0.5f, 0.5f, 0.5f, 0,0,1, -0.5f, 0.5f,-0.5f, 0,0,1, -0.5f,-0.5f,-0.5f, 0,0,1,
            -0.5f,-0.5f,-0.5f, 0,0,1, -0.5f,-0.5f, 0.5f, 0,0,1, -0.5f, 0.5f, 0.5f, 0,0,1,
             0.5f, 0.5f, 0.5f, 1,1,0,  0.5f, 0.5f,-0.5f, 1,1,0,  0.5f,-0.5f,-0.5f, 1,1,0,
             0.5f,-0.5f,-0.5f, 1,1,0,  0.5f,-0.5f, 0.5f, 1,1,0,  0.5f, 0.5f, 0.5f, 1,1,0,
            -0.5f,-0.5f,-0.5f, 0,1,1,  0.5f,-0.5f,-0.5f, 0,1,1,  0.5f,-0.5f, 0.5f, 0,1,1,
             0.5f,-0.5f, 0.5f, 0,1,1, -0.5f,-0.5f, 0.5f, 0,1,1, -0.5f,-0.5f,-0.5f, 0,1,1,
            -0.5f, 0.5f,-0.5f, 1,0,1,  0.5f, 0.5f,-0.5f, 1,0,1,  0.5f, 0.5f, 0.5f, 1,0,1,
             0.5f, 0.5f, 0.5f, 1,0,1, -0.5f, 0.5f, 0.5f, 1,0,1, -0.5f, 0.5f,-0.5f, 1,0,1
            };

            _cubeVao = GL.GenVertexArray();
            _cubeVbo = GL.GenBuffer();
            GL.BindVertexArray(_cubeVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _cubeVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3* sizeof(float));
            GL.EnableVertexAttribArray(1);
        }

        private void SetupQuadGeometry()
        {
            // Full screen 2 triangles: X, Y  + U, V (texture)
            float[] quadVertices = {
            -1f,  1f,  0f, 1f,
            -1f, -1f,  0f, 0f,
             1f, -1f,  1f, 0f,
            -1f,  1f,  0f, 1f,
             1f, -1f,  1f, 0f,
             1f,  1f,  1f, 1f
            };

            _quadVao = GL.GenVertexArray();
            _quadVbo = GL.GenBuffer();
            GL.BindVertexArray(_quadVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _quadVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Length * sizeof(float), quadVertices, BufferUsageHint.StreamDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4* sizeof(float), 2* sizeof(float));
            GL.EnableVertexAttribArray(1);
        }

        public void Dispose()
        {
            _cubeShader.Dispose();
            _postShader.Dispose();
            _fbo.Dispose();

            GL.DeleteVertexArray(_cubeVao);
            GL.DeleteBuffer(_cubeVbo);

            GL.DeleteVertexArray(_quadVao);
            GL.DeleteBuffer(_quadVbo);
        }

    }
}
