using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using MiniRenderer.Interfaces;


namespace MiniRenderer.Graphics
{
    public class OpenGLRenderer : IRenderer
    {
        // TODO: IRenderer implementation.

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

        void SetupCubeGeometry()
        {
            // TODO: Fill up the array, and finish the function.
            
            // 36 high points: X, Y, Z   +   R, G, B 
            float[] vertices =
            {
                
            };
        }

        private void SetupQuadGeometry()
        {
            // TODO: Fill up the array, and finish the function.

            // Full screen 2 triangles: X, Y  + U, V (texture)
            float[] quadVertices =
            {
                
            };
        }


    }
}
