using OpenTK.Graphics.OpenGL;
using StbImageSharp;
using System.IO;

namespace TKGL {
    public class Texture {
        int handle;
        ImageResult image;

        public Texture(string path) {
            handle = GL.GenTexture();
            Use();

            image = ImageResult.FromStream(File.OpenRead(path));
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, image.Data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void Use() {
            GL.BindTexture(TextureTarget.Texture2D, handle);
        }
    }
}
