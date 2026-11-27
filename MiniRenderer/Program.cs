using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using MiniRenderer.Graphics;
using MiniRenderer.Interfaces;
using System.Net.Mail;

namespace MiniRenderer
{
    public class AppWindow : GameWindow
    {
        readonly IRenderer _renderer;
        float _angle;

        public AppWindow(IRenderer renderer) : base(GameWindowSettings.Default, new NativeWindowSettings{ ClientSize = (800, 600), Title = "MiniRenderer - OpenGL 2-Pass Demo" })
        {
            _renderer = renderer;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            _renderer.Init(ClientSize.X, ClientSize.Y);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            _renderer.Resize(e.Width, e.Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            _angle += (float)args.Time * 1.2f;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            var model = Matrix4.CreateRotationY(_angle) * Matrix4.CreateRotationX(_angle * 0.6f);
            var view = Matrix4.LookAt(new Vector3(0, 0, 3), Vector3.Zero, Vector3.UnitY);
            var proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), ClientSize.X / (float)ClientSize.Y, 0.1f, 100f);

            _renderer.BeginFrame();
            _renderer.DrawCube(model, view, proj);
            _renderer.EndFrame();

            SwapBuffers();
        }

        protected override void OnUnload()
        {
            _renderer.Dispose();
            base.OnUnload();
        }

        public static void Main()
        {
            IRenderer renderer = new OpenGLRenderer();
            using var window = new AppWindow(renderer);
            window.Run();
        }

    }
}