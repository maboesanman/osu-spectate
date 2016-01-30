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
        private float BackgroundDim;
        private OsuStandardBeatmap beatmap;
        private BackgroundImageFitType FitType;
        private Color Tint;

        public SongBackgroundView(OsuStandardBeatmap map, float dim, Color tint, BackgroundImageFitType fit)
        {
            BackgroundDim = dim;
            beatmap = map;
            FitType = fit;
            Tint = Computation.ColorFromHSL(tint.GetHue(),tint.GetSaturation(),1.0f-BackgroundDim);
        }

        public void Draw(TimeSpan time, float x, float y, float width, float height, int windowWidth, int windowHeight)
        {
            GL.BindTexture(TextureTarget.Texture2D, beatmap.GetBackgroundTexture());
            GL.Color3(Tint.R/256.0f, Tint.G / 256.0f, Tint.B / 256.0f);
            GL.Disable(EnableCap.Blend);
            switch (FitType)
            {
                case (BackgroundImageFitType.STRETCH):
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
                    if(beatmap.GetBackgroundTextureWidth()*height*windowHeight<beatmap.GetBackgroundTextureHeight()*width*windowWidth)
                    {
                        //letterbox
                        float midx = width / 2.0f + x;
                        float dx = .5f * height * windowHeight / windowWidth * beatmap.GetBackgroundTextureWidth() / beatmap.GetBackgroundTextureHeight();
                        float midy = height / 2.0f + y;
                        float dy = height / 2.0f;
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(0, 1);
                        GL.Vertex2(midx - dx, midy - dy);
                        GL.TexCoord2(1, 1);
                        GL.Vertex2(midx + dx, midy - dy);
                        GL.TexCoord2(1, 0);
                        GL.Vertex2(midx + dx, midy + dy);
                        GL.TexCoord2(0, 0);
                        GL.Vertex2(midx - dx, midy + dy);
                        GL.End();
                    } else
                    {
                        //overscan
                        float midx = .5f;
                        float dx = .5f * 1.0f / (height * windowHeight / width / windowWidth * beatmap.GetBackgroundTextureWidth() / beatmap.GetBackgroundTextureHeight());
                        float midy = .5f;
                        float dy = .5f;
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(midx-dx, midy + dy);
                        GL.Vertex2(x, y);
                        GL.TexCoord2(midx + dx, midy + dy);
                        GL.Vertex2(x + width, y);
                        GL.TexCoord2(midx + dx, midy - dy);
                        GL.Vertex2(x + width, y + height);
                        GL.TexCoord2(midx - dx, midy - dy);
                        GL.Vertex2(x, y + height);
                        GL.End();
                    }
                    
                    break;
                case (BackgroundImageFitType.HORIZONTAL_FIT):
                    if (beatmap.GetBackgroundTextureWidth() * height * windowHeight < beatmap.GetBackgroundTextureHeight() * width * windowWidth)
                    {
                        //overscan
                        float midx = .5f;
                        float dx = .5f;
                        float midy = .5f;
                        float dy = .5f * height * windowHeight / width / windowWidth * beatmap.GetBackgroundTextureWidth() / beatmap.GetBackgroundTextureHeight();
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(midx - dx, midy + dy);
                        GL.Vertex2(x, y);
                        GL.TexCoord2(midx + dx, midy + dy);
                        GL.Vertex2(x + width, y);
                        GL.TexCoord2(midx + dx, midy - dy);
                        GL.Vertex2(x + width, y + height);
                        GL.TexCoord2(midx - dx, midy - dy);
                        GL.Vertex2(x, y + height);
                        GL.End();
                    }
                    else
                    {
                        //letterbox
                        float midx = width / 2.0f + x;
                        float dx = width / 2.0f;
                        float midy = height / 2.0f + y;
                        float dy = .5f * 1.0f / (windowHeight / width / windowWidth * beatmap.GetBackgroundTextureWidth() / beatmap.GetBackgroundTextureHeight());
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(0, 1);
                        GL.Vertex2(midx - dx, midy - dy);
                        GL.TexCoord2(1, 1);
                        GL.Vertex2(midx + dx, midy - dy);
                        GL.TexCoord2(1, 0);
                        GL.Vertex2(midx + dx, midy + dy);
                        GL.TexCoord2(0, 0);
                        GL.Vertex2(midx - dx, midy + dy);
                        GL.End();
                    }
                    break;
                case (BackgroundImageFitType.MINIMUM_FIT):
                    if (beatmap.GetBackgroundTextureWidth() * height * windowHeight < beatmap.GetBackgroundTextureHeight() * width * windowWidth)
                    {
                        //vertical
                        float midx = width / 2.0f + x;
                        float dx = .5f * height * windowHeight / windowWidth * beatmap.GetBackgroundTextureWidth() / beatmap.GetBackgroundTextureHeight();
                        float midy = height / 2.0f + y;
                        float dy = height / 2.0f;
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(0, 1);
                        GL.Vertex2(midx - dx, midy - dy);
                        GL.TexCoord2(1, 1);
                        GL.Vertex2(midx + dx, midy - dy);
                        GL.TexCoord2(1, 0);
                        GL.Vertex2(midx + dx, midy + dy);
                        GL.TexCoord2(0, 0);
                        GL.Vertex2(midx - dx, midy + dy);
                        GL.End();
                    }
                    else
                    {
                        //horizontal
                        float midx = width / 2.0f + x;
                        float dx = width / 2.0f;
                        float midy = height / 2.0f + y;
                        float dy = .5f * 1.0f / (windowHeight / width / windowWidth * beatmap.GetBackgroundTextureWidth() / beatmap.GetBackgroundTextureHeight());
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(0, 1);
                        GL.Vertex2(midx - dx, midy - dy);
                        GL.TexCoord2(1, 1);
                        GL.Vertex2(midx + dx, midy - dy);
                        GL.TexCoord2(1, 0);
                        GL.Vertex2(midx + dx, midy + dy);
                        GL.TexCoord2(0, 0);
                        GL.Vertex2(midx - dx, midy + dy);
                        GL.End();
                    }
                    break;
                case (BackgroundImageFitType.MAXIMUM_FIT)://not done
                    if (beatmap.GetBackgroundTextureWidth() * height * windowHeight < beatmap.GetBackgroundTextureHeight() * width * windowWidth)
                    {
                        //horizontal
                        float midx = .5f;
                        float dx = .5f;
                        float midy = .5f;
                        float dy = .5f * height * windowHeight / width / windowWidth * beatmap.GetBackgroundTextureWidth() / beatmap.GetBackgroundTextureHeight();
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(midx - dx, midy + dy);
                        GL.Vertex2(x, y);
                        GL.TexCoord2(midx + dx, midy + dy);
                        GL.Vertex2(x + width, y);
                        GL.TexCoord2(midx + dx, midy - dy);
                        GL.Vertex2(x + width, y + height);
                        GL.TexCoord2(midx - dx, midy - dy);
                        GL.Vertex2(x, y + height);
                        GL.End();
                    }
                    else
                    {
                        //vertical
                        float midx = .5f;
                        float dx = .5f * 1.0f / (height * windowHeight / width / windowWidth * beatmap.GetBackgroundTextureWidth() / beatmap.GetBackgroundTextureHeight());
                        float midy = .5f;
                        float dy = .5f;
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(midx - dx, midy + dy);
                        GL.Vertex2(x, y);
                        GL.TexCoord2(midx + dx, midy + dy);
                        GL.Vertex2(x + width, y);
                        GL.TexCoord2(midx + dx, midy - dy);
                        GL.Vertex2(x + width, y + height);
                        GL.TexCoord2(midx - dx, midy - dy);
                        GL.Vertex2(x, y + height);
                        GL.End();
                    }
                    break;
                default:
                    break;
            }
            GL.Enable(EnableCap.Blend);
        }
        public void SetReplay(OsuStandardReplay r, OsuStandardBeatmap b)
        {
            beatmap = b;
        }
        public void SetTint(float alpha, Color tint)
        {
            Tint = Computation.ColorFromHSL(tint.GetHue(), tint.GetSaturation(), alpha);
        }
        public void GenerateBackground(float width, float height, int windowWidth, int windowHeight)
        {

        }
    }
}
