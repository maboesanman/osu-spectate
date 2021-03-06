﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Diagnostics;

using osuElements;
using osuElements.Beatmaps;

namespace OsuSpectate.Beatmap
{
    public class OsuStandardBeatmap : osuElements.Beatmaps.Beatmap
    {
        int BackgroundTexture;
        int BackgroundTextureWidth;
        int BackgroundTextureHeight;
        BeatmapManager Manager;
        
        Dictionary<Slider, TextureContainer> sliderTexturesNomod;
        Dictionary<Slider, TextureContainer> sliderTexturesHardrock;
        Dictionary<Slider, TextureContainer> sliderTexturesEasy;
        

        public OsuStandardBeatmap(string path) : base(path)
        {
            
            
            BackgroundTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, BackgroundTexture);
            Bitmap bmp = new Bitmap(Path.Combine(Directory, Background.FilePath));
            BackgroundTextureHeight = bmp.Height;
            BackgroundTextureWidth = bmp.Width;
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            sliderTexturesNomod = new Dictionary<Slider, TextureContainer>(0);
            sliderTexturesHardrock = new Dictionary<Slider, TextureContainer>(0);
            sliderTexturesEasy = new Dictionary<Slider, TextureContainer>(0);

            
            //var SliderCalculations = Manager.SliderCalculations();
            //SliderCalculations.Start();
            //while (!SliderCalculations.IsCompleted) { }

            Console.WriteLine("finished loading beatmap: " + Title);
        }
        public TextureContainer GetSliderTexture(Slider slider, osuElements.Helpers.Mods mods)
        {
            if (mods.HasFlag(osuElements.Helpers.Mods.HardRock))
            {
                if (!sliderTexturesHardrock.ContainsKey(slider))
                {
                    sliderTexturesHardrock[slider] = GenerateSliderTexture(slider, mods);
                }
                return sliderTexturesHardrock[slider];
            }
            if (mods.HasFlag(osuElements.Helpers.Mods.Easy))
            {
                if (!sliderTexturesEasy.ContainsKey(slider))
                {
                    sliderTexturesEasy[slider] = GenerateSliderTexture(slider, mods);
                }
                return sliderTexturesEasy[slider];
            }
            if (!sliderTexturesNomod.ContainsKey(slider))
            {
                sliderTexturesNomod[slider] = GenerateSliderTexture(slider, mods);
            }
            return sliderTexturesNomod[slider];
        }
        public TextureContainer GenerateSliderTexture(Slider slider, osuElements.Helpers.Mods mods)
        {
            
            float scale = 1.5f;
            
            Tuple<osuElements.Position, float>[] positions = slider.GetAllCurvePoints(1);
            PointF[] points = new PointF[positions.Length];
            float maxX = 0.0f;
            float minX = float.MaxValue;
            float maxY = 0.0f;
            float minY = float.MaxValue;

            for(int i=0;i<positions.Length;i++)
            {
                points[i] = new PointF(positions.ElementAt(i).Item1.XForHitobject, positions.ElementAt(i).Item1.YForHitobject);

                minX = Math.Min(minX, positions.ElementAt(i).Item1.XForHitobject);
                maxX = Math.Max(maxX, positions.ElementAt(i).Item1.XForHitobject);
                minY = Math.Min(minY, positions.ElementAt(i).Item1.YForHitobject);
                maxY = Math.Max(maxY, positions.ElementAt(i).Item1.YForHitobject);
            }
            
            Bitmap SliderBMP = new Bitmap((int)(((maxX-minX)+2.0f*GetCSRadius(mods)) * scale), (int)(((maxY - minY) + 2.0f * GetCSRadius(mods)) * scale));
            Graphics SliderGFX = Graphics.FromImage(SliderBMP);
            SliderGFX.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            SliderGFX.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            SliderGFX.Clear(Color.Transparent);
            float cs = GetCSRadius(mods);

            Pen backPen = new Pen(Color.White, cs * 1.8f * scale);
            backPen.LineJoin = LineJoin.Round;
            backPen.EndCap = LineCap.Round;
            backPen.StartCap = LineCap.Round;

            Pen frontPen = new Pen(Color.Black, cs * 1.6f * scale);
            frontPen.LineJoin = LineJoin.Round;
            frontPen.EndCap = LineCap.Round;
            frontPen.StartCap = LineCap.Round;

            GraphicsPath path = new GraphicsPath();

            
            path.AddLines(points);
            Matrix m = new Matrix();
            m.Scale(scale, scale);
            m.Translate(-(minX - GetCSRadius(mods)), -(minY - GetCSRadius(mods)));
            path.Transform(m);
            SliderGFX.DrawPath(backPen, path);
            SliderGFX.DrawPath(frontPen, path);
            SliderBMP.MakeTransparent(Color.Black);
            BitmapData Data = SliderBMP.LockBits(new Rectangle(0, 0, SliderBMP.Width, SliderBMP.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            SliderBMP.UnlockBits(Data);
            SliderGFX.ReleaseHdc(SliderGFX.GetHdc());
            SliderGFX.Flush();
            SliderGFX.Dispose();
            
            int ID = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, ID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Data.Width, Data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, Data.Scan0);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            


            SliderBMP.Dispose();

            return new TextureContainer(ID, (int)minX, (int)maxX, (int)minY, (int)maxY);
        }
        public TimeSpan GetOD300Milliseconds(osuElements.Helpers.Mods mods)
        {
            if ((mods & osuElements.Helpers.Mods.HardRock) == osuElements.Helpers.Mods.HardRock)
            {
                return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * (78.0f - Math.Min(Diff_Overall * 1.4f, 10.0f) * 6.0f)));
            }
            if ((mods & osuElements.Helpers.Mods.Easy) == osuElements.Helpers.Mods.Easy)
            {
                return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * (78.0f - Diff_Overall * 0.5f * 6.0f)));
            }
            return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * (78.0f - Diff_Overall * 6.0f)));
        }
        public TimeSpan GetOD100Milliseconds(osuElements.Helpers.Mods mods)
        {
            if ((mods & osuElements.Helpers.Mods.HardRock) == osuElements.Helpers.Mods.HardRock)
            {
                return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * (138.0f - Math.Min(Diff_Overall * 1.4f, 10.0f) * 8.0f)));
            }
            if ((mods & osuElements.Helpers.Mods.Easy) == osuElements.Helpers.Mods.Easy)
            {
                return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * (138.0f - Diff_Overall * 0.5f * 8.0f)));
            }
            return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * (138.0f - Diff_Overall * 8.0f)));
        }
        public TimeSpan GetOD50Milliseconds(osuElements.Helpers.Mods mods)
        {
            if ((mods & osuElements.Helpers.Mods.HardRock) == osuElements.Helpers.Mods.HardRock)
            {
                return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * (198.0f - Math.Min(Diff_Overall*1.4f,10.0f) * 10.0f)));
            }
            if ((mods & osuElements.Helpers.Mods.Easy) == osuElements.Helpers.Mods.Easy)
            {
                return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * (198.0f - Diff_Overall*0.5f * 10.0f)));
            }
            return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * (198.0f - Diff_Overall * 10.0f)));
        }
        public TimeSpan GetARMilliseconds(osuElements.Helpers.Mods mods)
        {
            if ((mods & osuElements.Helpers.Mods.HardRock) == osuElements.Helpers.Mods.HardRock)
            {
                return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * ((1800.0f - (Math.Min(Diff_Approach * 1.4f, 10.0f)) * 120.0f) - (Math.Max((Math.Min(Diff_Approach * 1.4f, 10.0f) - 5.0f) * 30.0f, 0.0f)))));
            }
            if ((mods & osuElements.Helpers.Mods.Easy) == osuElements.Helpers.Mods.Easy)
            {
                return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * ((1800.0f - (Math.Min(Diff_Approach * 0.5f, 10.0f)) * 120.0f) - (Math.Max((Diff_Approach * 0.5f - 5.0f) * 30.0f, 0.0f)))));
            }
            return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * ((1800.0f - (Diff_Approach) * 120.0f) - (Math.Max((Diff_Approach - 5.0f) * 30.0f, 0.0f)))));
        }
        public float GetCSRadius(osuElements.Helpers.Mods mods)
        {
            if ((mods & osuElements.Helpers.Mods.HardRock) == osuElements.Helpers.Mods.HardRock)
            {
                return (512 / 16.0f) * (1.0f - 0.7f * (Math.Min(Diff_Size * 1.3f, 10.0f) - 5.0f) / 5.0f);
            }
            if ((mods & osuElements.Helpers.Mods.Easy) == osuElements.Helpers.Mods.Easy)
            {
                return (512 / 16.0f) * (1.0f - 0.7f * (Diff_Size * 0.5f - 5.0f) / 5.0f);
            }
            return (512 / 16.0f) * (1.0f - 0.7f * (Diff_Size - 5.0f) / 5.0f);
        }
        public int GetHitObjectCount()
        {
            return HitObjects.Count;
        }
        public HitObject GetHitObject(int i, osuElements.Helpers.Mods mods)
        {
            if ((mods & osuElements.Helpers.Mods.HardRock) == osuElements.Helpers.Mods.HardRock)
            {
                return HitObjects.ElementAt(i);
            } else
            {
                return HitObjects.ElementAt(i);
            }
            
        }
        public TimeSpan GetBeatLength(TimeSpan time)
        {
            int i = 0;
            int index = 0;
            while (TimingPoints.ElementAt(i).Offset<=time.TotalMilliseconds)
            {
                i++;

                if (i >= TimingPoints.Count)
                { break; }
                if (TimingPoints.ElementAt(i).IsTiming)
                {
                    index = i;
                }
            }
            return TimeSpan.FromMilliseconds(TimingPoints.ElementAt(index).MillisecondsPerBeat);
        }

        #region getters
        public int GetBackgroundTexture() { return BackgroundTexture; }
        public int GetBackgroundTextureHeight() { return BackgroundTextureHeight; }
        public int GetBackgroundTextureWidth() { return BackgroundTextureWidth; }
        public string GetBackgroundFilePath() { return Background.FilePath; }
        public string GetAudioFilePath() { return Directory+System.IO.Path.DirectorySeparatorChar+AudioFilename; }
        
        #endregion
    }
    public class TextureContainer
    {
        public int TextureID { get; private set; }
        public int MinX { get; set; }
        public int MaxX { get; set; }
        public int MinY { get; set; }
        public int MaxY { get; set; }
        public Position BottomLeft { get { return new Position(MinX, MinY); } }
        public Position BottomRight { get { return new Position(MaxX, MinY); } }
        public Position TopLeft { get { return new Position(MinX, MaxY); } }
        public Position TopRight { get { return new Position(MaxX, MaxY); } }

        public TextureContainer(int id,int minX,int maxX,int minY,int maxY)
        {
            TextureID = id;
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
        }
    }
    
}
