using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using MiniRenderer.Graphics;
using MiniRenderer.Interfaces;
using System.Net.Mail;

namespace MiniRenderer
{
    // Owns the window and connects OpenTK's frame events to the renderer.
    public class AppWindow : GameWindow
    {
        // Depend on the renderer interface so the window is not tied to OpenGL details.
        readonly IRenderer _renderer;

        // Accumulated rotation angle, updated using elapsed frame time.
        float _angle;

        public AppWindow(IRenderer renderer) : base(GameWindowSettings.Default, new NativeWindowSettings{ ClientSize = (800, 600), Title = "MiniRenderer - OpenGL 2-Pass Demo" })
        {
            _renderer = renderer;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            // Create renderer resources after the window's OpenGL context is available.
            _renderer.Init(ClientSize.X, ClientSize.Y);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            // Keep the viewport and off-screen target matched to the window size.
            _renderer.Resize(e.Width, e.Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            // Scale rotation by elapsed time so it stays consistent across frame rates.
            _angle += (float)args.Time * 1.2f;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            // Build the cube's object transform, camera view, and perspective projection.
            var model = Matrix4.CreateRotationY(_angle) * Matrix4.CreateRotationX(_angle * 0.6f);
            var view = Matrix4.LookAt(new Vector3(0, 0, 3), Vector3.Zero, Vector3.UnitY);
            var proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), ClientSize.X / (float)ClientSize.Y, 0.1f, 100f);

            // Render the scene to an off-screen target, then process it for display.
            _renderer.BeginFrame();
            _renderer.DrawCube(model, view, proj);
            _renderer.EndFrame();

            // Present the completed back buffer in the window.
            SwapBuffers();
        }

        protected override void OnUnload()
        {
            // Release GPU resources while the window's graphics context is still active.
            _renderer.Dispose();
            base.OnUnload();
        }

        public static void Main()
        {
            // Choose the OpenGL backend while keeping window lifecycle code backend-agnostic.
            IRenderer renderer = new OpenGLRenderer();
            using var window = new AppWindow(renderer);
            window.Run();
        }

    }
}