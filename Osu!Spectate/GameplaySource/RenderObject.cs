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

namespace OsuSpectate.GameplaySource
{
    public interface RenderObject
    {
        public string getType();
    }
    public class RenderHitCircle : RenderObject
    {
        public OsuStandardHitCircle HitCircle;
        public RenderHitCircle(OsuStandardHitCircle h, OsuStandardReplay r)
        {
            HitCircle = h;
        }
        public override string getType()
        {
            return "HitCircle";
        }
    }
    public class RenderSlider : RenderObject
    {
        public OsuStandardSlider Slider;
        public float x;
        public float y;
        public float width;
        public float height;
        public int SliderBorderTexture;
        public RenderSlider(OsuStandardSlider s, OsuStandardReplay r)
        {
            Slider = s;

            Bitmap SliderBMP = new Bitmap(512, 384);
            Graphics SliderGFX = Graphics.FromImage(SliderBMP);
            SliderGFX.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            SliderGFX.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            SliderGFX.Clear(Color.Transparent);
            float cs = r.GetCSRadius();
            PointF[] flipped = new PointF[s.curve.getPoints().Length];
            for (int i = 0; i < flipped.Length; i++)
            {
                flipped[i] = new PointF(s.curve.getPoints()[i].X, 384.0f - s.curve.getPoints()[i].Y);
            }


            SliderGFX.DrawLines(new Pen(Color.White, 2.0f * cs), flipped);
            SliderGFX.FillEllipse(new SolidBrush(Color.White), s.curve.pointOnCurve(0.0f).X - cs, 384.0f - s.curve.pointOnCurve(0.0f).Y - cs, 2.0f * cs, 2.0f * cs);
            SliderGFX.FillEllipse(new SolidBrush(Color.White), s.curve.pointOnCurve(1.0f).X - cs, 384.0f - s.curve.pointOnCurve(1.0f).Y - cs, 2.0f * cs, 2.0f * cs);
            SliderGFX.DrawLines(new Pen(Color.Black, cs * 7.0f / 4.0f), flipped);
            SliderGFX.FillEllipse(new SolidBrush(Color.Black), s.curve.pointOnCurve(0.0f).X - cs * 7.0f / 8.0f, 384.0f - s.curve.pointOnCurve(0.0f).Y - cs * 7.0f / 8.0f, 2.0f * cs * 7.0f / 8.0f, 2.0f * cs * 7.0f / 8.0f);
            SliderGFX.FillEllipse(new SolidBrush(Color.Black), s.curve.pointOnCurve(1.0f).X - cs * 7.0f / 8.0f, 384.0f - s.curve.pointOnCurve(1.0f).Y - cs * 7.0f / 8.0f, 2.0f * cs * 7.0f / 8.0f, 2.0f * cs * 7.0f / 8.0f);



            int ID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, ID);
            BitmapData Data = SliderBMP.LockBits(new Rectangle(0, 0, SliderBMP.Width, SliderBMP.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Data.Width, Data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, Data.Scan0);
            SliderBMP.UnlockBits(Data);
            SliderGFX.ReleaseHdc(SliderGFX.GetHdc());
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            SliderBorderTexture = ID;
            SliderBMP.Dispose();
            SliderGFX.Flush();
            SliderGFX.Dispose();



        }
        public override string getType()
        {
            return "Slider";
        }
        public void kill()
        {
            GL.DeleteTexture(SliderBorderTexture);
        }
    }
    public class RenderSpinner : RenderObject
    {
        public override string getType()
        {
            return "Spinner";
        }
    }
}
