using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

using OsuSpectate.Beatmap;
using OsuSpectate.GameplaySource;

namespace OsuSpectate.View
{
    public class SongBackgroundView : View
    {
        private int BackgroundTexture;
        private byte BackgroundDim;
        private int BackgroundDimTexture;
        private OsuStandardBeatmap beatmap;
        private BackgroundImageFitType FitType; 

        public SongBackgroundView(OsuStandardBeatmap map, byte dim, Color tint, BackgroundImageFitType fit)
        {
            BackgroundDim = dim;
            beatmap = map;
            FitType = fit;
            SetTint(BackgroundDim, tint);
            
        }

        public void Draw(TimeSpan time, float x, float y, float width, float height, int windowWidth, int windowHeight)
        {
            
            switch (FitType)
            {
                case (BackgroundImageFitType.STRETCH):
                    GL.BindTexture(TextureTarget.Texture2D, beatmap.GetBackgroundTexture());
                    GL.Begin(PrimitiveType.Quads);

                    GL.TexCoord2(0, 1);
                    GL.Vertex2(x, y);
                    GL.TexCoord2(1, 1);
                    GL.Vertex2(x + width, y);
                    GL.TexCoord2(1, 0);
                    GL.Vertex2(x + width, y + height);
                    GL.TexCoord2(0, 0);
                    GL.Vertex2(x, y + height);
                    GL.End();

                    GL.BindTexture(TextureTarget.Texture2D, BackgroundDimTexture);
                    GL.Begin(PrimitiveType.Quads);

                    GL.TexCoord2(0, 1);
                    GL.Vertex2(x, y);
                    GL.TexCoord2(1, 1);
                    GL.Vertex2(x + width, y);
                    GL.TexCoord2(1, 0);
                    GL.Vertex2(x + width, y + height);
                    GL.TexCoord2(0, 0);
                    GL.Vertex2(x, y + height);
                    GL.End();
                    break;
                case (BackgroundImageFitType.VERTICAL_FIT):
                    GL.BindTexture(TextureTarget.Texture2D, beatmap.GetBackgroundTexture());
                    GL.Begin(PrimitiveType.Quads);

                    GL.TexCoord2(.5f - (width * windowWidth * beatmap.GetBackgroundTextureHeight()) / (height * windowHeight * beatmap.GetBackgroundTextureWidth()), 1.0f);
                    GL.Vertex2(x + width / 2 - (height * windowHeight * beatmap.GetBackgroundTextureWidth()) / (windowWidth * beatmap.GetBackgroundTextureHeight()), y);
                    GL.TexCoord2(.5f + (width * windowWidth * beatmap.GetBackgroundTextureHeight()) / (height * windowHeight * beatmap.GetBackgroundTextureWidth()), 1.0f);
                    GL.Vertex2(x + width / 2 + (height * windowHeight * beatmap.GetBackgroundTextureWidth()) / (windowWidth * beatmap.GetBackgroundTextureHeight()), y);
                    GL.TexCoord2(.5f + (width * windowWidth * beatmap.GetBackgroundTextureHeight()) / (height * windowHeight * beatmap.GetBackgroundTextureWidth()), 0.0f);
                    GL.Vertex2(x + width / 2 + (height * windowHeight * beatmap.GetBackgroundTextureWidth()) / (windowWidth * beatmap.GetBackgroundTextureHeight()), y + height);
                    GL.TexCoord2(.5f - (width * windowWidth * beatmap.GetBackgroundTextureHeight()) / (height * windowHeight * beatmap.GetBackgroundTextureWidth()), 0.0f);
                    GL.Vertex2(x + width / 2 - (height * windowHeight * beatmap.GetBackgroundTextureWidth()) / (windowWidth * beatmap.GetBackgroundTextureHeight()), y + height);
                    GL.End();

                    GL.BindTexture(TextureTarget.Texture2D, BackgroundDimTexture);
                    GL.Begin(PrimitiveType.Quads);

                    GL.TexCoord2(0, 1);
                    GL.Vertex2(x, y);
                    GL.TexCoord2(1, 1);
                    GL.Vertex2(x + width, y);
                    GL.TexCoord2(1, 0);
                    GL.Vertex2(x + width, y + height);
                    GL.TexCoord2(0, 0);
                    GL.Vertex2(x, y + height);
                    GL.End();
                    break;
                case (BackgroundImageFitType.HORIZONTAL_FIT):
                    GL.BindTexture(TextureTarget.Texture2D, beatmap.GetBackgroundTexture());
                    GL.Begin(PrimitiveType.Quads);

                    GL.TexCoord2(0.0f, 1);
                    GL.Vertex2(x, y + height / 2 - (width * windowWidth * beatmap.GetBackgroundTextureHeight()) / (windowHeight * beatmap.GetBackgroundTextureWidth()));
                    GL.TexCoord2(0.0f, .5f - (height * windowHeight * beatmap.GetBackgroundTextureWidth()) / (width * windowWidth * beatmap.GetBackgroundTextureHeight()));
                    GL.Vertex2(x, y + height / 2 + (width * windowWidth * beatmap.GetBackgroundTextureHeight()) / (windowHeight * beatmap.GetBackgroundTextureWidth()));
                    GL.TexCoord2(1.0f, .5f - (height * windowHeight * beatmap.GetBackgroundTextureWidth()) / (width * windowWidth * beatmap.GetBackgroundTextureHeight()));
                    GL.Vertex2(x + width, y + height / 2 + (width * windowWidth * beatmap.GetBackgroundTextureHeight()) / (windowHeight * beatmap.GetBackgroundTextureWidth()));
                    GL.TexCoord2(1.0f, .5f + (height * windowHeight * beatmap.GetBackgroundTextureWidth()) / (width * windowWidth * beatmap.GetBackgroundTextureHeight()));
                    GL.Vertex2(x + width, y + height / 2 - (width * windowWidth * beatmap.GetBackgroundTextureHeight()) / (windowHeight * beatmap.GetBackgroundTextureWidth()));
                    GL.End();

                    GL.BindTexture(TextureTarget.Texture2D, BackgroundDimTexture);
                    GL.Begin(PrimitiveType.Quads);

                    GL.TexCoord2(0, 1);
                    GL.Vertex2(x, y);
                    GL.TexCoord2(1, 1);
                    GL.Vertex2(x + width, y);
                    GL.TexCoord2(1, 0);
                    GL.Vertex2(x + width, y + height);
                    GL.TexCoord2(0, 0);
                    GL.Vertex2(x, y + height);
                    GL.End();
                    break;
                case (BackgroundImageFitType.MINIMUM_FIT)://not done
                    GL.BindTexture(TextureTarget.Texture2D, beatmap.GetBackgroundTexture());
                    GL.Begin(PrimitiveType.Quads);

                    GL.TexCoord2(0.0f, 1);
                    GL.Vertex2(x, y + height / 2 - (width * windowWidth * beatmap.GetBackgroundTextureHeight()) / (windowHeight * beatmap.GetBackgroundTextureWidth()));
                    GL.TexCoord2(0.0f, .5f - (height * windowHeight * beatmap.GetBackgroundTextureWidth()) / (width * windowWidth * beatmap.GetBackgroundTextureHeight()));
                    GL.Vertex2(x, y + height / 2 + (width * windowWidth * beatmap.GetBackgroundTextureHeight()) / (windowHeight * beatmap.GetBackgroundTextureWidth()));
                    GL.TexCoord2(1.0f, .5f - (height * windowHeight * beatmap.GetBackgroundTextureWidth()) / (width * windowWidth * beatmap.GetBackgroundTextureHeight()));
                    GL.Vertex2(x + width, y + height / 2 + (width * windowWidth * beatmap.GetBackgroundTextureHeight()) / (windowHeight * beatmap.GetBackgroundTextureWidth()));
                    GL.TexCoord2(1.0f, .5f + (height * windowHeight * beatmap.GetBackgroundTextureWidth()) / (width * windowWidth * beatmap.GetBackgroundTextureHeight()));
                    GL.Vertex2(x + width, y + height / 2 - (width * windowWidth * beatmap.GetBackgroundTextureHeight()) / (windowHeight * beatmap.GetBackgroundTextureWidth()));
                    GL.End();

                    GL.BindTexture(TextureTarget.Texture2D, BackgroundDimTexture);
                    GL.Begin(PrimitiveType.Quads);

                    GL.TexCoord2(0, 1);
                    GL.Vertex2(x, y);
                    GL.TexCoord2(1, 1);
                    GL.Vertex2(x + width, y);
                    GL.TexCoord2(1, 0);
                    GL.Vertex2(x + width, y + height);
                    GL.TexCoord2(0, 0);
                    GL.Vertex2(x, y + height);
                    GL.End();
                    break;
                case (BackgroundImageFitType.MAXIMUM_FIT)://not done
                    GL.BindTexture(TextureTarget.Texture2D, beatmap.GetBackgroundTexture());
                    GL.Begin(PrimitiveType.Quads);

                    GL.TexCoord2(0.0f, 1);
                    GL.Vertex2(x, y + height / 2 - (width * windowWidth * beatmap.GetBackgroundTextureHeight()) / (windowHeight * beatmap.GetBackgroundTextureWidth()));
                    GL.TexCoord2(0.0f, .5f - (height * windowHeight * beatmap.GetBackgroundTextureWidth()) / (width * windowWidth * beatmap.GetBackgroundTextureHeight()));
                    GL.Vertex2(x, y + height / 2 + (width * windowWidth * beatmap.GetBackgroundTextureHeight()) / (windowHeight * beatmap.GetBackgroundTextureWidth()));
                    GL.TexCoord2(1.0f, .5f - (height * windowHeight * beatmap.GetBackgroundTextureWidth()) / (width * windowWidth * beatmap.GetBackgroundTextureHeight()));
                    GL.Vertex2(x + width, y + height / 2 + (width * windowWidth * beatmap.GetBackgroundTextureHeight()) / (windowHeight * beatmap.GetBackgroundTextureWidth()));
                    GL.TexCoord2(1.0f, .5f + (height * windowHeight * beatmap.GetBackgroundTextureWidth()) / (width * windowWidth * beatmap.GetBackgroundTextureHeight()));
                    GL.Vertex2(x + width, y + height / 2 - (width * windowWidth * beatmap.GetBackgroundTextureHeight()) / (windowHeight * beatmap.GetBackgroundTextureWidth()));
                    GL.End();

                    GL.BindTexture(TextureTarget.Texture2D, BackgroundDimTexture);
                    GL.Begin(PrimitiveType.Quads);

                    GL.TexCoord2(0, 1);
                    GL.Vertex2(x, y);
                    GL.TexCoord2(1, 1);
                    GL.Vertex2(x + width, y);
                    GL.TexCoord2(1, 0);
                    GL.Vertex2(x + width, y + height);
                    GL.TexCoord2(0, 0);
                    GL.Vertex2(x, y + height);
                    GL.End();
                    break;
                default:
                    break;
            }
        }
        public void SetReplay(OsuStandardReplay r, OsuStandardBeatmap b)
        {
            beatmap = b;
        }
        public void SetTint(byte alpha, Color tint)
        {
            BackgroundDim = alpha;
            Bitmap bmp = new Bitmap(1, 1);
            bmp.SetPixel(0, 0, Color.FromArgb(BackgroundDim, tint.R, tint.G, tint.B));
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            BackgroundDimTexture = id;
        }
    }
}
