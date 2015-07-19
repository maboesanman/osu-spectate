using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

using ReplayAPI;

using OsuSpectate.GameplaySource;
using OsuSpectate.Beatmap;
using OsuSpectate.Skin;
using OsuSpectate.Audio;

namespace OsuSpectate.View
{
    public class OsuStandardGameplayView : View
    {
        OsuStandardBeatmap Beatmap;
        OsuStandardGameplayInput GameplayInput;
        OsuSkin Skin;
        AudioPlayer Audio;

        //text drawing vars
        Bitmap NameBmp;
        Graphics NameBmpGfx;
        int NameTexture;
        Font NameFont;
        SolidBrush DrawBrush = new SolidBrush(Color.White);
        System.Drawing.Text.PrivateFontCollection PFC;
        int BackgroundDimTexture;

        //gameplay vars

        public OsuStandardGameplayView(OsuStandardBeatmap b, OsuStandardGameplayInput g, OsuSkin s, AudioPlayer a)
        {
            Beatmap = b;
            GameplayInput = g;
            Skin = s;
            Audio = a;

            Bitmap temp = new Bitmap(300, 300);
            for (int y = 0; y < 300; y++)
            {
                for (int x = 0; x < 300; x++)
                {
                    temp.SetPixel(y, x, Color.FromArgb(50, 0, 0, 0));
                }
            }
            PFC = new System.Drawing.Text.PrivateFontCollection();
            PFC.AddFontFile(@"Resources\Font_Exo_2\Exo2-Bold.otf");
            NameFont = new Font(PFC.Families[0], 60);
            BackgroundDimTexture = ContentPipe.LoadTextureFromBitmap(temp);

            NameBmp = new Bitmap(800, 160);
            NameBmpGfx = Graphics.FromImage(NameBmp);
            NameBmpGfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            NameBmpGfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            
            NameBmpGfx.Clear(Color.Transparent);
            NameBmpGfx.DrawString(GameplayInput.GetPlayerName(), NameFont, DrawBrush, new PointF(0.0F, 0.0F));
            ContentPipe.overwriteTextureBitmap(NameTexture, NameBmp);
            NameBmp.Dispose();
            NameBmpGfx.ReleaseHdc(NameBmpGfx.GetHdc());
            NameBmpGfx.Flush();
            NameBmpGfx.Dispose();
        }
        private float XComputation(float x, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight)
        {
            if (windowWidth*width / windowHeight/height < 10.0f / 9.0f)
            {
                return (x / 512.0f * width * windowWidth) / windowWidth + OriginX;
            }
            else
            {
                return (width * windowWidth / 2.0f - height * windowHeight * 5.0f / 9.0f + (x/512.0f) * height * windowHeight * 5.0f / 4.5f) / windowWidth + OriginX;
            }
        }
        private float YComputation(float y, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight)
        {
            if (windowWidth*width / windowHeight/height < 10.0f / 9.0f)
            {
                return (height * windowHeight / 2.0f - width * windowWidth * 3.0f / 8.0f + (1.0f - y / 384.0f) * width * windowWidth * 3.0f / 4.0f) / windowHeight + OriginY;
            }
            else
            {
                return (height * windowHeight / 2.0f - height * windowHeight * 5.0f / 12.0f + (1.0f-y/384.0f) * height * windowHeight * 5.0f / 6.0f) / windowHeight + OriginY;
            }
        }
        private float FadeInFunction(float f)
        {
            return 1.0f - (float)Math.Pow(f, 2.0f);
        }
        public void Draw(TimeSpan time, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight)
        {
            ReplayFrame tick = GameplayInput.GetReplayFrame(time);
            float CursorX = XComputation(tick.X, OriginX, OriginY, width, height, windowWidth, windowHeight);
            float CursorY = YComputation(tick.Y, OriginX, OriginY, width, height, windowWidth, windowHeight);
            float CursorWidth = Math.Abs(XComputation(20.0f, OriginX, OriginY, width, height, windowWidth, windowHeight) - XComputation(0.0f, OriginX, OriginY, width, height, windowWidth, windowHeight));
            float CursorHeight = Math.Abs(YComputation(20.0f, OriginX, OriginY, width, height, windowWidth, windowHeight) - YComputation(0.0f, OriginX, OriginY, width, height, windowWidth, windowHeight));

            //player name
            
            

            GL.BindTexture(TextureTarget.Texture2D, NameTexture);
            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(0, 1);
            GL.Vertex2(OriginX, OriginY+height-.1);
            GL.TexCoord2(0, 0);
            GL.Vertex2(OriginX, OriginY+height);
            GL.TexCoord2(1, 0);
            GL.Vertex2(OriginX+.5*windowHeight/windowWidth, OriginY +height);
            GL.TexCoord2(1, 1);
            GL.Vertex2(OriginX + .5 * windowHeight / windowWidth, OriginY + height - .1);
            GL.End();
            
            GL.BindTexture(TextureTarget.Texture2D, BackgroundDimTexture);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 1);
            GL.Vertex2(XComputation(0.0f,OriginX,OriginY,width,height,windowWidth,windowHeight),YComputation(0.0f, OriginX, OriginY, width, height, windowWidth, windowHeight));
            GL.TexCoord2(1, 1);
            GL.Vertex2(XComputation(512.0f,OriginX,OriginY,width,height,windowWidth,windowHeight),YComputation(0.0f, OriginX, OriginY, width, height, windowWidth, windowHeight));
            GL.TexCoord2(1, 0);
            GL.Vertex2(XComputation(512.0f,OriginX,OriginY,width,height,windowWidth,windowHeight),YComputation(384.0f, OriginX, OriginY, width, height, windowWidth, windowHeight));
            GL.TexCoord2(0, 0);
            GL.Vertex2(XComputation(0.0f,OriginX,OriginY,width,height,windowWidth,windowHeight),YComputation(384.0f, OriginX, OriginY, width, height, windowWidth, windowHeight));
            GL.End();
           
            //hit objects

            for (int i = GameplayInput.GetRenderList().Count - 1; i >= 0; i--)
            {
                switch (GameplayInput.GetRenderList().ElementAt(i).getType())
                {
                    case ("HitCircle"):

                        OsuStandardHitCircle c = ((RenderHitCircle)GameplayInput.GetRenderList().ElementAt(i)).HitCircle;
                        GL.BindTexture(TextureTarget.Texture2D, Skin.GetHitCircle());
                        GL.Color4(1.0f, 0.5f, 1.0f, FadeInFunction((float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds)));
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 100.0f);
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(0, 1);
                        GL.Vertex2(XComputation(c.x - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), YComputation(c.y - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 1);
                        GL.Vertex2(XComputation(c.x + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), YComputation(c.y - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 0);
                        GL.Vertex2(XComputation(c.x + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), YComputation(c.y + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(0, 0);
                        GL.Vertex2(XComputation(c.x - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), YComputation(c.y + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.End();

                        GL.BindTexture(TextureTarget.Texture2D, Skin.GetHitCircleOverlay());
                        GL.Color4(1.0f, 1.0f, 1.0f, FadeInFunction((float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds)));
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(0, 1);
                        GL.Vertex2(XComputation(c.x - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), YComputation(c.y - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 1);
                        GL.Vertex2(XComputation(c.x + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), YComputation(c.y - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 0);
                        GL.Vertex2(XComputation(c.x + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), YComputation(c.y + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(0, 0);
                        GL.Vertex2(XComputation(c.x - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), YComputation(c.y + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.End();

                        GL.BindTexture(TextureTarget.Texture2D, Skin.GetApproachCircle());
                        GL.Color4(1.0f, 0.5f, 1.0f, FadeInFunction((float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds)));
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(0, 1);
                        GL.Vertex2(XComputation(c.x - (GameplayInput.GetCSRadius() * 4.0f * (float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius()), OriginX, OriginY, width, height, windowWidth, windowHeight), YComputation(c.y - (GameplayInput.GetCSRadius() * 4.0f * (float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius()), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 1);
                        GL.Vertex2(XComputation(c.x + (GameplayInput.GetCSRadius() * 4.0f * (float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius()), OriginX, OriginY, width, height, windowWidth, windowHeight), YComputation(c.y - (GameplayInput.GetCSRadius() * 4.0f * (float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius()), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 0);
                        GL.Vertex2(XComputation(c.x + (GameplayInput.GetCSRadius() * 4.0f * (float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius()), OriginX, OriginY, width, height, windowWidth, windowHeight), YComputation(c.y + (GameplayInput.GetCSRadius() * 4.0f * (float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius()), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(0, 0);
                        GL.Vertex2(XComputation(c.x - (GameplayInput.GetCSRadius() * 4.0f * (float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius()), OriginX, OriginY, width, height, windowWidth, windowHeight), YComputation(c.y + (GameplayInput.GetCSRadius() * 4.0f * (float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius()), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.End();
                        break;
                    case ("Slider"):
                        RenderSlider rs = (RenderSlider)GameplayInput.GetRenderList().ElementAt(i);
                        OsuStandardSlider s = rs.Slider;
                        float opacity = FadeInFunction((float)(s.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds));
                        
                        GL.BindTexture(TextureTarget.Texture2D, rs.SliderBorderTexture);
                        GL.Color4(1.0f, 1.0f, 1.0f, opacity);
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(0, 1);
                        GL.Vertex2(XComputation(0.0f, OriginX, OriginY, width, height, windowWidth, windowHeight), YComputation(0.0f, OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 1);
                        GL.Vertex2(XComputation(512.0f, OriginX, OriginY, width, height, windowWidth, windowHeight), YComputation(0.0f, OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 0);
                        GL.Vertex2(XComputation(512.0f, OriginX, OriginY, width, height, windowWidth, windowHeight), YComputation(384.0f, OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(0, 0);
                        GL.Vertex2(XComputation(0.0f, OriginX, OriginY, width, height, windowWidth, windowHeight), YComputation(384.0f, OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.End();
                        /*
                        GL.BindTexture(TextureTarget.Texture2D, skin.GetHitCircle());
                        GL.Color4(1.0f, 0.5f, 1.0f, opacity);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 100.0f);
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(0, 1);
                        GL.Vertex2(xComputation(s.x - replay.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), yComputation(s.y - replay.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 1);
                        GL.Vertex2(xComputation(s.x + replay.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), yComputation(s.y - replay.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 0);
                        GL.Vertex2(xComputation(s.x + replay.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), yComputation(s.y + replay.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(0, 0);
                        GL.Vertex2(xComputation(s.x - replay.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), yComputation(s.y + replay.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.End();

                        GL.BindTexture(TextureTarget.Texture2D, skin.GetHitCircleOverlay());
                        GL.Color4(1.0f, 1.0f, 1.0f, opacity);
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(0, 1);
                        GL.Vertex2(xComputation(s.x - replay.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), yComputation(s.y - replay.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 1);
                        GL.Vertex2(xComputation(s.x + replay.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), yComputation(s.y - replay.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 0);
                        GL.Vertex2(xComputation(s.x + replay.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), yComputation(s.y + replay.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(0, 0);
                        GL.Vertex2(xComputation(s.x - replay.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), yComputation(s.y + replay.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.End();
                        */

                        break;
                }

                
            }

                //cursor

                GL.BindTexture(TextureTarget.Texture2D, Skin.GetCursor());
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.White);
            GL.TexCoord2(0, 0);
            GL.Vertex2(CursorX - CursorWidth, CursorY - CursorHeight);
            GL.TexCoord2(1, 0);
            GL.Vertex2(CursorX - CursorWidth, CursorY + CursorHeight);
            GL.TexCoord2(1, 1);
            GL.Vertex2(CursorX + CursorWidth, CursorY + CursorHeight);
            GL.TexCoord2(0, 1);
            GL.Vertex2(CursorX + CursorWidth, CursorY - CursorHeight);
            GL.End();

            //keys pressed

            
        }
        public void setReplay(OsuStandardReplay r, OsuStandardBeatmap b)
        {
            Beatmap = b;
            GameplayInput = r;
        }
    }
}
