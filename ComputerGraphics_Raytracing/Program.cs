using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ComputerGraphics_Raytracing
{
    public class MyWindow : GameWindow
    {
        public MyWindow(int width, int height, string title) :
            base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title }) { }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            KeyboardState input = KeyboardState;

            GL.Clear(ClearBufferMask.ColorBufferBit);

            SwapBuffers();

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            MyWindow window = new MyWindow(800, 600, "Raytracing");
            Shaders shaders = new Shaders("D:\\source\\ComputerGraphics_Raytracing\\ComputerGraphics_Raytracing\\raytracing.vert",
                                          "D:\\source\\ComputerGraphics_Raytracing\\ComputerGraphics_Raytracing\\raytracing.frag");
            shaders.ActivateProgram();
            window.Run();
        }
    }
}
