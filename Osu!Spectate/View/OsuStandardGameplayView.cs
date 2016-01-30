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
using OsuSpectate.GameplayEngine;

namespace OsuSpectate.View
{
    public class OsuStandardGameplayView : View
    {
        OsuStandardBeatmap Beatmap;
        OsuStandardGameplayInput GameplayInput;
        OsuSkin Skin;
        AudioPlayer Audio;
        public Color CursorColor;

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
            CursorColor = Color.White;

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
            NameBmpGfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            NameBmpGfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            
            NameBmpGfx.Clear(Color.Transparent);
            NameBmpGfx.DrawString(GameplayInput.GetPlayerName(), NameFont, DrawBrush, new PointF(0.0F, 0.0F));
            NameTexture = ContentPipe.LoadTextureFromBitmap(NameBmp);
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
            float CursorX = Computation.XComputation(frame.X, OriginX, OriginY, width, height, windowWidth, windowHeight);
            float CursorY = Computation.YComputation(frame.Y, OriginX, OriginY, width, height, windowWidth, windowHeight);
            
            float CursorWidth = Math.Abs(Computation.XComputation(20.0f, OriginX, OriginY, width, height, windowWidth, windowHeight) - Computation.XComputation(0.0f, OriginX, OriginY, width, height, windowWidth, windowHeight));
            float CursorHeight = Math.Abs(Computation.YComputation(20.0f, OriginX, OriginY, width, height, windowWidth, windowHeight) - Computation.YComputation(0.0f, OriginX, OriginY, width, height, windowWidth, windowHeight));

            //player name
            float buffer = 40.0f;
            GL.BindTexture(TextureTarget.Texture2D, BackgroundDimTexture);
            GL.Color3(1.0f, 1.0f, 1.0f);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 1);
            GL.Vertex2(Computation.XComputation(-buffer, OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(-buffer, OriginX, OriginY, width, height, windowWidth, windowHeight));
            GL.TexCoord2(1, 1);
            GL.Vertex2(Computation.XComputation(512.0f + buffer, OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(-buffer, OriginX, OriginY, width, height, windowWidth, windowHeight));
            GL.TexCoord2(1, 0);
            GL.Vertex2(Computation.XComputation(512.0f + buffer, OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(384.0f + buffer, OriginX, OriginY, width, height, windowWidth, windowHeight));
            GL.TexCoord2(0, 0);
            GL.Vertex2(Computation.XComputation(-buffer, OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(384.0f + buffer, OriginX, OriginY, width, height, windowWidth, windowHeight));
            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, NameTexture);
            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(0, 1);
            GL.Vertex2(OriginX, OriginY+height-.2);
            GL.TexCoord2(0, 0);
            GL.Vertex2(OriginX, OriginY+height);
            GL.TexCoord2(1, 0);
            GL.Vertex2(OriginX + 1.0 * windowHeight / windowWidth, OriginY + height);
            GL.TexCoord2(1, 1);
            GL.Vertex2(OriginX + 1.0 * windowHeight / windowWidth, OriginY + height - .2);
            GL.End();
            
            
            //hit objects
    //        Console.WriteLine(GameplayInput.GetRenderList().Count);
            for (int i = GameplayInput.GetRenderList().Count - 1; i >= 0; i--)
            {
                switch (GameplayInput.GetRenderList().ElementAt(i).GetType())
                {
                    
                    case ("HitCircle"):
                        #region
                        OsuStandardHitCircle c = ((RenderHitCircle)GameplayInput.GetRenderList().ElementAt(i)).HitCircle;
                        float r = (Skin.getComboColors()[c.comboIndex % Skin.getComboColors().Length].R * 1.0f) / (255.0f);
                        float g = (Skin.getComboColors()[c.comboIndex % Skin.getComboColors().Length].G * 1.0f) / (255.0f);
                        float b = (Skin.getComboColors()[c.comboIndex % Skin.getComboColors().Length].B * 1.0f) / (255.0f);
                        GL.BindTexture(TextureTarget.Texture2D, Skin.GetHitCircle());
                        GL.Color4(r, g, b, FadeInFunction((float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds)));
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
                        GL.Color4(r, g, b, FadeInFunction((float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds)));
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(0, 1);
                        GL.Vertex2(Computation.XComputation(c.x - (GameplayInput.GetCSRadius() * 4.0f * (float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius(GameplayInput.GetMods())), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(c.y - (GameplayInput.GetCSRadius() * 4.0f * (float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius(GameplayInput.GetMods())), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 1);
                        GL.Vertex2(Computation.XComputation(c.x + (GameplayInput.GetCSRadius() * 4.0f * (float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius(GameplayInput.GetMods())), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(c.y - (GameplayInput.GetCSRadius() * 4.0f * (float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius(GameplayInput.GetMods())), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 0);
                        GL.Vertex2(Computation.XComputation(c.x + (GameplayInput.GetCSRadius() * 4.0f * (float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius(GameplayInput.GetMods())), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(c.y + (GameplayInput.GetCSRadius() * 4.0f * (float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius(GameplayInput.GetMods())), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(0, 0);
                        GL.Vertex2(Computation.XComputation(c.x - (GameplayInput.GetCSRadius() * 4.0f * (float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius(GameplayInput.GetMods())), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(c.y + (GameplayInput.GetCSRadius() * 4.0f * (float)(c.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius(GameplayInput.GetMods())), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.End();
                        break;
                    #endregion
                    case ("Slider"):
                        #region
                        RenderSlider rs = (RenderSlider)GameplayInput.GetRenderList().ElementAt(i);
                        OsuStandardSlider s = rs.Slider;
                        float opacity = 1.0f;
                        if(s.getStart()>time)
                        {
                            opacity = FadeInFunction((float)(s.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds));
                        }
                        
                        float red = (Skin.getComboColors()[s.comboIndex % Skin.getComboColors().Length].R * 1.0f) / (255.0f);
                        float green = (Skin.getComboColors()[s.comboIndex % Skin.getComboColors().Length].G * 1.0f) / (255.0f);
                        float blue = (Skin.getComboColors()[s.comboIndex % Skin.getComboColors().Length].B * 1.0f) / (255.0f);
                        PointF p;
                        if ((GameplayInput.GetMods() & Mods.HardRock) == Mods.HardRock)
                        {
                            GL.BindTexture(TextureTarget.Texture2D, rs.SliderBorderTexture.Value);
                            GL.Color4(1.0f, 1.0f, 1.0f, opacity);
                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 100.0f);
                            GL.Begin(PrimitiveType.Quads);
                            GL.TexCoord2(0, 1);
                            GL.Vertex2(Computation.XComputation(s.curve.minX - Beatmap.GetCSRadius(GameplayInput.GetMods()), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(s.curve.minY - Beatmap.GetCSRadius(GameplayInput.GetMods()), OriginX, OriginY, width, height, windowWidth, windowHeight));
                            GL.TexCoord2(1, 1);
                            GL.Vertex2(Computation.XComputation(s.curve.maxX + Beatmap.GetCSRadius(GameplayInput.GetMods()), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(s.curve.minY - Beatmap.GetCSRadius(GameplayInput.GetMods()), OriginX, OriginY, width, height, windowWidth, windowHeight));
                            GL.TexCoord2(1, 0);
                            GL.Vertex2(Computation.XComputation(s.curve.maxX + Beatmap.GetCSRadius(GameplayInput.GetMods()), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(s.curve.maxY + Beatmap.GetCSRadius(GameplayInput.GetMods()), OriginX, OriginY, width, height, windowWidth, windowHeight));
                            GL.TexCoord2(0, 0);
                            GL.Vertex2(Computation.XComputation(s.curve.minX - Beatmap.GetCSRadius(GameplayInput.GetMods()), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(s.curve.maxY + Beatmap.GetCSRadius(GameplayInput.GetMods()), OriginX, OriginY, width, height, windowWidth, windowHeight));
                            GL.End();
                        }
                        else
                        {
                            GL.BindTexture(TextureTarget.Texture2D, rs.SliderBorderTexture.Value);
                            GL.Color4(1.0f, 1.0f, 1.0f, opacity);
                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 100.0f);
                            GL.Begin(PrimitiveType.Quads);
                            GL.TexCoord2(0, 0);
                            GL.Vertex2(Computation.XComputation(s.curve.minX - Beatmap.GetCSRadius(GameplayInput.GetMods()), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(s.curve.minY - Beatmap.GetCSRadius(GameplayInput.GetMods()), OriginX, OriginY, width, height, windowWidth, windowHeight));
                            GL.TexCoord2(1, 0);
                            GL.Vertex2(Computation.XComputation(s.curve.maxX + Beatmap.GetCSRadius(GameplayInput.GetMods()), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(s.curve.minY - Beatmap.GetCSRadius(GameplayInput.GetMods()), OriginX, OriginY, width, height, windowWidth, windowHeight));
                            GL.TexCoord2(1, 1);
                            GL.Vertex2(Computation.XComputation(s.curve.maxX + Beatmap.GetCSRadius(GameplayInput.GetMods()), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(s.curve.maxY + Beatmap.GetCSRadius(GameplayInput.GetMods()), OriginX, OriginY, width, height, windowWidth, windowHeight));
                            GL.TexCoord2(0, 1);
                            GL.Vertex2(Computation.XComputation(s.curve.minX - Beatmap.GetCSRadius(GameplayInput.GetMods()), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(s.curve.maxY + Beatmap.GetCSRadius(GameplayInput.GetMods()), OriginX, OriginY, width, height, windowWidth, windowHeight));
                            GL.End();
                        }

                        if (s.getStart() > time)
                        {
                            
                            GL.BindTexture(TextureTarget.Texture2D, Skin.GetApproachCircle());
                            GL.Color4(red, green, blue, opacity);
                            GL.Begin(PrimitiveType.Quads);
                            GL.TexCoord2(0, 1);
                            GL.Vertex2(Computation.XComputation(s.x - (GameplayInput.GetCSRadius() * 4.0f * (float)(s.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius(GameplayInput.GetMods())), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(s.y - (GameplayInput.GetCSRadius() * 4.0f * (float)(s.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius(GameplayInput.GetMods())), OriginX, OriginY, width, height, windowWidth, windowHeight));
                            GL.TexCoord2(1, 1);
                            GL.Vertex2(Computation.XComputation(s.x + (GameplayInput.GetCSRadius() * 4.0f * (float)(s.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius(GameplayInput.GetMods())), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(s.y - (GameplayInput.GetCSRadius() * 4.0f * (float)(s.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius(GameplayInput.GetMods())), OriginX, OriginY, width, height, windowWidth, windowHeight));
                            GL.TexCoord2(1, 0);
                            GL.Vertex2(Computation.XComputation(s.x + (GameplayInput.GetCSRadius() * 4.0f * (float)(s.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius(GameplayInput.GetMods())), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(s.y + (GameplayInput.GetCSRadius() * 4.0f * (float)(s.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius(GameplayInput.GetMods())), OriginX, OriginY, width, height, windowWidth, windowHeight));
                            GL.TexCoord2(0, 0);
                            GL.Vertex2(Computation.XComputation(s.x - (GameplayInput.GetCSRadius() * 4.0f * (float)(s.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius(GameplayInput.GetMods())), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(s.y + (GameplayInput.GetCSRadius() * 4.0f * (float)(s.getStart().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds) + Beatmap.GetCSRadius(GameplayInput.GetMods())), OriginX, OriginY, width, height, windowWidth, windowHeight));
                            GL.End();
                        }
                            

                        

                        

                        p = s.curve.pointOnCurve(1.0f/s.repeat);
                        GL.BindTexture(TextureTarget.Texture2D, Skin.GetHitCircle());
                        GL.Color4(red, green, blue, opacity);
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
                        GL.Color4(1.0f, 1.0f, 1.0f, opacity);
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

                        GL.BindTexture(TextureTarget.Texture2D, Skin.GetHitCircle());
                        GL.Color4(red, green, blue, opacity);
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

                        try
                        {
                            if(time>s.getStart()&&time <s.getEnd())
                            {
                                float t = (time - s.getStart()).Ticks * 1.0f / ((s.getEnd() - s.getStart()).Ticks * 1.0f);

                                p = s.curve.pointOnCurve(t);
                                GL.BindTexture(TextureTarget.Texture2D, Skin.GetSliderFollowCircle());
                                GL.Color4(red, green, blue, opacity);
                                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 100.0f);
                                GL.Begin(PrimitiveType.Quads);
                                GL.TexCoord2(0, 1);
                                GL.Vertex2(Computation.XComputation(p.X - GameplayInput.GetCSRadius() * 2, OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(p.Y - GameplayInput.GetCSRadius() * 2, OriginX, OriginY, width, height, windowWidth, windowHeight));
                                GL.TexCoord2(1, 1);
                                GL.Vertex2(Computation.XComputation(p.X + GameplayInput.GetCSRadius() * 2, OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(p.Y - GameplayInput.GetCSRadius() * 2, OriginX, OriginY, width, height, windowWidth, windowHeight));
                                GL.TexCoord2(1, 0);
                                GL.Vertex2(Computation.XComputation(p.X + GameplayInput.GetCSRadius() * 2, OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(p.Y + GameplayInput.GetCSRadius() * 2, OriginX, OriginY, width, height, windowWidth, windowHeight));
                                GL.TexCoord2(0, 0);
                                GL.Vertex2(Computation.XComputation(p.X - GameplayInput.GetCSRadius() * 2, OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(p.Y + GameplayInput.GetCSRadius() * 2, OriginX, OriginY, width, height, windowWidth, windowHeight));
                                GL.End();

                                GL.BindTexture(TextureTarget.Texture2D, Skin.GetSliderBall()[0]);
                                GL.Color4(red, green, blue, opacity);
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
                        }
                        catch(DivideByZeroException)
                        {

                        }
                        
                        break;
                    #endregion
                    default:
                        break;
                }
            }
            for (int i = GameplayInput.GetRenderList().Count - 1; i >= 0; i--)
            {
                switch (GameplayInput.GetRenderList().ElementAt(i).GetType())
                {
                    case ("300"):
                        #region
                        Render300 r300 = (Render300)(GameplayInput.GetRenderList().ElementAt(i));

                        GL.BindTexture(TextureTarget.Texture2D, Skin.GetHit300()[0]);
                        GL.Color4(1.0f, 1.0f, 1.0f, FadeInFunction(-(float)(r300.GetStartTime().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds)));
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(0, 0);
                        GL.Vertex2(Computation.XComputation(r300.X - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(r300.Y - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 0);
                        GL.Vertex2(Computation.XComputation(r300.X + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(r300.Y - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 1);
                        GL.Vertex2(Computation.XComputation(r300.X + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(r300.Y + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(0, 1);
                        GL.Vertex2(Computation.XComputation(r300.X - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(r300.Y + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.End();
                        
                        break;
                        #endregion
                    case ("100"):
                        #region
                        Render100 r100 = (Render100)(GameplayInput.GetRenderList().ElementAt(i));

                        GL.BindTexture(TextureTarget.Texture2D, Skin.GetHit100()[0]);
                        GL.Color4(1.0f, 1.0f, 1.0f, FadeInFunction(-(float)(r100.GetStartTime().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds)));
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(0, 0);
                        GL.Vertex2(Computation.XComputation(r100.X - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(r100.Y - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 0);
                        GL.Vertex2(Computation.XComputation(r100.X + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(r100.Y - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 1);
                        GL.Vertex2(Computation.XComputation(r100.X + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(r100.Y + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(0, 1);
                        GL.Vertex2(Computation.XComputation(r100.X - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(r100.Y + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.End();
                        
                        break;
                        #endregion
                    case ("50"):
                        #region
                        Render50 r50 = (Render50)(GameplayInput.GetRenderList().ElementAt(i));

                        GL.BindTexture(TextureTarget.Texture2D, Skin.GetHit50()[0]);
                        GL.Color4(1.0f, 1.0f, 1.0f, FadeInFunction(-(float)(r50.GetStartTime().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds)));
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(0, 0);
                        GL.Vertex2(Computation.XComputation(r50.X - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(r50.Y - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 0);
                        GL.Vertex2(Computation.XComputation(r50.X + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(r50.Y - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 1);
                        GL.Vertex2(Computation.XComputation(r50.X + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(r50.Y + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(0, 1);
                        GL.Vertex2(Computation.XComputation(r50.X - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(r50.Y + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.End();
                        break;
                        #endregion
                    case ("Miss"):
                        #region
                        RenderMiss rMiss = (RenderMiss)(GameplayInput.GetRenderList().ElementAt(i));

                        GL.BindTexture(TextureTarget.Texture2D, Skin.GetHit0()[0]);
                        GL.Color4(1.0f, 1.0f, 1.0f, FadeInFunction(-(float)(rMiss.GetStartTime().Subtract(time).TotalMilliseconds / GameplayInput.GetARMilliseconds().TotalMilliseconds)));
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(0, 0);
                        GL.Vertex2(Computation.XComputation(rMiss.X - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(rMiss.Y - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 0);
                        GL.Vertex2(Computation.XComputation(rMiss.X + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(rMiss.Y - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(1, 1);
                        GL.Vertex2(Computation.XComputation(rMiss.X + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(rMiss.Y + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.TexCoord2(0, 1);
                        GL.Vertex2(Computation.XComputation(rMiss.X - GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight), Computation.YComputation(rMiss.Y + GameplayInput.GetCSRadius(), OriginX, OriginY, width, height, windowWidth, windowHeight));
                        GL.End();
                        
                        break;
                    #endregion
                    default:
                        break;
                }
            }

                //cursor
            
            GL.BindTexture(TextureTarget.Texture2D, Skin.GetCursor());
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(CursorColor);
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
