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

using osuElements.Beatmaps;
using osuElements;


namespace OsuSpectate.GameplayEngine
{
    public interface RenderObject
    {
        string GetRenderObjectType();
        void Draw(TimeSpan time, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight);
    }
    
    public class RenderHitCircle : RenderObject
    {
        public HitCircle BaseHitCircle;
        public OsuStandardGameplayEngine GameplayEngine;
        private bool Initialized;
        public RenderHitCircle(HitCircle c, OsuStandardGameplayEngine e)
        {
            BaseHitCircle = c;
            GameplayEngine = e;
        }
        public string GetRenderObjectType()
        {
            return "HitCircle";
        }
        public TimeSpan GetStartTime()
        {
            return TimeSpan.FromMilliseconds(BaseHitCircle.StartTime).Subtract(GameplayEngine.GetARMilliseconds());
        }
        public TimeSpan GetEndTime()
        {
            return TimeSpan.FromMilliseconds(BaseHitCircle.StartTime).Add(GameplayEngine.GetOD50Milliseconds());
        }

        public void Draw(TimeSpan time, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight)
        {
            throw new NotImplementedException();
        }
    }
    public class RenderSliderBorder : RenderObject
    {
        public Slider BaseSlider;
        public OsuStandardGameplayEngine GameplayEngine;
        public TextureContainer SliderBorderTexture;
        private bool Initialized;
        public RenderSliderBorder(Slider s, OsuStandardGameplayEngine e)
        {
            GameplayEngine = e;
            BaseSlider = s;
        }
        public void computeTexture()
        {
            SliderBorderTexture = GameplayEngine.getBeatmap().GetSliderTexture(BaseSlider, GameplayEngine.getMods());
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
            return TimeSpan.FromMilliseconds(BaseSlider.StartTime).Subtract(GameplayEngine.GetARMilliseconds());
        }
        public TimeSpan GetEndTime()
        {
            return TimeSpan.FromMilliseconds(BaseSlider.EndTime).Add(GameplayEngine.GetOD50Milliseconds());
        }
        public void Draw(TimeSpan time, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight)
        {
            throw new NotImplementedException();
        }
    }
    public class RenderSliderHead : RenderObject
    {
        public Slider BaseSlider;
        public OsuStandardGameplayEngine GameplayEngine;
        private bool Initialized;
        public RenderSliderHead(Slider s, OsuStandardGameplayEngine e)
        {
            GameplayEngine = e;
            BaseSlider = s;
        }
        public string GetRenderObjectType()
        {
            return "SliderHead";
        }
        public TimeSpan GetStartTime()
        {
            return TimeSpan.FromMilliseconds(BaseSlider.StartTime).Subtract(GameplayEngine.GetARMilliseconds());
        }
        public TimeSpan GetEndTime()
        {
            return TimeSpan.FromMilliseconds(BaseSlider.StartTime);
        }
        public void Draw(TimeSpan time, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight)
        {
            throw new NotImplementedException();
        }
    }
    public class RenderSliderTick : RenderObject
    {
        public Slider BaseSlider;
        public OsuStandardGameplayEngine GameplayEngine;
        public int? SliderBorderTexture;
        private bool Initialized;
        private PointF Position;
        private TimeSpan Time;
        public RenderSliderTick(Slider s, float time, OsuStandardGameplayEngine e)
        {
            GameplayEngine = e;
            BaseSlider = s;
            osuElements.Position p = s.PositionAtTime(time);
            Position = new PointF(p.XForHitobject,p.YForHitobject);
            Time = TimeSpan.FromMilliseconds( s.StartTime+time);
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
            return TimeSpan.FromMilliseconds(BaseSlider.EndTime);
        }
        public void Draw(TimeSpan time, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight)
        {
            throw new NotImplementedException();
        }
    }
    public class RenderSliderTail : RenderObject
    {
        public Slider BaseSlider;
        public OsuStandardGameplayEngine GameplayEngine;
        public Nullable<int> SliderBorderTexture;
        private bool Initialized;
        private PointF Position;
        public RenderSliderTail(Slider s, OsuStandardGameplayEngine e)
        {
            GameplayEngine = e;
            BaseSlider = s;
        }
        public string GetRenderObjectType()
        {
            return "SliderTail";
        }
        public TimeSpan GetStartTime()
        {
            return TimeSpan.FromMilliseconds(BaseSlider.StartTime)- GameplayEngine.GetARMilliseconds();
        }
        public TimeSpan GetEndTime()
        {
            return TimeSpan.FromMilliseconds(BaseSlider.EndTime);
        }
        public void Draw(TimeSpan time, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight)
        {
            throw new NotImplementedException();
        }
    }
    public class RenderSpinner : RenderObject
    {
        public Spinner BaseSpinner;
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
        public Position position;
        private bool Initialized;
        public Render300(HitObject c, List<RenderObject> renderList, Tree<GameplayEvent> eventList)
        {
            time = TimeSpan.FromMilliseconds(c.EndTime);
            position = c.EndPosition;
            if (c.Type == osuElements.Helpers.HitObjectType.Slider)
            {
                position = ((Slider)c).PositionAtTime(c.EndTime);
            }
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
        public Position position;
        private bool Initialized;
        public Render100(HitObject c, List<RenderObject> renderList, Tree<GameplayEvent> eventList)
        {
            time = TimeSpan.FromMilliseconds(c.EndTime);
            position = c.EndPosition;
            if (c.Type == osuElements.Helpers.HitObjectType.Slider)
            {
                position = ((Slider)c).PositionAtTime(c.EndTime);
            }
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
        public Position position;
        private bool Initialized;
        public Render50(HitObject c, List<RenderObject> renderList, Tree<GameplayEvent> eventList)
        {
            time = TimeSpan.FromMilliseconds(c.EndTime);
            position = c.EndPosition;
            if (c.Type == osuElements.Helpers.HitObjectType.Slider)
            {
                position = ((Slider)c).PositionAtTime(c.EndTime);
            }
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
        public Position position;
        private bool Initialized;
        public RenderMiss(HitObject c, List<RenderObject> renderList, Tree<GameplayEvent> eventList)
        {
            time = TimeSpan.FromMilliseconds(c.EndTime);
            position = c.EndPosition;
            if (c.Type == osuElements.Helpers.HitObjectType.Slider)
            {
                position = ((Slider)c).PositionAtTime(c.EndTime);
            }
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
