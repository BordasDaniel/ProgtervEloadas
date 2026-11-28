using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;

namespace MiniRenderer.Interfaces
{
    /// <summary>
    /// Describes the frame lifecycle expected by the window, independent of graphics API.
    /// </summary>
    public interface IRenderer : IDisposable
    {
        /// <summary>Creates renderer resources for the initial window dimensions.</summary>
        void Init(int width, int height);

        /// <summary>Updates the viewport and render targets after a window resize.</summary>
        void Resize(int width, int height);

        /// <summary>Begins a frame and prepares the render target for scene drawing.</summary>
        void BeginFrame();

        /// <summary>Draws the cube using its model, camera-view, and projection matrices.</summary>
        void DrawCube(Matrix4 model, Matrix4 view, Matrix4 projection);

        /// <summary>Finishes the scene pass and presents its processed image.</summary>
        void EndFrame();
    }
}
