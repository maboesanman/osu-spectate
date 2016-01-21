using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OsuSpectate.Beatmap;
using OsuSpectate.GameplaySource;

namespace OsuSpectate.GameplaySource
{
    public interface GameplayObject
    {
        string GetType();
        TimeSpan GetTime();
    }
    public class GameplayHitCircle : GameplayObject
    {
        public TimeSpan time;
        public OsuStandardGameplayInput input;
        public HitCircleEndEvent endEvent;
        public RenderHitCircleEndEvent renderEndEvent;
        public OsuStandardHitCircle circle;
        public GameplayHitCircle(OsuStandardHitCircle c, OsuStandardGameplayInput r, List<GameplayObject> gl, List<RenderObject> rl, List<GameplayEvent> el)
        {
            circle = c;
            time = c.getStart();
            input = r;
            RenderHitCircle render = new RenderHitCircle(c, r);
            el.Add(endEvent = new HitCircleEndEvent(this, gl));
            el.Add(renderEndEvent = new RenderHitCircleEndEvent(render, rl));
            el.Add(new HitCircleBeginEvent(this, gl));
            el.Add(new RenderHitCircleBeginEvent(render, rl));

        }

        public TimeSpan GetStartTime()
        {
            return time.Subtract(input.GetOD50Milliseconds());
        }
        public TimeSpan GetEndTime()
        {
            return time.Add(input.GetOD50Milliseconds());
        }
        string GameplayObject.GetType()
        {
            return "hitcircle";
        }

        public TimeSpan GetTime()
        {
            return time;
        }
    }
}
