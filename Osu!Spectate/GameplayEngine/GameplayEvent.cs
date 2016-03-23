using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OsuSpectate.Beatmap;
using OsuSpectate.GameplaySource;
using OsuSpectate.GameplayEngine;
using osuElements.Helpers;
using osuElements.Replays;
using osuElements.Beatmaps;
using osuElements;

using System.Drawing;

namespace OsuSpectate.GameplayEngine
{
    public abstract class GameplayEvent : IComparable<GameplayEvent>
    {
        TimeSpan time;
        private Guid InstanceID { get;}
        bool handled;
        public GameplayEvent(TimeSpan t)
        {
            time = t;
            InstanceID = Guid.NewGuid();
            handled = false;
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
                return InstanceID.CompareTo(e.InstanceID);
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
    }
    public class DumbEvent : GameplayEvent
    {
        public DumbEvent(TimeSpan t) : base(t)
        {

        }
        public override void _handle()
        {
            throw new NotImplementedException();
        }

        public override void _kill()
        {
            throw new NotImplementedException();
        }
    }
    public class ReplayReleaseEvent : GameplayEvent
    {
        ReplayKeys keys;
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
        Tree<GameplayEvent> Parent;
        Tree<GameplayObject> GameplayList;
        List<RenderObject> RenderList;
        OsuStandardGameplayEngine Engine;
        ReplayFrame Frame;
        public ReplayClickEvent(ReplayFrame frame, Tree<GameplayEvent> parent, Tree<GameplayObject> gameplayList, List<RenderObject> renderList, OsuStandardGameplayEngine engine)
            : base(new TimeSpan(frame.Time * TimeSpan.TicksPerMillisecond))
        {
            Parent = parent;
            RenderList = renderList;
            GameplayList = gameplayList;
            Engine = engine;
            Frame = frame;
            Parent.Add(this);
        }
        public override void _handle()
        {
            
            //Console.WriteLine(Frame.Time + " " + Frame.Keys);
            foreach (GameplayObject o in GameplayList)
            {
                switch(o.GetType())
                {
                    case ("hitcircle"):
                        HitCircle c = ((GameplayHitCircle)o).circle;
                        if (Frame.Position.Distance(c.StartPosition) < Engine.GetCSRadius())
                        {
                            if(Math.Abs(Frame.Time- o.GetTime().TotalMilliseconds)< Engine.GetOD300Milliseconds().TotalMilliseconds)
                            {
                                //Console.WriteLine("300");
                                new Render300(c,RenderList,Parent);
                                //add 300
                            }
                            else if (Math.Abs(Frame.Time - o.GetTime().TotalMilliseconds) < Engine.GetOD100Milliseconds().TotalMilliseconds)
                            {
                                //Console.WriteLine("100");
                                new Render100(c, RenderList, Parent);
                                Console.WriteLine("100 from hitcircle");
                                //add 100
                            }
                            else if (Math.Abs(Frame.Time - o.GetTime().TotalMilliseconds) < Engine.GetOD50Milliseconds().TotalMilliseconds)
                            {
                                //Console.WriteLine("50");
                                new Render50(c, RenderList, Parent);
                                Console.WriteLine("50 from hitcircle");
                                //add 50
                            }
                            else
                            {
                                break;
                            }
                            ((GameplayHitCircle)o).renderEndEvent.kill();
                            ((GameplayHitCircle)o).endEvent.kill();
                            return;
                        }
                        break;
                    case ("slider"):
                        Slider s = ((GameplaySlider)o).slider;
                        if (Frame.Position.Distance(s.StartPosition) < Engine.GetCSRadius())
                        {
                            ((GameplaySlider)o).items.Add(true);
                            ((GameplaySlider)o).headEnd.kill();
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
        Tree<GameplayObject> GameplayList;
        GameplayHitCircle GC;
        public HitCircleBeginEvent(GameplayHitCircle gc, Tree<GameplayObject> gameplayList)
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
        Tree<GameplayObject> GameplayList;
        Tree<GameplayEvent> EventList;
        GameplayHitCircle GC;
        public HitCircleEndEvent(GameplayHitCircle gc, Tree<GameplayObject> gameplayList, Tree<GameplayEvent> eventList)
            : base(gc.GetEndTime())
        {
            GC = gc;
            GameplayList = gameplayList;
            EventList = eventList;
        }
        public override void _handle()
        {
            new RenderMiss(GC.circle, GC.engine.getRenderList(),EventList);
            Console.WriteLine("Combo Break");
            GameplayList.Remove(GC);
        }
        public override void _kill()
        {
            GameplayList.Remove(GC);
        }
    }
    public class SliderHeadBeginEvent : GameplayEvent
    {
        Tree<GameplayObject> GameplayList;
        GameplaySlider GS;
        public SliderHeadBeginEvent(GameplaySlider gs, Tree<GameplayObject> gameplayList)
            : base(TimeSpan.FromMilliseconds(gs.slider.StartTime).Subtract(gs.engine.GetOD50Milliseconds()))
        {
            GS = gs;
            GameplayList = gameplayList;
        }
        public override void _handle()
        {
            GameplayList.Add(GS);
        }
        public override void _kill()
        {
        }
    }
    public class SliderHeadEndEvent : GameplayEvent
    {
        Tree<GameplayObject> GameplayList;
        Tree<GameplayEvent> EventList;
        GameplaySlider GS;
        public SliderHeadEndEvent(GameplaySlider gs, Tree<GameplayObject> gameplayList, Tree<GameplayEvent> eventList)
            : base(TimeSpan.FromMilliseconds(gs.slider.StartTime).Add(gs.engine.GetOD50Milliseconds()))
        {
            GS = gs;
            GameplayList = gameplayList;
            EventList = eventList;
        }
        public override void _handle()
        {
            GS.items.Add(false);//missed circle
            Console.WriteLine("Combo Break");
            GameplayList.Remove(GS);
        }
        public override void _kill()
        {
            GameplayList.Remove(GS);
        }
    }
    public class SliderTickEvent : GameplayEvent
    {
        GameplaySlider GS;
        Position position;
        public SliderTickEvent(GameplaySlider gs, float p)
            : base(TimeSpan.FromMilliseconds(gs.slider.StartTime).Add(TimeSpan.FromMilliseconds(p*gs.slider.Duration)))
        {
            GS = gs;
            position = gs.slider.PositionAt(p);
        }
        public override void _handle()
        {
            ReplayFrame f = GS.engine.getReplayFrame(getTime());
            ;
            if (f.Position.Distance(position) < 3.0f * GS.engine.GetCSRadius() & f.Keys!=ReplayKeys.None)
            {
                //success
                GS.items.Add(true);
                //add combo
            } else
            {
                //faliure
                GS.items.Add(false);
                Console.WriteLine("Combo Break");
                //drop combo
            }
        }
        public override void _kill()
        {
            throw new NotImplementedException();
        }
    }
    public class SliderTailEvent : GameplayEvent
    {
        GameplaySlider GS;
        Tree<GameplayEvent> EventList;
        public SliderTailEvent(GameplaySlider gs, Tree<GameplayEvent> eventList)
            : base(TimeSpan.FromMilliseconds(gs.slider.EndTime))
        {
            GS = gs;
            EventList = eventList;
        }
        public override void _handle()
        {

            
            ReplayFrame f = GS.engine.getReplayFrame(TimeSpan.FromMilliseconds(GS.slider.EndTime));
            Position p = GS.slider.EndPosition;
            if (f.Position.Distance(p) < 3.0f*GS.engine.GetCSRadius()& f.Keys != ReplayKeys.None)
            {
                //success
                GS.items.Add(true);
                //add combo
            }
            else
            {
                //faliure
                GS.items.Add(false);
            }
            
            int s = 0;
            foreach (bool x in GS.items)
            {
                if (x) s++;
            }

            if (s == GS.items.Count)
            {
                new Render300(GS.slider, GS.engine.getRenderList(), EventList);
            }
            else if (2 * s >= GS.items.Count)
            {
                new Render100(GS.slider, GS.engine.getRenderList(), EventList);
            }
            else if (s > 0)
            {
                new Render50(GS.slider, GS.engine.getRenderList(), EventList);
            }
            else
            {
                new RenderMiss(GS.slider, GS.engine.getRenderList(), EventList);
            }
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
        HitCircle Circle;
        RenderHitCircle Render;
        OsuStandardGameplayEngine GameplayEngine;
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
        RenderSliderBorder rs;
        Tree<GameplayEvent> Parent;
        public RenderSliderDrawEvent(TimeSpan time, Tree<GameplayEvent> parent, RenderSliderBorder slider) : base(time)
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
        Tree<GameplayEvent> Parent;
        List<RenderObject> RenderList;
        Slider Slider;
        RenderSliderBorder Render;
        
        public RenderSliderBeginEvent(Slider slider, Tree<GameplayEvent> parent, List<RenderObject> renderList, OsuStandardGameplayEngine engine)
            : base(TimeSpan.FromMilliseconds(slider.StartTime).Subtract(engine.GetARMilliseconds()))
        {
            Parent = parent;
            Slider = slider;
            Render = new RenderSliderBorder(slider, engine);
            RenderList = renderList;
            Parent.Add(this);
            
            Parent.Add(new RenderSliderDrawEvent(TimeSpan.FromMilliseconds(slider.StartTime).Subtract(engine.GetARMilliseconds()).Subtract(TimeSpan.FromMilliseconds(slider.Length*10.0f)),Parent, Render));
            
            
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
        Tree<GameplayEvent> Parent;
        List<RenderObject> RenderList;
        RenderSliderBorder Render;
        public RenderSliderEndEvent(Slider slider, RenderSliderBorder render, Tree<GameplayEvent> parent, List<RenderObject> renderList)
            : base(TimeSpan.FromMilliseconds(slider.EndTime))
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
        OsuStandardGameplayEngine GameplayEngine;
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
        OsuStandardGameplayEngine GameplayEngine;
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
        OsuStandardGameplayEngine GameplayEngine;
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
        OsuStandardGameplayEngine GameplayEngine;
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
