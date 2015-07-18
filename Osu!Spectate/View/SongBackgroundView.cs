using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using System.Drawing;

using OsuSpectate.Beatmap;
using OsuSpectate.GameplaySource;

namespace OsuSpectate.View
{
    public class SongBackgroundView : View
    {
        private byte BackgroundDim;
        private int BackgroundDimTexture;
        private OsuStandardBeatmap beatmap;
        private int FitType; //0 = stretch, 1 = vertical fit, 2 = horizontal fit
        private int width;
        private int height;

        public SongBackgroundView(OsuStandardBeatmap map, int width, int height, byte dim, Color tint, int fit)
        {
            BackgroundDim = dim;
            beatmap = map;
            FitType = fit;
            Bitmap temp = new Bitmap(height, width);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    temp.SetPixel(y, x, Color.FromArgb(BackgroundDim, tint.R, tint.G, tint.B));
                }
            }

            BackgroundDimTexture = ContentPipe.LoadTextureFromBitmap(temp);
        }

        public void Draw(TimeSpan time, float x, float y, float width, float height, int windowWidth, int windowHeight)
        {
            switch (FitType)
            {
                case (0):
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
                case (1):
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
                case (2):
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
        public void setReplay(OsuStandardReplay r, OsuStandardBeatmap b)
        {
            beatmap = b;
        }
        public void setTint(byte alpha, Color tint)
        {
            Bitmap temp = new Bitmap(height, width);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    temp.SetPixel(y, x, Color.FromArgb(BackgroundDim, tint.R, tint.G, tint.B));
                }
            }
            BackgroundDimTexture = ContentPipe.LoadTextureFromBitmap(temp);
        }
    }
}
