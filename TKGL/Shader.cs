using OpenTK.Graphics.OpenGL;
using System.IO;

namespace TKGL {
    public class Shader {
        int handle;
        
        public Shader(string vertexPath, string fragmentPath) {
            int vertexShader;
            int fragmentShader;

            var vertexShaderSource = File.ReadAllText(vertexPath);
            vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);
            string infoLog = GL.GetShaderInfoLog(vertexShader);
            if (!string.IsNullOrEmpty(infoLog)) {
                System.Console.Error.WriteLine(infoLog);
            }
            
            var fragmentShaderSource = File.ReadAllText(fragmentPath);
            fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);
            infoLog = GL.GetShaderInfoLog(vertexShader);
            if (!string.IsNullOrEmpty(infoLog)) {
                System.Console.Error.WriteLine(infoLog);
            }

            handle = GL.CreateProgram();
            GL.AttachShader(handle, vertexShader);
            GL.AttachShader(handle, fragmentShader);

            GL.LinkProgram(handle);

            GL.DetachShader(handle, vertexShader);
            GL.DetachShader(handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public void Use() {
            GL.UseProgram(handle);
        }

        public void Dispose() {
            GL.DeleteProgram(handle);
        }

        public int GetUniformLocation(string uniformName) => GL.GetUniformLocation(handle, uniformName);
        public void SetUniform(string uniformName, float value) {
            GL.Uniform1(GetUniformLocation(uniformName), value);
        }
        public void SetUniform(string uniformName, OpenTK.Vector4 vec) {
            GL.Uniform4(GetUniformLocation(uniformName), vec);
        }

        public void SetUniform(string uniformName, OpenTK.Graphics.Color4 col) {
            GL.Uniform4(GetUniformLocation(uniformName), col);
        }

        public int GetAttribLocation(string attribName) => GL.GetAttribLocation(handle, attribName);
    }
}
