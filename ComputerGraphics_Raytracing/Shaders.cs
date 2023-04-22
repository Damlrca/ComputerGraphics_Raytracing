using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace ComputerGraphics_Raytracing
{
    class Shaders
    {
        private int BasicVertexShader;
        private int BasicFragmentShader;
        private int BasicProgramID;

        public Shaders(string vertex_filename, string fragment_filename)
        {
            BasicVertexShader = CreateShader(vertex_filename, ShaderType.VertexShader);
            BasicFragmentShader = CreateShader(fragment_filename, ShaderType.FragmentShader);

            BasicProgramID = GL.CreateProgram();
            GL.AttachShader(BasicProgramID, BasicVertexShader);
            GL.AttachShader(BasicProgramID, BasicFragmentShader);
            GL.LinkProgram(BasicProgramID);

            int code;
            GL.GetProgram(BasicProgramID, GetProgramParameterName.LinkStatus, out code);
            if (code == 0)
            {
                string infolog = GL.GetProgramInfoLog(BasicProgramID);
                Console.WriteLine($"Ошибка линковки шейдерной программы\n\n{infolog}");
            }
            else
            {
                Console.WriteLine($"Шейдерная программа успешно слинкована");
            }
        }

        public void ActivateProgram()
        {
            GL.UseProgram(BasicProgramID);
        }

        public void DeactivateProgram()
        {
            GL.UseProgram(0);
        }

        private int CreateShader(string filename, ShaderType type)
        {
            int shaderID = GL.CreateShader(type);
            using (StreamReader sr = new StreamReader(filename))
            {
                GL.ShaderSource(shaderID, sr.ReadToEnd());
            }
            GL.CompileShader(shaderID);

            int code;
            GL.GetShader(shaderID, ShaderParameter.CompileStatus, out code);
            if (code == 0)
            {
                string infolog = GL.GetShaderInfoLog(shaderID);
                Console.WriteLine($"Ошибка компиляции шейдера {filename}\n\n{infolog}");
            }
            else
            {
                Console.WriteLine($"Шейдер {filename} успешно скомпилирован");
            }

            return shaderID;
        }
    }
}
