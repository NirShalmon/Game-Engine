using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;

namespace GameEngine
{
    /// <summary>
    /// An image to be shown on objects.
    /// </summary>
    public struct Texture
    {
        internal int id {  get; }

        /// <summary>
        /// The width of this texture in pixels.
        /// </summary>
        public int width {  get; }

        /// <summary>
        /// The height of this texture in pixels.
        /// </summary>
        public int height {  get; }

        /// <summary>
        /// The size of this texture in pixels.
        /// </summary>
        public Size resolution {
            get {
                return new Size(width,height);
            }
        }

        /// <summary>
        /// Construct a texture from a image at a path.
        /// </summary>
        /// <param name="path">The path of the image.</param>
        public Texture(string path) {
            if(string.IsNullOrEmpty(path)) {
                throw new ArgumentException(path);
            }

            id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D,id);

            // We will not upload mipmaps, so disable mipmapping (otherwise the texture will not appear).
            // We can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
            // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
            GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureMinFilter,(int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureMagFilter,(int)TextureMagFilter.Linear);

            Bitmap bmp = new Bitmap(path);
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0,0,bmp.Width,bmp.Height),ImageLockMode.ReadOnly,System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D,0,PixelInternalFormat.Rgba,bmp_data.Width,bmp_data.Height,0,OpenTK.Graphics.OpenGL.PixelFormat.Bgra,PixelType.UnsignedByte,bmp_data.Scan0);

            width = bmp_data.Width;
            height = bmp_data.Height;
            bmp.UnlockBits(bmp_data);
        }
    }
}
