
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

using System.Drawing;
using Splat;
#if __MOBILE__
using OpenTK.Graphics.ES20;
#endif

#if __WINDOWS__
using OpenTK.Graphics.OpenGL;
using System.Windows.Media.Imaging;
using System.Windows;
#endif

#if __ANDROID__
using Android.Graphics;
using Android.Graphics.Drawables;
#endif

#if __IOS__
using CoreGraphics;
using UIKit;
using Foundation;
#endif

namespace OsuSpectate
{
    public class ContentPipe
    {
        //returns <id,width,height>
        public static Tuple<int,int,int> LoadTextureFromPath(string path)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);
#if __WINDOWS__
            Bitmap bmp = new Bitmap(path);
            int width = bmp.Width;
            int height = bmp.Height;
            System.Drawing.Imaging.BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
#endif
#if __IOS__
            NSData texData = NSData.FromFile(path);
            UIImage image = UIImage.LoadFromData(texData);
            int width = (int)image.CGImage.Width;
			int height = (int)image.CGImage.Height;

			CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB ();
			byte [] imageData = new byte[height * width * 4];
			CGContext context = new CGBitmapContext  (imageData, width, height, 8, 4 * width, colorSpace,
			                                          CGBitmapFlags.PremultipliedLast | CGBitmapFlags.ByteOrder32Big);

			context.TranslateCTM (0, height);
			context.ScaleCTM (1, -1);
			colorSpace.Dispose ();
			context.ClearRect (new CGRect (0, 0, width, height));
			context.DrawImage (new CGRect (0, 0, width, height), image.CGImage);

            GL.TexImage2D (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, (int)width, (int)height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, imageData);
			context.Dispose ();
#endif
#if __ANDROID__
            var image = BitmapFactory.DecodeFile(path);
            int width = image.Width;
			int height = image.Height;
            Android.Opengl.GLUtils.TexImage2D(id, 0, image, 0);
            image.Recycle();
#endif
#if __MOBILE__
            GL.GenerateMipmap(TextureTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
#endif
            
            return new Tuple<int,int,int>(id,width,height);
        }
        public static Tuple<int, int, int> LoadTextureFromLocalPath(string path)
        {
            string directory = "";
            return LoadTextureFromPath(System.IO.Path.Combine(directory, path));
        }

        public static Tuple<int, int, int> LoadTextureFromBitmap(IBitmap bitmap)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

#if __WINDOWS__
            var source = bitmap.ToNative();
            Bitmap bmp = new Bitmap(source.PixelWidth,source.PixelHeight,System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            System.Drawing.Imaging.BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            source.CopyPixels(Int32Rect.Empty,data.Scan0,data.Height * data.Stride,data.Stride);
            int width = bmp.Width;
            int height = bmp.Height;
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
#endif
#if __IOS__
            UIImage image = bitmap.ToNative();
            int width = (int)image.CGImage.Width;
            int height = (int)image.CGImage.Height;

            CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();
            byte[] imageData = new byte[height * width * 4];
            CGContext context = new CGBitmapContext(imageData, width, height, 8, 4 * width, colorSpace,
                                                      CGBitmapFlags.PremultipliedLast | CGBitmapFlags.ByteOrder32Big);

            context.TranslateCTM(0, height);
            context.ScaleCTM(1, -1);
            colorSpace.Dispose();
            context.ClearRect(new CGRect(0, 0, width, height));
            context.DrawImage(new CGRect(0, 0, width, height), image.CGImage);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, (int)width, (int)height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, imageData);
            context.Dispose();
#endif
#if __ANDROID__
            Bitmap image = ((BitmapDrawable)bitmap.ToNative()).Bitmap;
            int width = image.Width;
			int height = image.Height;
            Android.Opengl.GLUtils.TexImage2D(id, 0, image, 0);
            image.Recycle();
#endif
#if __MOBILE__
            GL.GenerateMipmap(TextureTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
#endif

            return new Tuple<int, int, int>(id, width, height);
        }
        /*
        public static void overwriteTextureBitmap(int id, Image image)
        {
            Bitmap bmp = new Bitmap(image);
            GL.BindTexture(TextureTarget.Texture2D, id);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }
        */
    }
}
