using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using MiniRenderer.Interfaces;

namespace MiniRenderer.Graphics
{
    // Implements the renderer contract with an off-screen scene pass and a screen-space post-process pass.
    public class OpenGLRenderer : IRenderer
    {
        // The first program colors the cube; the second applies effects to the rendered image.
        Shader _cubeShader = null!;
        Shader _postShader = null!;

        // Stores the scene color and depth while the cube is rendered off-screen.
        Framebuffer _fbo = null!;

        // VAOs describe the vertex layouts; VBOs contain the actual vertex data.
        int _cubeVao, _cubeVbo;
        int _quadVao, _quadVbo;

        public void Init(int width, int height)
        {
            // Shaders are copied beside the application by the project file.
            string ShaderPath(string file) => Path.Combine(AppContext.BaseDirectory, "Shaders", file);

            _cubeShader = new(ShaderPath("cube.vert"), ShaderPath("cube.frag"));
            _postShader = new(ShaderPath("post.vert"), ShaderPath("post.frag"));
            _fbo = new(width, height);

            SetupCubeGeometry();
            SetupQuadGeometry();
        }

        public void Resize(int width, int height)
        {
            if (width > 0 && height > 0)
            {
                // Ignore minimized/zero-sized windows; otherwise resize both screen and off-screen rendering.
                GL.Viewport(0, 0, width, height);
                _fbo.Resize(width, height);
            }
        }

        public void BeginFrame()
        {
            // Pass 1: render the 3D scene into the framebuffer's color texture and depth buffer.
            _fbo.Bind();
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0.12f, 0.14f, 0.18f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void DrawCube(Matrix4 model, Matrix4 view, Matrix4 projection)
        {
            _cubeShader.Use();
            // Supply the transforms used by the vertex shader to move the cube into clip space.
            _cubeShader.SetMatrix4("uModel", model);
            _cubeShader.SetMatrix4("uView", view);
            _cubeShader.SetMatrix4("uProjection", projection);

            GL.BindVertexArray(_cubeVao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }

        public void EndFrame()
        {
            // Pass 2: return to the window framebuffer and draw a full-screen quad with the scene texture.
            Framebuffer.Unbind();
            GL.Disable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _postShader.Use();
            // The post-process shader reads the framebuffer color from texture unit 0.
            _postShader.SetInt("uScreenTexture", 0);
            
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _fbo.ColorTextureHandle);

            GL.BindVertexArray(_quadVao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }

        void SetupCubeGeometry()
        {  
            // The cube has 6 faces x 2 triangles x 3 vertices; each vertex stores XYZ position and RGB color.
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

            // Each vertex is six floats: the first three are position and the next three are color.
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3* sizeof(float));
            GL.EnableVertexAttribArray(1);
        }

        private void SetupQuadGeometry()
        {
            // Two triangles cover the screen; each vertex pairs clip-space XY with texture UV coordinates.
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

            // Read two position floats followed by two texture-coordinate floats from each vertex.
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4* sizeof(float), 2* sizeof(float));
            GL.EnableVertexAttribArray(1);
        }

        public void Dispose()
        {
            // Delete every GPU resource created by this renderer.
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
