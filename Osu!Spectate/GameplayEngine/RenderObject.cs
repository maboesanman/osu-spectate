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

using OsuSpectate.GameplaySource;
using OsuSpectate.Beatmap;
using ReplayAPI;

namespace OsuSpectate.GameplayEngine
{
    public interface RenderObject
    {
        string GetRenderObjectType();
        void Draw(TimeSpan time, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight);
    }
    
    public class RenderHitCircle : RenderObject
    {
        public OsuStandardHitCircle HitCircle;
        public OsuStandardGameplayEngine GameplayEngine;
        private bool Initialized;
        public RenderHitCircle(OsuStandardHitCircle c, OsuStandardGameplayEngine e)
        {
            HitCircle = c;
            GameplayEngine = e;
        }
        public string GetRenderObjectType()
        {
            return "HitCircle";
        }
        public TimeSpan GetStartTime()
        {
            return HitCircle.getStart().Subtract(GameplayEngine.GetARMilliseconds());
        }
        public TimeSpan GetEndTime()
        {
            return HitCircle.getStart().Add(GameplayEngine.GetOD50Milliseconds());
        }

        public void Draw(TimeSpan time, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight)
        {
            throw new NotImplementedException();
        }
    }
    public class RenderSliderBorder : RenderObject
    {
        public OsuStandardSlider Slider;
        public OsuStandardGameplayEngine GameplayEngine;
        public Nullable<int> SliderBorderTexture;
        private bool Initialized;
        public RenderSliderBorder(OsuStandardSlider s, OsuStandardGameplayEngine e)
        {
            GameplayEngine = e;
            Slider = s;
        }
        public void computeTexture()
        {
            SliderBorderTexture = Slider.beatmap.GetSliderTexture(Slider, GameplayEngine.getMods());
        }
        public string GetRenderObjectType()
        {
            return "Slider";
        }
        public void kill()
        {
            
        }
        public TimeSpan GetStartTime()
        {
            return Slider.getStart().Subtract(GameplayEngine.GetARMilliseconds());
        }
        public TimeSpan GetEndTime()
        {
            return Slider.getEnd().Add(GameplayEngine.GetOD50Milliseconds());
        }
        public void Draw(TimeSpan time, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight)
        {
            throw new NotImplementedException();
        }
    }
    public class RenderSliderHead : RenderObject
    {
        public OsuStandardSlider Slider;
        public OsuStandardGameplayEngine GameplayEngine;
        private bool Initialized;
        public RenderSliderHead(OsuStandardSlider s, OsuStandardGameplayEngine e)
        {
            GameplayEngine = e;
            Slider = s;
        }
        public string GetRenderObjectType()
        {
            return "SliderHead";
        }
        public TimeSpan GetStartTime()
        {
            return Slider.getStart().Subtract(GameplayEngine.GetARMilliseconds());
        }
        public TimeSpan GetEndTime()
        {
            return Slider.getStart();
        }
        public void Draw(TimeSpan time, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight)
        {
            throw new NotImplementedException();
        }
    }
    public class RenderSliderTick : RenderObject
    {
        public OsuStandardSlider Slider;
        public OsuStandardGameplayEngine GameplayEngine;
        public Nullable<int> SliderBorderTexture;
        private bool Initialized;
        private PointF Position;
        private TimeSpan Time;
        public RenderSliderTick(OsuStandardSlider s, float position, OsuStandardGameplayEngine e)
        {
            GameplayEngine = e;
            Slider = s;
            Position = s.curve.pointOnCurve(position);
            Time = TimeSpan.FromTicks((long)(s.getBeatmap().GetSliderDuration(s).Ticks*position));
        }
        public string GetRenderObjectType()
        {
            return "SliderTick";
        }
        public TimeSpan GetStartTime()
        {
            return Time-GameplayEngine.GetARMilliseconds();
        }
        public TimeSpan GetEndTime()
        {
            return Slider.getEnd();
        }
        public void Draw(TimeSpan time, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight)
        {
            throw new NotImplementedException();
        }
    }
    public class RenderSliderTail : RenderObject
    {
        public OsuStandardSlider Slider;
        public OsuStandardGameplayEngine GameplayEngine;
        public Nullable<int> SliderBorderTexture;
        private bool Initialized;
        private PointF Position;
        public RenderSliderTail(OsuStandardSlider s, OsuStandardGameplayEngine e)
        {
            GameplayEngine = e;
            Slider = s;
        }
        public string GetRenderObjectType()
        {
            return "SliderTail";
        }
        public TimeSpan GetStartTime()
        {
            return Slider.getStart()- GameplayEngine.GetARMilliseconds();
        }
        public TimeSpan GetEndTime()
        {
            return Slider.getEnd();
        }
        public void Draw(TimeSpan time, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight)
        {
            throw new NotImplementedException();
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
        public string GetRenderObjectType()
        {
            return "Spinner";
        }
        public void Draw(TimeSpan time, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight)
        {
            throw new NotImplementedException();
        }
    }
    public class Render300 : RenderObject
    {
        public TimeSpan time;
        public float X;
        public float Y;
        private bool Initialized;
        public Render300(OsuStandardHitObject c, List<RenderObject> renderList, Tree<GameplayEvent> eventList)
        {
            time = c.getEnd();
            PointF p = c.getEndPosition();
            X = p.X;
            Y = p.Y;
            eventList.Add(new Render300EndEvent(this, renderList));
            eventList.Add(new Render300BeginEvent(this, renderList));
        }
        public string GetRenderObjectType()
        {
            return "300";
        }
        public TimeSpan GetStartTime()
        {
            return time;
        }
        public TimeSpan GetEndTime()
        {
            return time.Add(TimeSpan.FromMilliseconds(500));
        }
        public void Draw(TimeSpan time, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight)
        {
            throw new NotImplementedException();
        }
    }
    public class Render100 : RenderObject
    {
        public TimeSpan time;
        public float X;
        public float Y;
        private bool Initialized;
        public Render100(OsuStandardHitObject c, List<RenderObject> renderList, Tree<GameplayEvent> eventList)
        {
            time = c.getEnd();
            PointF p = c.getEndPosition();
            X = p.X;
            Y = p.Y;
            eventList.Add(new Render100EndEvent(this, renderList));
            eventList.Add(new Render100BeginEvent(this, renderList));
        }
        public string GetRenderObjectType()
        {
            return "100";
        }
        public TimeSpan GetStartTime()
        {
            return time;
        }
        public TimeSpan GetEndTime()
        {
            return time.Add(TimeSpan.FromMilliseconds(500));
        }
        public void Draw(TimeSpan time, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight)
        {
            throw new NotImplementedException();
        }
    }
    public class Render50 : RenderObject
    {
        public TimeSpan time;
        public float X;
        public float Y;
        private bool Initialized;
        public Render50(OsuStandardHitObject c, List<RenderObject> renderList, Tree<GameplayEvent> eventList)
        {
            time = c.getEnd();
            PointF p = c.getEndPosition();
            X = p.X;
            Y = p.Y;
            eventList.Add(new Render50EndEvent(this, renderList));
            eventList.Add(new Render50BeginEvent(this, renderList));
        }
        public string GetRenderObjectType()
        {
            return "50";
        }
        public TimeSpan GetStartTime()
        {
            return time;
        }
        public TimeSpan GetEndTime()
        {
            return time.Add(TimeSpan.FromMilliseconds(500));
        }
        public void Draw(TimeSpan time, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight)
        {
            throw new NotImplementedException();
        }
    }
    public class RenderMiss : RenderObject
    {
        public TimeSpan time;
        public float X;
        public float Y;
        private bool Initialized;
        public RenderMiss(OsuStandardHitObject c, List<RenderObject> renderList, Tree<GameplayEvent> eventList)
        {
            time = c.getEnd();
            PointF p = c.getEndPosition();
            X = p.X;
            Y = p.Y;
            eventList.Add(new RenderMissEndEvent(this, renderList));
            eventList.Add(new RenderMissBeginEvent(this, renderList));
        }
        public string GetRenderObjectType()
        {
            return "Miss";
        }
        public TimeSpan GetStartTime()
        {
            return time;
        }
        public TimeSpan GetEndTime()
        {
            return time.Add(TimeSpan.FromMilliseconds(500));
        }
        public void Draw(TimeSpan time, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight)
        {
            throw new NotImplementedException();
        }
    }
}
