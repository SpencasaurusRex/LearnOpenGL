using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;

namespace TKGL {
    public class Game : GameWindow {
        float[] rectangleVertices = {
            // positions         // colors          // uvs
             0.5f,  0.5f, 0.0f,  1.0f, 0.0f, 0.0f,  1.0f,  1.0f,
             0.5f, -0.5f, 0.0f,  0.0f, 1.0f, 0.0f,  1.0f, -1.0f,
            -0.5f, -0.5f, 0.0f,  0.0f, 0.0f, 1.0f, -1.0f, -1.0f,
            -0.5f,  0.5f, 0.0f,  1.0f, 1.0f, 0.0f, -1.0f,  1.0f,
        };

        uint[] rectangleIndices = {
            0, 1, 3,
            1, 2, 3
        };

        int vao;
        int vbo;
        int ebo;
        
        Shader shader;
        Texture texture;
        KeyboardState previousKeyboard;

        double totalTime;
        bool wireframe;

        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title){ }
        
        protected override void OnUpdateFrame(FrameEventArgs e) {
            var input = Keyboard.GetState();
            if (input.IsKeyDown(Key.Escape)) {
                Exit();
            }
            if (input.IsKeyDown(Key.F8) && !previousKeyboard.IsKeyDown(Key.F8)) {
                wireframe = !wireframe;
                if (wireframe) {
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                }
                else {
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                }
            }
            previousKeyboard = input;
            base.OnUpdateFrame(e);
        }

        protected override void OnLoad(EventArgs e) {
            Console.WriteLine($"Max Vertex Attribs: {GL.GetInteger(GetPName.MaxVertexAttribs)}");

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            shader = new Shader("vert.glsl", "frag.glsl");
            texture = new Texture(@"resources\wall.jpg");

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();

            GL.BindVertexArray(vao); 
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, rectangleVertices.Length * sizeof(float), rectangleVertices, BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), sizeof(float) * 3);
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), sizeof(float) * 6);
                GL.EnableVertexAttribArray(2);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
                GL.BufferData(BufferTarget.ElementArrayBuffer, rectangleIndices.Length * sizeof(uint), rectangleIndices, BufferUsageHint.StaticDraw);
                
                texture.Use();
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            }
            GL.BindVertexArray(0);

            base.OnLoad(e);
        }


        protected override void OnRenderFrame(FrameEventArgs e) {
            totalTime += RenderTime;
            
            GL.Clear(ClearBufferMask.ColorBufferBit);

            shader.Use();
            shader.SetUniform("time", (float)totalTime);

            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.Triangles, rectangleIndices.Length, DrawElementsType.UnsignedInt, 0);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e) {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e) {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(vbo);
            shader.Dispose();
            
            base.OnUnload(e);
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
        }
    }
}
