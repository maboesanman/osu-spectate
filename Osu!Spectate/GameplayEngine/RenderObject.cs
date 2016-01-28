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
        string GetType();
        void Initialize(float x, float y, float width, float height, int windowWidth, int windowHeight);
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
        public void Initialize(float x, float y, float width, float height, int windowWidth, int windowHeight)
        {
            Initialized = true;
        }
        public string GetType()
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
    }
    public class RenderSlider : RenderObject
    {
        public OsuStandardSlider Slider;
        public OsuStandardGameplayEngine GameplayEngine;
        public Nullable<int> SliderBorderTexture;
        private bool Initialized;
        public RenderSlider(OsuStandardSlider s, OsuStandardGameplayEngine e)
        {
            GameplayEngine = e;
            Slider = s;
        }
        public void computeTexture()
        {
            SliderBorderTexture = Slider.beatmap.GetSliderTexture(Slider, GameplayEngine.getMods());
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
    public class Render300 : RenderObject
    {
        public TimeSpan time;
        public float X;
        public float Y;
        private bool Initialized;
        public Render300(OsuStandardHitCircle c, List<RenderObject> renderList, Tree<GameplayEvent> eventList)
        {
            time = c.time;
            X = c.x;
            Y = c.y;
            eventList.Add(new Render300EndEvent(this, renderList));
            eventList.Add(new Render300BeginEvent(this, renderList));
        }
        public void Initialize(float x, float y, float width, float height, int windowWidth, int windowHeight)
        {
            Initialized = true;
        }
        public string GetType()
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
    }
    public class Render100 : RenderObject
    {
        public TimeSpan time;
        public float X;
        public float Y;
        private bool Initialized;
        public Render100(OsuStandardHitCircle c, List<RenderObject> renderList, Tree<GameplayEvent> eventList)
        {
            time = c.time;
            X = c.x;
            Y = c.y;
            eventList.Add(new Render100EndEvent(this, renderList));
            eventList.Add(new Render100BeginEvent(this, renderList));
        }
        public void Initialize(float x, float y, float width, float height, int windowWidth, int windowHeight)
        {
            Initialized = true;
        }
        public string GetType()
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
    }
    public class Render50 : RenderObject
    {
        public TimeSpan time;
        public float X;
        public float Y;
        private bool Initialized;
        public Render50(OsuStandardHitCircle c, List<RenderObject> renderList, Tree<GameplayEvent> eventList)
        {
            time = c.time;
            X = c.x;
            Y = c.y;
            eventList.Add(new Render50EndEvent(this, renderList));
            eventList.Add(new Render50BeginEvent(this, renderList));
        }
        public void Initialize(float x, float y, float width, float height, int windowWidth, int windowHeight)
        {
            Initialized = true;
        }
        public string GetType()
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
    }
    public class RenderMiss : RenderObject
    {
        public TimeSpan time;
        public float X;
        public float Y;
        private bool Initialized;
        public RenderMiss(OsuStandardHitCircle c, List<RenderObject> renderList, Tree<GameplayEvent> eventList)
        {
            time = c.time;
            X = c.x;
            Y = c.y;
            eventList.Add(new RenderMissEndEvent(this, renderList));
            eventList.Add(new RenderMissBeginEvent(this, renderList));
        }
        public void Initialize(float x, float y, float width, float height, int windowWidth, int windowHeight)
        {
            Initialized = true;
        }
        public string GetType()
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
    }
}
