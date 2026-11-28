using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace MiniRenderer.Graphics
{
    /// <summary>
    /// Compiles a vertex/fragment shader pair, links them into a GPU program, and provides uniform helpers.
    /// </summary>
    public class Shader : IDisposable
    {
        public int Handle { get; }

        /// <summary>Loads the shader source files and creates their linked OpenGL program.</summary>
        public Shader(string vertexPath, string fragmentPath)
        {
            // Compile each stage separately before linking them into a program.
            int vert = CompileShader(ShaderType.VertexShader, File.ReadAllText(vertexPath));
            int frag = CompileShader(ShaderType.FragmentShader, File.ReadAllText(fragmentPath));

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vert);
            GL.AttachShader(Handle, frag);
            GL.LinkProgram(Handle);

            // The linked program retains the compiled stages, so the temporary shader objects can be deleted.
            GL.DeleteShader(vert);
            GL.DeleteShader(frag);
        }

        static int CompileShader(ShaderType type, string source)
        {
            // OpenGL reports compilation errors through the shader info log.
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

        /// <summary>Makes this program active for subsequent draw calls.</summary>
        public void Use(){
            GL.UseProgram(Handle);
        }

        /// <summary>Sets a matrix uniform, such as a model, view, or projection transform.</summary>
        public void SetMatrix4(string name, Matrix4 matrix)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.UniformMatrix4(location, false, ref matrix);
        }

        /// <summary>Sets an integer uniform, commonly used to select a texture unit.</summary>
        public void SetInt(string name, int value)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform1(location, value);
        }

        public void Dispose()
        {
            // Release the linked program when its owner no longer needs it.
            GL.DeleteProgram(Handle);
        }
    }
}
