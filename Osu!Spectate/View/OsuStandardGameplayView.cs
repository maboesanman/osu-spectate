using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

using ReplayAPI;

using OsuSpectate;
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
        private float FadeInFunction(float f)
        {
            return 1.0f - (float)Math.Pow(f, 2.0f);
        }
        public void Draw(TimeSpan time, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight)
        {
            ReplayFrame frame = GameplayInput.GetReplayFrame(time);
            float CursorX;
            float CursorY;
            if ((GameplayInput.GetMods() & Mods.HardRock) == Mods.HardRock)
            {
                CursorX = Computation.XComputation(frame.X, OriginX, OriginY, width, height, windowWidth, windowHeight);
                CursorY = Computation.YComputation(384.0f-frame.Y, OriginX, OriginY, width, height, windowWidth, windowHeight);
            } else
            {
                CursorX = Computation.XComputation(frame.X, OriginX, OriginY, width, height, windowWidth, windowHeight);
                CursorY = Computation.YComputation(frame.Y, OriginX, OriginY, width, height, windowWidth, windowHeight);
            }
            float CursorWidth = Math.Abs(Computation.XComputation(20.0f, OriginX, OriginY, width, height, windowWidth, windowHeight) - Computation.XComputation(0.0f, OriginX, OriginY, width, height, windowWidth, windowHeight));
            float CursorHeight = Math.Abs(Computation.YComputation(20.0f, OriginX, OriginY, width, height, windowWidth, windowHeight) - Computation.YComputation(0.0f, OriginX, OriginY, width, height, windowWidth, windowHeight));

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
            GL.Vertex2(Computation.XComputation(0.0f,OriginX,OriginY,width,height,windowWidth,windowHeight),Computation.YComputation(0.0f, OriginX, OriginY, width, height, windowWidth, windowHeight));
            GL.TexCoord2(1, 1);
            GL.Vertex2(Computation.XComputation(512.0f,OriginX,OriginY,width,height,windowWidth,windowHeight),Computation.YComputation(0.0f, OriginX, OriginY, width, height, windowWidth, windowHeight));
            GL.TexCoord2(1, 0);
            GL.Vertex2(Computation.XComputation(512.0f,OriginX,OriginY,width,height,windowWidth,windowHeight),Computation.YComputation(384.0f, OriginX, OriginY, width, height, windowWidth, windowHeight));
            GL.TexCoord2(0, 0);
            GL.Vertex2(Computation.XComputation(0.0f,OriginX,OriginY,width,height,windowWidth,windowHeight),Computation.YComputation(384.0f, OriginX, OriginY, width, height, windowWidth, windowHeight));
            GL.End();
           
            //hit objects
    //        Console.WriteLine(GameplayInput.GetRenderList().Count);
            for (int i = GameplayInput.GetRenderList().Count - 1; i >= 0; i--)
            {
                switch (GameplayInput.GetRenderList().ElementAt(i).GetType())
                {
                    case ("HitCircle"):
                        OsuStandardHitCircle c = ((RenderHitCircle)GameplayInput.GetRenderList().ElementAt(i)).HitCircle;
                        GL.BindTexture(TextureTarget.Texture2D, Skin.GetHitCircle());
                        GL.Color4(1.0f, 0.5f, 1.0f, FadeInFunction((float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds)));
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 100.0f);
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(0, 1);
                        GL.Vertex2(Computation.XComputation(c.x - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(c.y - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 1);
                        GL.Vertex2(Computation.XComputation(c.x + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(c.y - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 0);
                        GL.Vertex2(Computation.XComputation(c.x + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(c.y + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(0, 0);
                        GL.Vertex2(Computation.XComputation(c.x - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(c.y + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.End();
                        
                        GL.BindTexture(TextureTarget.Texture2D, Skin.GetHitCircleOverlay());
                        GL.Color4(1.0f, 1.0f, 1.0f, FadeInFunction((float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds)));
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(0, 1);
                        GL.Vertex2(Computation.XComputation(c.x - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(c.y - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 1);
                        GL.Vertex2(Computation.XComputation(c.x + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(c.y - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 0);
                        GL.Vertex2(Computation.XComputation(c.x + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(c.y + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(0, 0);
                        GL.Vertex2(Computation.XComputation(c.x - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(c.y + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.End();

                        GL.BindTexture(TextureTarget.Texture2D, Skin.GetApproachCircle());
                        GL.Color4(1.0f, 0.5f, 1.0f, FadeInFunction((float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds)));
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(0, 1);
                        GL.Vertex2(Computation.XComputation(c.x - (GameplayInput.GetCSRadius() * 4.0f * (float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius()), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(c.y - (GameplayInput.GetCSRadius() * 4.0f * (float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius()), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 1);
                        GL.Vertex2(Computation.XComputation(c.x + (GameplayInput.GetCSRadius() * 4.0f * (float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius()), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(c.y - (GameplayInput.GetCSRadius() * 4.0f * (float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius()), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 0);
                        GL.Vertex2(Computation.XComputation(c.x + (GameplayInput.GetCSRadius() * 4.0f * (float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius()), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(c.y + (GameplayInput.GetCSRadius() * 4.0f * (float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius()), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(0, 0);
                        GL.Vertex2(Computation.XComputation(c.x - (GameplayInput.GetCSRadius() * 4.0f * (float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius()), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(c.y + (GameplayInput.GetCSRadius() * 4.0f * (float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius()), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.End();
                        break;
                    case ("Slider"):
                        RenderSlider rs = (RenderSlider)GameplayInput.GetRenderList().ElementAt(i);
                        OsuStandardSlider s = rs.Slider;
                        float opacity = FadeInFunction((float)(s.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds));
                        int n = (int)s.curve.getLength();
                        for (int j=0;j<n;j++)
                        {
                            PointF p = s.curve.pointOnCurve((float)j / (float)n);
                            GL.BindTexture(TextureTarget.Texture2D, Skin.GetHitCircle());
                            GL.Color4(1.0f, 0.5f, 1.0f, FadeInFunction((float)(s.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds)));
                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 100.0f);
                            GL.Begin(PrimitiveType.Quads);
                            GL.TexCoord2(0, 1);
                            GL.Vertex2(Computation.XComputation(p.X - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(p.Y - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                            GL.TexCoord2(1, 1);
                            GL.Vertex2(Computation.XComputation(p.X + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(p.Y - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                            GL.TexCoord2(1, 0);
                            GL.Vertex2(Computation.XComputation(p.X + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(p.Y + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                            GL.TexCoord2(0, 0);
                            GL.Vertex2(Computation.XComputation(p.X - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(p.Y + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                            GL.End();
                            
                            GL.BindTexture(TextureTarget.Texture2D, Skin.GetHitCircleOverlay());
                            GL.Color4(1.0f, 1.0f, 1.0f, FadeInFunction((float)(s.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds)));
                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 100.0f);
                            GL.Begin(PrimitiveType.Quads);
                            GL.TexCoord2(0, 1);
                            GL.Vertex2(Computation.XComputation(p.X - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(p.Y - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                            GL.TexCoord2(1, 1);
                            GL.Vertex2(Computation.XComputation(p.X + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(p.Y - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                            GL.TexCoord2(1, 0);
                            GL.Vertex2(Computation.XComputation(p.X + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(p.Y + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                            GL.TexCoord2(0, 0);
                            GL.Vertex2(Computation.XComputation(p.X - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(p.Y + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                            GL.End();

                        }
                        
                        GL.BindTexture(TextureTarget.Texture2D, Skin.GetHitCircle());
                        GL.Color4(1.0f, 0.5f, 1.0f, opacity);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 100.0f);
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(0, 1);
                        GL.Vertex2(Computation.XComputation(s.x - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(s.y - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 1);
                        GL.Vertex2(Computation.XComputation(s.x + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(s.y - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 0);
                        GL.Vertex2(Computation.XComputation(s.x + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(s.y + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(0, 0);
                        GL.Vertex2(Computation.XComputation(s.x - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(s.y + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.End();

                        GL.BindTexture(TextureTarget.Texture2D, Skin.GetHitCircleOverlay());
                        GL.Color4(1.0f, 1.0f, 1.0f, opacity);
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(0, 1);
                        GL.Vertex2(Computation.XComputation(s.x - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(s.y - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 1);
                        GL.Vertex2(Computation.XComputation(s.x + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(s.y - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 0);
                        GL.Vertex2(Computation.XComputation(s.x + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(s.y + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(0, 0);
                        GL.Vertex2(Computation.XComputation(s.x - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(s.y + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.End();
                        

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
    }
}
