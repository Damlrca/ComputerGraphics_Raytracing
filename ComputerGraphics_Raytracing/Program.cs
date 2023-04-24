using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ComputerGraphics_Raytracing
{
    public class MyWindow : GameWindow
    {
        private float[] vertices = {
            -1f, -1f,
            -1f,  1f,
             1f,  1f,
             1f, -1f
        };
        private int VertexBufferObject;
        private int VertexArrayObject;
        private Shaders shaders;

        public MyWindow(int width, int height, string title) :
            base(new GameWindowSettings() { RenderFrequency = 60 }, new NativeWindowSettings() { Size = (width, height), Title = title })
        { }

        protected override void OnLoad()
        {
            base.OnLoad();

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);   

            shaders = new Shaders("..\\..\\..\\raytracing.vert", "..\\..\\..\\raytracing.frag");
            shaders.ActivateProgram();

            shaders.Uniform2("uCamera.aspect", (float)Size.X / Size.Y, 1.0f);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            
            shaders.Dispose();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            shaders.ActivateProgram();
            GL.BindVertexArray(VertexArrayObject);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            KeyboardState input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            shaders.Uniform2("uCamera.aspect", (float)Size.X / Size.Y, 1.0f);

            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            MyWindow window = new MyWindow(800, 600, "Raytracing");
            window.Run();
        }
    }
}
