using OpenTK.Graphics.OpenGL;
using StbImageSharp;
using System.IO;

namespace TKGL {
    public class Texture {
        int handle;
        ImageResult image;
        TextureUnit unit;

        public Texture(string path, TextureUnit unit) {
            handle = GL.GenTexture();
            this.unit = unit;
            Use();

            image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void Use() {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, handle);
        }
    }
}
