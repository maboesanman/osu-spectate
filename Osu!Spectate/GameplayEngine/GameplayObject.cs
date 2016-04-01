using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OsuSpectate.GameplaySource;

using osuElements.Beatmaps;
using osuElements;

namespace OsuSpectate.GameplayEngine
{
    public abstract class GameplayObject : IComparable<GameplayObject>
    {
        abstract public string GetType();
        abstract public TimeSpan GetTime();

        public int CompareTo(GameplayObject other)
        {
            return (GetTime().CompareTo(other.GetTime()));
        }
    }
    public class GameplayHitCircle : GameplayObject
    {
        public TimeSpan time;
        public OsuStandardGameplayEngine engine;
        public HitCircleEndEvent endEvent;
        public RenderHitCircleEndEvent renderEndEvent;
        public HitCircle circle;
        public GameplayHitCircle(HitCircle c, OsuStandardGameplayEngine e, Tree<GameplayObject> gl, List<RenderObject> rl, Tree<GameplayEvent> el)
        {
            circle = c;
            time = TimeSpan.FromMilliseconds(c.StartTime);
            engine = e;

            RenderHitCircle render = new RenderHitCircle(c, e);
            endEvent = new HitCircleEndEvent(this, gl, el);
            renderEndEvent = new RenderHitCircleEndEvent(render, rl);
            el.Add(endEvent);
            el.Add(renderEndEvent);
            el.Add(new HitCircleBeginEvent(this, gl));
            el.Add(new RenderHitCircleBeginEvent(render, rl));


        }

        public TimeSpan GetStartTime()
        {
            return time.Subtract(engine.GetOD50Milliseconds());
        }
        public TimeSpan GetEndTime()
        {
            return time.Add(engine.GetOD50Milliseconds());
        }
        public override string GetType()
        {
            return "hitcircle";
        }

        public override TimeSpan GetTime()
        {
            return time;
        }
    }
    public class GameplaySlider : GameplayObject
    {
        
        public TimeSpan time;
        public OsuStandardGameplayEngine engine;
        public Slider slider;
        public List<bool> items;
        public bool headHit;
        public bool tailHit;
        public bool finished;
        public SliderHeadEndEvent headEnd;
        public GameplaySliderTail tail;
        public GameplaySlider(Slider s, OsuStandardGameplayEngine e, Tree<GameplayObject> gl, List<RenderObject> rl, Tree<GameplayEvent> el)
        {
            slider = s;
            time = TimeSpan.FromMilliseconds(s.StartTime);
            engine = e;
            headHit = false;
            tailHit = false;
            finished = false;
            RenderSliderBorder renderBorder = new RenderSliderBorder(slider, engine);
            RenderSliderHead renderHead = new RenderSliderHead(slider, engine);
            RenderSliderTail renderTail = new RenderSliderTail(slider, engine);
            
            float beatLength = (float)(e.getBeatmap().GetBeatLength(TimeSpan.FromMilliseconds(slider.StartTime)).TotalMilliseconds);
            float tickRate = e.getBeatmap().SliderTickRate;
            float duration = slider.Duration;
            items = new List<bool>();
            el.Add(new SliderHeadBeginEvent(this, gl));
            headEnd = new SliderHeadEndEvent(this, gl, el);
            el.Add(headEnd);
            tail = new GameplaySliderTail(TimeSpan.FromMilliseconds(slider.EndTime), this);
            
            for(float x = beatLength/tickRate; x< duration;x+= beatLength / tickRate)
            {
                if (x % (duration / slider.SegmentCount) != 0.0f)
                {
                    el.Add(new SliderTickEvent(this,s.PositionAtTime(s.StartTime+x),TimeSpan.FromMilliseconds(s.StartTime + x)));
                }
                //new slider tick at x/duration
            }
            for(float x=1;x<slider.SegmentCount;x++)
            {
                if (x % 2 == 0)
                {
                    el.Add(new SliderTickEvent(this, slider.StartPosition, TimeSpan.FromMilliseconds(slider.StartTime + x * (slider.Duration / slider.SegmentCount))));
                }
                if (x % 2 == 1)
                {
                    el.Add(new SliderTickEvent(this, slider.EndPosition, TimeSpan.FromMilliseconds(slider.StartTime + x * (slider.Duration / slider.SegmentCount))));
                }
            }
            el.Add(new SliderTailBeginEvent(this, gl, el));
            el.Add(new SliderTailEndEvent(this, gl, el));
            /*
            RenderSliderBorder render = new RenderHitCircle(c, e);
            endEvent = new HitCircleEndEvent(this, gl, el);
            renderEndEvent = new RenderHitCircleEndEvent(render, rl);
            el.Add(endEvent);
            el.Add(renderEndEvent);
            el.Add(new HitCircleBeginEvent(this, gl));
            el.Add(new RenderHitCircleBeginEvent(render, rl));
            */

        }

        public TimeSpan GetStartTime()
        {
            return time.Subtract(engine.GetOD50Milliseconds());
        }
        public TimeSpan GetEndTime()
        {
            return time.Add(engine.GetOD50Milliseconds());
        }
        public override string GetType()
        {
            return "slider";
        }

        public override TimeSpan GetTime()
        {
            return time;
        }
    }
    public class GameplaySliderTail : GameplayObject
    {
        public TimeSpan time;
        public GameplaySlider GS;
        public GameplaySliderTail(TimeSpan t,GameplaySlider gs)
        {
            time = t;
            GS = gs;
        }
        public override TimeSpan GetTime()
        {
            return time;
        }
        public TimeSpan GetStartTime()
        {
            return time.Subtract(GS.engine.GetOD50Milliseconds());
        }
        public TimeSpan GetEndTime()
        {
            return time;
        }
        public override string GetType()
        {
            return "slider tail";
        }
    }
}
