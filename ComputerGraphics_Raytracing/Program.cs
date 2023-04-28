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

            initializeCamera();
            initializeScene();
        }

        private void initializeCamera()
        {
            shaders.Uniform3("uCamera.position", 0.0f, 0.0f, -8.0f);
            shaders.Uniform3("uCamera.view", 0.0f, 0.0f, 1.0f);
            shaders.Uniform3("uCamera.up", 0.0f, 1.0f, 0.0f);
            shaders.Uniform3("uCamera.right", 1.0f, 0.0f, 0.0f);
            shaders.Uniform2("uCamera.scale", (float)Size.X / Size.Y, 1.0f);
        }

        private void initializeScene()
        {
            // TRIANGLES
            // left wall: triangles 0, 1
            shaders.Uniform3("triangles[0].v1", -5.0f, -5.0f, -5.0f);
            shaders.Uniform3("triangles[0].v2", -5.0f, 5.0f, 5.0f);
            shaders.Uniform3("triangles[0].v3", -5.0f, 5.0f, -5.0f);
            shaders.Uniform1("triangles[0].MaterialId", 0);

            shaders.Uniform3("triangles[1].v1", -5.0f, -5.0f, -5.0f);
            shaders.Uniform3("triangles[1].v2", -5.0f, -5.0f, 5.0f);
            shaders.Uniform3("triangles[1].v3", -5.0f, 5.0f, 5.0f);
            shaders.Uniform1("triangles[1].MaterialId", 0);

            // right wall: triangles 2, 3

            // down wall: triangles 4, 5
            shaders.Uniform3("triangles[4].v1", 5.0f, -5.0f, -5.0f);
            shaders.Uniform3("triangles[4].v2", -5.0f, -5.0f, 5.0f);
            shaders.Uniform3("triangles[4].v3", -5.0f, -5.0f, -5.0f);
            shaders.Uniform1("triangles[4].MaterialId", 0);
            
            shaders.Uniform3("triangles[5].v1", 5.0f, -5.0f, -5.0f);
            shaders.Uniform3("triangles[5].v2", 5.0f, -5.0f, 5.0f);
            shaders.Uniform3("triangles[5].v3", -5.0f, -5.0f, 5.0f);
            shaders.Uniform1("triangles[5].MaterialId", 0);

            // up wall: triangles 6, 7

            // back wall: triangles 8, 9
            shaders.Uniform3("triangles[8].v1", -5.0f, -5.0f, 5.0f);
            shaders.Uniform3("triangles[8].v2", 5.0f, 5.0f, 5.0f);
            shaders.Uniform3("triangles[8].v3", -5.0f, 5.0f, 5.0f);
            shaders.Uniform1("triangles[8].MaterialId", 0);
            
            shaders.Uniform3("triangles[9].v1", -5.0f, -5.0f, 5.0f);
            shaders.Uniform3("triangles[9].v2", 5.0f, -5.0f, 5.0f);
            shaders.Uniform3("triangles[9].v3", 5.0f, 5.0f, 5.0f);
            shaders.Uniform1("triangles[9].MaterialId", 0);

            // front wall: triangles 10, 11 ?

            // SPHERES
            shaders.Uniform3("spheres[0].center", -1.0f, -1.0f, -2.0f);
            shaders.Uniform1("spheres[0].radius", 2.0f);
            shaders.Uniform1("spheres[0].MaterialId", 0);

            shaders.Uniform3("spheres[1].center", 2.0f, 1.0f, 2.0f);
            shaders.Uniform1("spheres[1].radius", 1.0f);
            shaders.Uniform1("spheres[1].MaterialId", 0);
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

            shaders.Uniform2("uCamera.scale", (float)Size.X / Size.Y, 1.0f);

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
