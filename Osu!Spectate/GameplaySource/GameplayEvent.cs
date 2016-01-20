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
        public abstract void handle();
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
    class ReplayClickEvent : GameplayEvent
    {
        Keys keys;
        float x;
        float y;
        public ReplayClickEvent(ReplayFrame frame)
            : base(new TimeSpan(frame.Time*TimeSpan.TicksPerMillisecond))
        {
            keys = frame.Keys;
            x = frame.X;
            y = frame.Y;

        }
        public override void handle()
        {

        }
    }
    class ReplayReleasedEvent : GameplayEvent
    {
        public ReplayReleasedEvent(ReplayFrame frame)
            : base(new TimeSpan(frame.Time * TimeSpan.TicksPerMillisecond))
        {

        }
        public override void handle()
        {

        }
    }
    class HitCircleBeginEvent : GameplayEvent
    {
        public HitCircleBeginEvent(ReplayFrame frame)
            : base(new TimeSpan(frame.Time * TimeSpan.TicksPerMillisecond))
        {

        }
        public override void handle()
        {

        }
    }
    class HitCircleEndEvent : GameplayEvent
    {
        public HitCircleEndEvent(ReplayFrame frame)
            : base(new TimeSpan(frame.Time * TimeSpan.TicksPerMillisecond))
        {

        }
        public override void handle()
        {

        }
    }
    class SliderBeginEvent : GameplayEvent
    {
        public SliderBeginEvent(ReplayFrame frame)
            : base(new TimeSpan(frame.Time * TimeSpan.TicksPerMillisecond))
        {

        }
        public override void handle()
        {

        }
    }
    class SliderTickEvent : GameplayEvent
    {
        public SliderTickEvent(ReplayFrame frame)
            : base(new TimeSpan(frame.Time * TimeSpan.TicksPerMillisecond))
        {

        }
        public override void handle()
        {

        }
    }
    class SliderEndEvent : GameplayEvent
    {
        public SliderEndEvent(ReplayFrame frame)
            : base(new TimeSpan(frame.Time * TimeSpan.TicksPerMillisecond))
        {

        }
        public override void handle()
        {

        }
    }
    class SpinnerBeginEvent : GameplayEvent
    {
        public SpinnerBeginEvent(ReplayFrame frame)
            : base(new TimeSpan(frame.Time * TimeSpan.TicksPerMillisecond))
        {

        }
        public override void handle()
        {

        }
    }
    class SpinnerEndEvent : GameplayEvent
    {
        public SpinnerEndEvent(ReplayFrame frame)
            : base(new TimeSpan(frame.Time * TimeSpan.TicksPerMillisecond))
        {

        }
        public override void handle()
        {

        }
    }
    //render events
    class RenderHitCircleBeginEvent : GameplayEvent
    {
        List<GameplayEvent> Parent;
        List<RenderObject> RenderList;
        OsuStandardHitCircle Circle;
        RenderHitCircle Render;
        public RenderHitCircleBeginEvent(OsuStandardHitCircle circle, List<GameplayEvent> parent, List<RenderObject> renderList, OsuStandardGameplayInput replay)
            : base(circle.getStart().Subtract(circle.getBeatmap().GetARMilliseconds(replay.GetMods())))
        {
            Parent = parent;
            Circle = circle;
            Render = new RenderHitCircle(circle, replay);
            RenderList = renderList;
            Parent.Add(this);
        }
        public override void handle()
        {
            
            RenderList.Add(Render);
            Parent.Remove(this);
            new RenderHitCircleEndEvent(Circle, Render, Parent,RenderList);
        }
    }
    class RenderHitCircleEndEvent : GameplayEvent
    {
        List<GameplayEvent> Parent;
        List<RenderObject> RenderList;
        RenderHitCircle Render;
        public RenderHitCircleEndEvent(OsuStandardHitCircle circle, RenderHitCircle render, List<GameplayEvent> parent, List<RenderObject> renderList)
            : base(circle.getEnd())
        {
            //put this after circle.getEnd()
            //.Add(circle.getBeatmap().GetOD50Milliseconds(render.GameplayInput.GetMods())) 
            Parent = parent;
            RenderList = renderList;
            Render = render;
            
            Parent.Add(this);
        }
        public override void handle()
        {
            RenderList.Remove(Render);
            Parent.Remove(this);
        }
    }
    class RenderSliderDrawEvent : GameplayEvent
    {
        RenderSlider rs;
        List<GameplayEvent> Parent;
        public RenderSliderDrawEvent(TimeSpan time, List<GameplayEvent> parent, RenderSlider slider) : base(time)
        {
            Parent = parent;
            rs = slider;
        }
        public override void handle()
        {
            rs.computeTexture();
            Parent.Remove(this);
        }
    }
    class RenderSliderBeginEvent : GameplayEvent
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
        public override void handle()
        {
            RenderList.Add(Render);
            Parent.Remove(this);
            Parent.Add(new RenderSliderEndEvent(Slider, Render, Parent, RenderList));
        }
    }
    class RenderSliderEndEvent : GameplayEvent
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
        public override void handle()
        {
            RenderList.Remove(Render);
            Parent.Remove(this);
        }
    }
}
