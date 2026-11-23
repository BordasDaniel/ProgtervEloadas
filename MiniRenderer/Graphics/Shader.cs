using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace MiniRenderer.Graphics
{
    public class Shader : IDisposable
    {
        public int Handle { get; }
        public Shader(string vertexPath, string fragmentPath)
        {
            int vert = CompileShader(ShaderType.VertexShader, File.ReadAllText(vertexPath));
            int frag = CompileShader(ShaderType.FragmentShader, File.ReadAllText(fragmentPath));

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vert);
            GL.AttachShader(Handle, frag);
            GL.LinkProgram(Handle);

            GL.DeleteShader(vert);
            GL.DeleteShader(frag);
        }

        static int CompileShader(ShaderType type, string source)
        {
            int shader = GL.CreateShader(type);
            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
            if (status == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error compiling {type} shader: {infoLog}");
            }
            return shader;
        }

        public void Use(){
            GL.UseProgram(Handle);
        }

        public void SetMatrix4(string name, Matrix4 matrix)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.UniformMatrix4(location, false, ref matrix);
        }

        public void SetInt(string name, int value)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform1(location, value);
        }

        public void Dispose()
        {
            GL.DeleteProgram(Handle);
        }
    }
}
