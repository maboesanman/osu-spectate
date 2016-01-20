using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using ReplayAPI;

using OsuSpectate.Beatmap;

namespace OsuSpectate.GameplaySource
{
    public class OsuStandardReplay : Replay, OsuStandardGameplayInput
    {
        
        public OsuStandardBeatmap Beatmap;
        private BackgroundWorker sliderRenderer;

        private SortedDictionary<TimeSpan, int> ReplayFrameIndex;
        private List<TimeSpan> ReplayFrameIndexKeys;
        private SortedDictionary<TimeSpan, int> LifeFrameIndex;
        private List<TimeSpan> LifeFrameIndexKeys;

        public List<GameplayFrame> GameplayFrames;
        private SortedDictionary<TimeSpan, int> GameplayFrameIndex;
        private List<TimeSpan> GameplayFrameIndexKeys;

        private 

        //EventList
        List<GameplayEvent> EventList;
        TimeSpan CurrentTime;

        List<RenderObject> RenderList;

        public OsuStandardReplay(string replayFile, OsuStandardBeatmap beatmap, bool fullLoad = false) : base(replayFile, fullLoad)
        {
            Beatmap = beatmap;
            ReplayFrameIndex = new SortedDictionary<TimeSpan, int>();
            for (int i = 0; i < ReplayFrames.Count(); i++)
            {
                
                ReplayFrameIndex[new TimeSpan(ReplayFrames.ElementAt(i).Time*TimeSpan.TicksPerMillisecond)]= i;
            }
            ReplayFrameIndexKeys = ReplayFrameIndex.Keys.ToList();

            LifeFrameIndex = new SortedDictionary<TimeSpan,int>();
            for (int i = 0; i < LifeFrames.Count(); i++)
            {
                LifeFrameIndex[new TimeSpan(LifeFrames.ElementAt(i).Time * TimeSpan.TicksPerMillisecond)] = i;
            }
            LifeFrameIndexKeys = LifeFrameIndex.Keys.ToList();


            EventList = new List<GameplayEvent>();
            RenderList = new List<RenderObject>();
            CurrentTime = TimeSpan.Zero;
            sliderRenderer = new BackgroundWorker();
            sliderRenderer.DoWork += RenderSlider;
            for (int i = 0; i < Beatmap.GetHitObjectCount(); i++)
            {
                OsuStandardHitObject ho = Beatmap.GetHitObject(i, Mods);
                switch (ho.getType())
                {
                    case ("hitcircle"):
                        new RenderHitCircleBeginEvent((OsuStandardHitCircle)ho,EventList,RenderList,this);
                        break;
                    case ("slider"):
                        new RenderSliderBeginEvent((OsuStandardSlider)ho, EventList, RenderList, this);
                        break;

                }
            }
            for (int i = 0; i < (Beatmap).GetHitObjectCount(); i++)
            {
                OsuStandardHitObject ho = (Beatmap).GetHitObject(i, Mods);
                if (ho.getType() == "slider")
                {
                    Beatmap.GetSliderTexture((OsuStandardSlider)ho, GetMods());
                }
            }
            //sliderRenderer.RunWorkerAsync(Beatmap);
        }
        public string GetPlayerName()
        {
            return PlayerName;
        }
        public Mods GetMods()
        {
            return Mods;
        }
        public GameplayFrame GetGameplayFrame(TimeSpan t)
        {
            int a = GameplayFrameIndexKeys.BinarySearch(t);
            Console.WriteLine(a);
            return GameplayFrames.ElementAt(GameplayFrameIndex[GameplayFrameIndexKeys.ElementAt(a)]);
        }
        public GameplayFrame GetGameplayFrame(long milliseconds)
        {
            return GetGameplayFrame(new TimeSpan(milliseconds * TimeSpan.TicksPerMillisecond));
        }


        public ReplayFrame GetReplayFrame(TimeSpan time)
        {
            int index = ReplayFrameIndexKeys.BinarySearch(time);
            TimeSpan KeyPrevious = new TimeSpan(0);
            TimeSpan KeyNext = new TimeSpan(0);
            if (index == 0)
            {
                KeyPrevious = ReplayFrameIndexKeys[0];
                KeyNext = ReplayFrameIndexKeys[0];
            }
            else if (index < 0)
            {
                KeyPrevious = ReplayFrameIndexKeys[(~index - 1)];
                KeyNext = ReplayFrameIndexKeys[Math.Min(~index, ReplayFrameIndexKeys.Count - 1)];
            }
            else
            {
                KeyPrevious = ReplayFrameIndexKeys[index - 1];
                KeyNext = ReplayFrameIndexKeys[Math.Min(index, ReplayFrameIndexKeys.Count - 1)];
            }
            if (KeyNext == KeyPrevious)
            {
                ReplayFrame frame;
                frame = ReplayFrames[ReplayFrameIndex[KeyNext]];
                return frame;
            }
            ReplayFrame Frame1 = ReplayFrames[ReplayFrameIndex[KeyPrevious]];
            ReplayFrame Frame2 = ReplayFrames[ReplayFrameIndex[KeyNext]];
            long milliseconds = (long)time.TotalMilliseconds;
            float timeScale = (milliseconds * 1.0F - (float)Frame1.Time * 1.0F) / ((float)Frame2.Time * 1.0F - (float)Frame1.Time * 1.0F);
            ReplayFrame ResultFrame = Frame1;
            ResultFrame.Time=   (int) milliseconds;
            ResultFrame.X = timeScale * Frame2.X + (1 - timeScale) * Frame1.X;
            ResultFrame.Y = timeScale * Frame2.Y + (1 - timeScale) * Frame1.Y;
            return ResultFrame;
        }

        public ReplayFrame GetReplayFrame(long milliseconds)
        {
            return GetReplayFrame(new TimeSpan(milliseconds * TimeSpan.TicksPerMillisecond));
        }

        public TimeSpan GetOD300Milliseconds()
        {
            return Beatmap.GetOD300Milliseconds(GetMods());
        }
        public TimeSpan GetOD100Milliseconds()
        {
            return Beatmap.GetOD100Milliseconds(GetMods());
        }
        public TimeSpan GetOD50Milliseconds()
        {
            return Beatmap.GetOD50Milliseconds(GetMods());
        }
        public TimeSpan GetARMilliseconds()
        {
            return Beatmap.GetARMilliseconds(GetMods());
        }
        public float GetCSRadius()
        {
            return Beatmap.GetCSRadius(GetMods());
        }
        public void HandleUntil(TimeSpan time)
        {
            bool x = true;
            while (EventList.Count > 0 && x)
            {
                EventList.Sort();
                if (EventList.First().getTime().CompareTo(time) < 0)
                {
                    EventList.First().handle();
                }
                else
                {
                    x = false;
                }
            }
        }
        public OsuStandardBeatmap GetBeatmap()
        {
            return Beatmap;
        }
        public List<RenderObject> GetRenderList()
        {
            return RenderList;
        }
        public void RenderSlider(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < ((OsuStandardBeatmap)e.Argument).GetHitObjectCount(); i++)
            {
                OsuStandardHitObject ho = ((OsuStandardBeatmap)e.Argument).GetHitObject(i, Mods);
                if(ho.getType()=="slider")
                {
                    Beatmap.GetSliderTexture((OsuStandardSlider)ho, GetMods());
                }
            }
        }
    }
}
