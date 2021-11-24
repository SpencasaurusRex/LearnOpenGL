using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using StbImageSharp;
using System;
using Valve.VR;

namespace TKGL {
    public class Game : GameWindow {
        float[] rectangleVertices = {
            // positions         // colors          // uvs
             0.5f,  0.5f, 0.0f,  1.0f, 0.0f, 0.0f,  2.0f, 2.0f,
             0.5f, -0.5f, 0.0f,  0.0f, 1.0f, 0.0f,  2.0f, 0.0f,
            -0.5f, -0.5f, 0.0f,  0.0f, 0.0f, 1.0f,  0.0f, 0.0f,
            -0.5f,  0.5f, 0.0f,  1.0f, 1.0f, 0.0f,  0.0f, 2.0f,
        };

        uint[] rectangleIndices = {
            0, 1, 3,
            1, 2, 3
        };

        int vao;
        int vbo;
        int ebo;

        Shader shader;
        Texture wallTexture;
        Texture faceTexture;
        KeyboardState previousKeyboard;

        double totalTime;
        bool wireframe;
        float blend;

        float x;
        float y;

        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title) { }
        
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

            const float blendDelta = 0.01f;
            if (input.IsKeyDown(Key.Up)) {
                blend = Math.Min(blend + blendDelta, 1.0f);
            }
            if (input.IsKeyDown(Key.Down)) {
                blend = Math.Max(blend - blendDelta, 0.0f);
            }

            float dx = 0;
            float dy = 0;
            if (input.IsKeyDown(Key.W)) {
                dy += 1;
            }
            if (input.IsKeyDown(Key.S)) {
                dy -= 1;
            }
            if (input.IsKeyDown(Key.D)) {
                dx += 1;
            }
            if (input.IsKeyDown(Key.A)) {
                dx -= 1;
            }
            x += dx * (float)e.Time;
            y += dy * (float)e.Time;

            previousKeyboard = input;
            base.OnUpdateFrame(e);
        }

        protected override void OnLoad(EventArgs e) {
            
            // Console.WriteLine($"Max Vertex Attribs: {GL.GetInteger(GetPName.MaxVertexAttribs)}");
            StbImage.stbi_set_flip_vertically_on_load(1);

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            shader = new Shader(@"Shaders\vert.glsl", @"Shaders\frag.glsl");
            wallTexture = new Texture(@"resources\wall.png", TextureUnit.Texture0);
            faceTexture = new Texture(@"resources\awesomeface.png", TextureUnit.Texture1);

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
                
                wallTexture.Use();
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.MirroredRepeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.MirroredRepeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
                
                faceTexture.Use();
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

                shader.Use();
                shader.SetUniform("texture1", 0);
                shader.SetUniform("texture2", 1);

            }
            GL.BindVertexArray(0);

            //InitVR();

            base.OnLoad(e);
        }

        void InitVR() {
            EVRInitError initError = EVRInitError.None;
            OpenVR.Init(ref initError, EVRApplicationType.VRApplication_Scene);

            uint width = 0, height = 0;
            OpenVR.System.GetRecommendedRenderTargetSize(ref width, ref height);

            Console.WriteLine($"Recommended Render Target Size: {width}x{height}");
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            totalTime += RenderTime;
            
            GL.Clear(ClearBufferMask.ColorBufferBit);

            shader.SetUniform("time", (float)totalTime);
            shader.SetUniform("blend", blend);
            shader.SetUniform("transform", Matrix4.CreateRotationZ((float)totalTime) * Matrix4.CreateTranslation(new Vector3(x, y, 0)));

            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.Triangles, rectangleIndices.Length, DrawElementsType.UnsignedInt, 0);

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
