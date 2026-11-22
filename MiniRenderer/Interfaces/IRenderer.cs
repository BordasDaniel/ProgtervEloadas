using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;

namespace MiniRenderer.Interfaces
{
    public interface IRenderer : IDisposable
    {
        void Init(int width, int height);
        void Resize(int width, int height);
        void BeginFrame();
        void DrawCube(Matrix4 model, Matrix4 view, Matrix4 projection);
        void EndFrame();
    }
}
