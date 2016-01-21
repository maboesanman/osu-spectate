using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OsuSpectate.Beatmap;
using ReplayAPI;

namespace OsuSpectate.GameplaySource
{
    public abstract class GameplayEvent : IComparable<GameplayEvent>
    {
        TimeSpan time;
        int duplicateIndex;
        bool handled;
        public GameplayEvent(TimeSpan t)
        {
            time = t;
        }
        public void handle()
        {
            _handle();
            setHandled();
        }
        public void kill()
        {
            _kill();
            setHandled();
        }
        public abstract void _handle();
        public abstract void _kill();
        public int CompareTo(GameplayEvent e)
        {
            if (time != e.getTime())
            {
                return time.CompareTo(e.getTime());
            }
            else
            {
                return duplicateIndex.CompareTo(e.getDuplicateIndex());
            }
        }
        public bool isHandled()
        {
            return (handled);
        }
        public void setHandled()
        {
            handled = true;
        }
        public TimeSpan getTime()
        {
            return time;
        }
        public int getDuplicateIndex()
        {
            return duplicateIndex;
        }
        public void setDuplicateIndex(int i)
        {
            duplicateIndex = i;
        }
    }
    public class ReplayReleaseEvent : GameplayEvent
    {
        Keys keys;
        float x;
        float y;
        public ReplayReleaseEvent(ReplayFrame frame)
            : base(new TimeSpan(frame.Time*TimeSpan.TicksPerMillisecond))
        {
            keys = frame.Keys;
            x = frame.X;
            y = frame.Y;

        }
        public override void _handle()
        {

        }

        public override void _kill()
        {
            throw new NotImplementedException();
        }
    }
    public class ReplayClickEvent : GameplayEvent
    {
        List<GameplayEvent> Parent;
        List<GameplayObject> GameplayList;
        List<RenderObject> RenderList;
        OsuStandardGameplayInput Replay;
        ReplayFrame Frame;
        public ReplayClickEvent(ReplayFrame frame, List<GameplayEvent> parent, List<GameplayObject> gameplayList, List<RenderObject> renderList, OsuStandardGameplayInput replay)
            : base(new TimeSpan(frame.Time * TimeSpan.TicksPerMillisecond))
        {
            Parent = parent;
            RenderList = renderList;
            GameplayList = gameplayList;
            Replay = replay;
            Frame = frame;
            Parent.Add(this);
        }
        public override void _handle()
        {
            GameplayList.Sort((x, y) => x.GetTime().CompareTo(y.GetTime()));
            for (int i=0;i<GameplayList.Count;i++)
            {
                switch(GameplayList.ElementAt(i).GetType())
                {
                    case ("hitcircle"):
                        OsuStandardHitCircle c = ((GameplayHitCircle)GameplayList.ElementAt(i)).circle;
                        if ((Frame.X - c.x) * (Frame.X - c.x) + (Frame.Y - c.y) * (Frame.Y - c.y)<Replay.GetCSRadius()*Replay.GetCSRadius())
                        {
                            if(Math.Abs(Frame.Time- GameplayList.ElementAt(i).GetTime().TotalMilliseconds)<Replay.GetOD300Milliseconds().TotalMilliseconds)
                            {
                                //Console.WriteLine("300");
                                new Render300(c,RenderList,Parent);
                            }
                            else if (Math.Abs(Frame.Time - GameplayList.ElementAt(i).GetTime().TotalMilliseconds) < Replay.GetOD100Milliseconds().TotalMilliseconds)
                            {
                                //Console.WriteLine("100");
                                new Render100(c, RenderList, Parent);
                            }
                            else if (Math.Abs(Frame.Time - GameplayList.ElementAt(i).GetTime().TotalMilliseconds) < Replay.GetOD50Milliseconds().TotalMilliseconds)
                            {
                                //Console.WriteLine("50");
                                new Render50(c, RenderList, Parent);
                            }
                            else
                            {
                                break;
                            }
                            ((GameplayHitCircle)GameplayList.ElementAt(i)).endEvent.setHandled();
                            ((GameplayHitCircle)GameplayList.ElementAt(i)).renderEndEvent.setHandled();
                            ((GameplayHitCircle)GameplayList.ElementAt(i)).renderEndEvent.kill();
                            ((GameplayHitCircle)GameplayList.ElementAt(i)).endEvent.kill();
                            
                            i = GameplayList.Count;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        public override void _kill()
        {
            throw new NotImplementedException();
        }
    }
    public class HitCircleBeginEvent : GameplayEvent
    {
        List<GameplayObject> GameplayList;
        GameplayHitCircle GC;
        public HitCircleBeginEvent(GameplayHitCircle gc, List<GameplayObject> gameplayList)
            : base(gc.GetStartTime())
        {
            GC = gc;
            GameplayList = gameplayList;
        }
        public override void _handle()
        {
            GameplayList.Add(GC);
        }
        public override void _kill()
        {
            throw new NotImplementedException();
        }
    }
    public class HitCircleEndEvent : GameplayEvent
    {
        List<GameplayObject> GameplayList;
        GameplayHitCircle GC;
        public HitCircleEndEvent(GameplayHitCircle gc, List<GameplayObject> gameplayList)
            : base(gc.GetEndTime())
        {
            GC = gc;
            GameplayList = gameplayList;
        }
        public override void _handle()
        {
            GameplayList.Remove(GC);
        }
        public override void _kill()
        {
            GameplayList.Remove(GC);
        }
    }
    public class SliderBeginEvent : GameplayEvent
    {
        public SliderBeginEvent(ReplayFrame frame)
            : base(new TimeSpan(frame.Time * TimeSpan.TicksPerMillisecond))
        {

        }
        public override void _handle()
        {

        }
        public override void _kill()
        {
            throw new NotImplementedException();
        }
    }
    public class SliderTickEvent : GameplayEvent
    {
        public SliderTickEvent(ReplayFrame frame)
            : base(new TimeSpan(frame.Time * TimeSpan.TicksPerMillisecond))
        {

        }
        public override void _handle()
        {

        }
        public override void _kill()
        {
            throw new NotImplementedException();
        }
    }
    public class SliderEndEvent : GameplayEvent
    {
        public SliderEndEvent(ReplayFrame frame)
            : base(new TimeSpan(frame.Time * TimeSpan.TicksPerMillisecond))
        {

        }
        public override void _handle()
        {

        }
        public override void _kill()
        {
            throw new NotImplementedException();
        }
    }
    public class SpinnerBeginEvent : GameplayEvent
    {
        public SpinnerBeginEvent(ReplayFrame frame)
            : base(new TimeSpan(frame.Time * TimeSpan.TicksPerMillisecond))
        {

        }
        public override void _handle()
        {

        }
        public override void _kill()
        {
            throw new NotImplementedException();
        }
    }
    public class SpinnerEndEvent : GameplayEvent
    {
        public SpinnerEndEvent(ReplayFrame frame)
            : base(new TimeSpan(frame.Time * TimeSpan.TicksPerMillisecond))
        {

        }
        public override void _handle()
        {

        }
        public override void _kill()
        {
            throw new NotImplementedException();
        }
    }
    //render events
    public class RenderHitCircleBeginEvent : GameplayEvent
    {
        List<RenderObject> RenderList;
        OsuStandardHitCircle Circle;
        RenderHitCircle Render;
        OsuStandardGameplayInput GameplayInput;
        public RenderHitCircleBeginEvent(RenderHitCircle rc, List<RenderObject> renderList)
            : base(rc.GetStartTime())
        {
            Render = rc;
            RenderList = renderList;
        }
        public override void _handle()
        {
            RenderList.Add(Render);
        }
        public override void _kill()
        {
            throw new NotImplementedException();
        }
    }
    public class RenderHitCircleEndEvent : GameplayEvent
    {
        List<GameplayEvent> Parent;
        List<RenderObject> RenderList;
        RenderHitCircle Render;
        public RenderHitCircleEndEvent(RenderHitCircle rc, List<RenderObject> renderList)
            : base(rc.GetEndTime())
        {
            RenderList = renderList;
            Render = rc;
        }
        public override void _handle()
        {
            RenderList.Remove(Render);
        }
        public override void _kill()
        {
            RenderList.Remove(Render);
        }
    }
    public class RenderSliderDrawEvent : GameplayEvent
    {
        RenderSlider rs;
        List<GameplayEvent> Parent;
        public RenderSliderDrawEvent(TimeSpan time, List<GameplayEvent> parent, RenderSlider slider) : base(time)
        {
            Parent = parent;
            rs = slider;
        }
        public override void _handle()
        {
            rs.computeTexture();
        }
        public override void _kill()
        {
            throw new NotImplementedException();
        }
    }
    public class RenderSliderBeginEvent : GameplayEvent
    {
        List<GameplayEvent> Parent;
        List<RenderObject> RenderList;
        OsuStandardSlider Slider;
        RenderSlider Render;
        
        public RenderSliderBeginEvent(OsuStandardSlider slider, List<GameplayEvent> parent, List<RenderObject> renderList, OsuStandardGameplayInput replay)
            : base(slider.getStart().Subtract(slider.getBeatmap().GetARMilliseconds(replay.GetMods())))
        {
            Parent = parent;
            Slider = slider;
            Render = new RenderSlider(slider, replay);
            RenderList = renderList;
            Parent.Add(this);
            parent.Add(new RenderSliderDrawEvent((slider.getStart().Subtract(slider.getBeatmap().GetARMilliseconds(replay.GetMods()))).Subtract(TimeSpan.FromMilliseconds(slider.pixelLength*10.0f)),Parent, Render));

        }
        public override void _handle()
        {
            RenderList.Add(Render);
           
            Parent.Add(new RenderSliderEndEvent(Slider, Render, Parent, RenderList));
        }
        public override void _kill()
        {
            throw new NotImplementedException();
        }
    }
    public class RenderSliderEndEvent : GameplayEvent
    {
        List<GameplayEvent> Parent;
        List<RenderObject> RenderList;
        RenderSlider Render;
        public RenderSliderEndEvent(OsuStandardSlider slider, RenderSlider render, List<GameplayEvent> parent, List<RenderObject> renderList)
            : base(slider.getEnd())
        {
            Parent = parent;
            RenderList = renderList;
            Render = render;
        }
        public override void _handle()
        {
            RenderList.Remove(Render);
        }
        public override void _kill()
        {
            RenderList.Remove(Render);
        }
    }
    public class Render300BeginEvent : GameplayEvent
    {
        List<RenderObject> RenderList;
        Render300 Render;
        OsuStandardGameplayInput GameplayInput;
        public Render300BeginEvent(Render300 r300, List<RenderObject> renderList)
            : base(r300.GetStartTime())
        {
            Render = r300;
            RenderList = renderList;
        }
        public override void _handle()
        {
            RenderList.Add(Render);
        }
        public override void _kill() { }
    }
    public class Render300EndEvent : GameplayEvent
    {
        List<RenderObject> RenderList;
        Render300 Render;
        public Render300EndEvent(Render300 r300, List<RenderObject> renderList)
            : base(r300.GetStartTime().Add(TimeSpan.FromMilliseconds(300)))
        {
            Render = r300;
            RenderList = renderList;
        }
        public override void _handle()
        {
            RenderList.Remove(Render);
        }
        public override void _kill()
        {
            RenderList.Remove(Render);
        }
    }
    public class Render100BeginEvent : GameplayEvent
    {
        List<RenderObject> RenderList;
        Render100 Render;
        OsuStandardGameplayInput GameplayInput;
        public Render100BeginEvent(Render100 r100, List<RenderObject> renderList)
            : base(r100.GetStartTime())
        {
            Render = r100;
            RenderList = renderList;
        }
        public override void _handle()
        {
            RenderList.Add(Render);
        }
        public override void _kill() { }
    }
    public class Render100EndEvent : GameplayEvent
    {
        List<RenderObject> RenderList;
        Render100 Render;
        public Render100EndEvent(Render100 r100, List<RenderObject> renderList)
            : base(r100.GetStartTime().Add(TimeSpan.FromMilliseconds(300)))
        {
            Render = r100;
            RenderList = renderList;
        }
        public override void _handle()
        {
            RenderList.Remove(Render);
        }
        public override void _kill()
        {
            RenderList.Remove(Render);
        }
    }
    public class Render50BeginEvent : GameplayEvent
    {
        List<RenderObject> RenderList;
        Render50 Render;
        OsuStandardGameplayInput GameplayInput;
        public Render50BeginEvent(Render50 r50, List<RenderObject> renderList)
            : base(r50.GetStartTime())
        {
            Render = r50;
            RenderList = renderList;
        }
        public override void _handle()
        {
            RenderList.Add(Render);
        }
        public override void _kill() { }
    }
    public class Render50EndEvent : GameplayEvent
    {
        List<RenderObject> RenderList;
        Render50 Render;
        public Render50EndEvent(Render50 r50, List<RenderObject> renderList)
            : base(r50.GetStartTime().Add(TimeSpan.FromMilliseconds(300)))
        {
            Render = r50;
            RenderList = renderList;
        }
        public override void _handle()
        {
            RenderList.Remove(Render);
        }
        public override void _kill()
        {
            RenderList.Remove(Render);
        }
    }
    public class RenderMissBeginEvent : GameplayEvent
    {
        List<RenderObject> RenderList;
        RenderMiss Render;
        OsuStandardGameplayInput GameplayInput;
        public RenderMissBeginEvent(RenderMiss rMiss, List<RenderObject> renderList)
            : base(rMiss.GetStartTime())
        {
            Render = rMiss;
            RenderList = renderList;
        }
        public override void _handle()
        {
            RenderList.Add(Render);
        }
        public override void _kill() { }
    }
    public class RenderMissEndEvent : GameplayEvent
    {
        List<RenderObject> RenderList;
        RenderMiss Render;
        public RenderMissEndEvent(RenderMiss rMiss, List<RenderObject> renderList)
            : base(rMiss.GetStartTime().Add(TimeSpan.FromMilliseconds(300)))
        {
            Render = rMiss;
            RenderList = renderList;
        }
        public override void _handle()
        {
            RenderList.Remove(Render);
        }
        public override void _kill()
        {
            RenderList.Remove(Render);
        }
    }
}
