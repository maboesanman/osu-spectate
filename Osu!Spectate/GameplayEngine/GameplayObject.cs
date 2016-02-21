using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OsuSpectate.Beatmap;
using OsuSpectate.GameplaySource;

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
        public OsuStandardHitCircle circle;
        public GameplayHitCircle(OsuStandardHitCircle c, OsuStandardGameplayEngine e, Tree<GameplayObject> gl, List<RenderObject> rl, Tree<GameplayEvent> el)
        {
            circle = c;
            time = c.getStart();
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
        public OsuStandardSlider slider;
        public List<bool> items;

        public SliderHeadEndEvent headEnd;
        public GameplaySlider(OsuStandardSlider s, OsuStandardGameplayEngine e, Tree<GameplayObject> gl, List<RenderObject> rl, Tree<GameplayEvent> el)
        {
            slider = s;
            time = s.getStart();
            engine = e;

            RenderSliderBorder renderBorder = new RenderSliderBorder(slider, engine);
            RenderSliderHead renderHead = new RenderSliderHead(slider, engine);
            RenderSliderTail renderTail = new RenderSliderTail(slider, engine);
            float beatLength = (float)e.getBeatmap().GetBeatLength(s.getStart()).TotalMilliseconds;
            float tickRate = e.getBeatmap().GetSliderTickRate();
            float duration = (float)(slider.getEnd() - slider.getStart()).TotalMilliseconds;
            items = new List<bool>();
            el.Add(new SliderHeadBeginEvent(this, gl));
            headEnd = new SliderHeadEndEvent(this, gl, el);
            el.Add(headEnd);
            for(float x = beatLength/tickRate; x< duration;x+= beatLength / tickRate)
            {
                if (x % (duration / slider.repeat) != 0)
                {
                    el.Add(new SliderTickEvent(this, x / duration));
                }
                //new slider tick at x/duration
            }
            for(float x=1;x<slider.repeat;x++)
            {
                el.Add(new SliderTickEvent(this, 1.0f / slider.repeat * x));
            }
            el.Add(new SliderTailEvent(this, el));
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
   }
