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
            endEvent = new HitCircleEndEvent(this, gl);
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
}
