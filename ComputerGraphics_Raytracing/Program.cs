using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ComputerGraphics_Raytracing
{
    public class MyWindow : GameWindow
    {
        private readonly float[] vertices = {
            -1f, -1f,
            -1f,  1f,
             1f,  1f,
             1f, -1f
        };
        private int VertexBufferObject;
        private int VertexArrayObject;
        private Shaders shaders;
        private Camera camera;
        private Matrix3 rotationMatrixYR;
        private Matrix3 rotationMatrixYL;
        private int MAX_DEPTH;
        private float angle = (float)Math.PI / 60 / 4;
        private readonly string basicTitle;
        private int sceneID;

        public MyWindow(int width, int height, string title) :
            base(new GameWindowSettings() { UpdateFrequency = 60 },
                 new NativeWindowSettings() { Size = (width, height), Title = title })
        {
            basicTitle = title;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            VSync = VSyncMode.On;

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

            shaders = new Shaders("..\\..\\..\\raytracing.vert", "..\\..\\..\\raytracing.frag");
            shaders.ActivateProgram();

            camera = new Camera
            {
                position = new Vector3(0.0f, 0.0f, -10.0f),
                view = new Vector3(0.0f, 0.0f, 1.0f),
                up = new Vector3(0.0f, 1.0f, 0.0f),
                right = new Vector3(1.0f, 0.0f, 0.0f),
                scale = new Vector2((float)Size.X / Size.Y, 1.0f)
            };
            camera.view.Normalize();
            camera.up.Normalize();
            camera.right.Normalize();

            sceneID = 1;
            SceneInitializer.InitializeCamera(shaders, camera);
            SceneInitializer.InitializeDefaultScene(shaders);
            SceneInitializer.InitializeMaterials(shaders);
            
            rotationMatrixYR = Matrix3.CreateRotationY(-angle);
            rotationMatrixYL = Matrix3.CreateRotationY(angle);

            MAX_DEPTH = 4;
            shaders.Uniform1("MAX_DEPTH", MAX_DEPTH);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            Title = basicTitle + $" (FPS: {(int)(1 / e.Time)}; last frame: {e.Time * 1000:0.00}ms)";

            GL.Clear(ClearBufferMask.ColorBufferBit);

            shaders.ActivateProgram();
            GL.BindVertexArray(VertexArrayObject);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);

            SwapBuffers();
        }

        KeyboardState PreviousKeyboardState = null;
        bool RotateY = false;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            KeyboardState input = KeyboardState.GetSnapshot();

            // ESC
            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            // UP DOWN RIGHT LEFT
            if (input.IsKeyDown(Keys.Up))
            {
                Matrix3 rotationRightR = Matrix3.CreateFromAxisAngle(camera.right, angle);
                camera.view = rotationRightR * camera.view;
                camera.up = rotationRightR * camera.up;
                camera.right = rotationRightR * camera.right;
            }
            if (input.IsKeyDown(Keys.Down))
            {
                Matrix3 rotationRightL = Matrix3.CreateFromAxisAngle(camera.right, -angle);
                camera.view = rotationRightL * camera.view;
                camera.up = rotationRightL * camera.up;
                camera.right = rotationRightL * camera.right;
            }
            if (input.IsKeyDown(Keys.Right))
            {
                camera.view = rotationMatrixYR * camera.view;
                camera.up = rotationMatrixYR * camera.up;
                camera.right = rotationMatrixYR * camera.right;
            }
            if (input.IsKeyDown(Keys.Left))
            {
                camera.view = rotationMatrixYL * camera.view;
                camera.up = rotationMatrixYL * camera.up;
                camera.right = rotationMatrixYL * camera.right;
            }

            // W A S D SPACE SHIFT
            if (input.IsKeyDown(Keys.W))
            {
                Vector3 dir = Vector3.Normalize(Vector3.Cross(camera.right, new Vector3(0, 1, 0)));
                camera.position += dir / 10;
            }
            if (input.IsKeyDown(Keys.S))
            {
                Vector3 dir = Vector3.Normalize(Vector3.Cross(camera.right, new Vector3(0, 1, 0)));
                camera.position += -dir / 10;
            }
            if (input.IsKeyDown(Keys.D))
            {
                camera.position += camera.right / 10;
            }
            if (input.IsKeyDown(Keys.A))
            {
                camera.position += -camera.right / 10;
            }
            if (input.IsKeyDown(Keys.Space))
            {
                camera.position += new Vector3(0.0f, 1.0f, 0.0f) / 10;
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                camera.position += -new Vector3(0.0f, 1.0f, 0.0f) / 10;
            }

            if (input.IsKeyDown(Keys.D1))
            {
                if (sceneID != 1)
                {
                    SceneInitializer.InitializeDefaultScene(shaders);
                    sceneID = 1;
                }
            }
            else if (input.IsKeyDown(Keys.D2))
            {
                if (sceneID != 2)
                {
                    SceneInitializer.InitializeScene2(shaders);
                    sceneID = 2;
                }
            }

            if (PreviousKeyboardState != null)
            {
                // + -
                if (!PreviousKeyboardState.IsKeyDown(Keys.Equal) && input.IsKeyDown(Keys.Equal))
                {
                    MAX_DEPTH++;
                    MAX_DEPTH = Math.Min(MAX_DEPTH, 20);
                    shaders.Uniform1("MAX_DEPTH", MAX_DEPTH);
                }
                if (!PreviousKeyboardState.IsKeyDown(Keys.Minus) && input.IsKeyDown(Keys.Minus))
                {
                    MAX_DEPTH--;
                    MAX_DEPTH = Math.Max(MAX_DEPTH, 0);
                    shaders.Uniform1("MAX_DEPTH", MAX_DEPTH);
                }
                // R
                if (!PreviousKeyboardState.IsKeyDown(Keys.R) && input.IsKeyDown(Keys.R))
                {
                    RotateY = !RotateY;
                }
            }
            PreviousKeyboardState = input;

            if (RotateY)
            {
                camera.position = rotationMatrixYR * camera.position;
                camera.view = rotationMatrixYR * camera.view;
                camera.up = rotationMatrixYR * camera.up;
                camera.right = rotationMatrixYR * camera.right;
            }
            shaders.Uniform3("uCamera.position", camera.position);
            shaders.Uniform3("uCamera.view", camera.view);
            shaders.Uniform3("uCamera.up", camera.up);
            shaders.Uniform3("uCamera.right", camera.right);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            camera.scale = new Vector2((float)Size.X / Size.Y, 1.0f);
            shaders.Uniform2("uCamera.scale", camera.scale);

            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            shaders.Dispose();
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            MyWindow window = new MyWindow(800, 600, "ComputerGraphics: Raytracing");
            window.Run();
        }
    }
}
