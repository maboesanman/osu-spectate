using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

using OsuSpectate.Beatmap;

namespace OsuSpectate.GameplaySource
{
    public interface RenderObject
    {
        string GetType();
        void Initialize(float x, float y, float width, float height, int windowWidth, int windowHeight);
    }
    
    public class RenderHitCircle : RenderObject
    {
        public OsuStandardHitCircle HitCircle;
        public OsuStandardGameplayInput GameplayInput;
        private bool Initialized;
        public RenderHitCircle(OsuStandardHitCircle h, OsuStandardGameplayInput r)
        {
            HitCircle = h;
            GameplayInput = r;
        }
        public void Initialize(float x, float y, float width, float height, int windowWidth, int windowHeight)
        {
            Initialized = true;
        }
        public string GetType()
        {
            return "HitCircle";
        }
    }
    public class RenderSlider : RenderObject
    {
        public OsuStandardSlider Slider;
        public int SliderBorderTexture;
        private bool Initialized;
        public RenderSlider(OsuStandardSlider s, OsuStandardGameplayInput r)
        {
            Slider = s;

            Bitmap SliderBMP = new Bitmap(512, 384);
            Graphics SliderGFX = Graphics.FromImage(SliderBMP);
            SliderGFX.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            SliderGFX.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            SliderGFX.Clear(Color.Transparent);
            float cs = r.GetCSRadius();

            Pen backPen = new Pen(Color.White, cs * 2.0f);
            backPen.LineJoin = LineJoin.Round;
            backPen.EndCap = LineCap.Round;
            backPen.StartCap = LineCap.Round;

            Pen frontPen = new Pen(Color.Black, cs * 1.75f);
            frontPen.LineJoin = LineJoin.Round;
            frontPen.EndCap = LineCap.Round;
            frontPen.StartCap = LineCap.Round;

            GraphicsPath path = new GraphicsPath();
            path.AddLines(s.curve.getPoints().ToArray());
            SliderGFX.DrawPath(backPen, path);
            SliderGFX.DrawPath(frontPen, path);
            


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
        public void Initialize(float x, float y, float width, float height, int windowWidth, int windowHeight)
        {

            Initialized = true;
        }
        public string GetType()
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
        public OsuStandardSpinner Spinner;
        public int SliderBorderTexture;
        private bool Initialized;
        public void Initialize(float x, float y, float width, float height, int windowWidth, int windowHeight)
        {

            Initialized = true;
        }
        public string GetType()
        {
            return "Spinner";
        }
        public void kill()
        {
            GL.DeleteTexture(SliderBorderTexture);
        }
    }
}
