using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ReplayAPI;

using OsuSpectate.Beatmap;

namespace OsuSpectate.GameplaySource
{
    public class OsuStandardReplay : Replay, OsuStandardGameplayInput
    {
        
        public OsuStandardBeatmap Beatmap;

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
            
            for (int i = 0; i < Beatmap.GetHitObjectList().Count; i++)
            {
                switch (Beatmap.GetHitObjectList().ElementAt(i).getType())
                {
                    case ("hitcircle"):
                        new RenderHitCircleBeginEvent((OsuStandardHitCircle)(Beatmap.GetHitObjectList().ElementAt(i)),EventList,RenderList,this);
                        break;
                    case ("slider"):
                        new RenderSliderBeginEvent((OsuStandardSlider)(Beatmap.GetHitObjectList().ElementAt(i)), EventList, RenderList, this);
                        break;

                }
            }
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
            return GameplayFrames.ElementAt(GameplayFrameIndex[GameplayFrameIndexKeys.ElementAt(GameplayFrameIndexKeys.BinarySearch(t))]);
        }
        public GameplayFrame GetGameplayFrame(long milliseconds)
        {
            return GetGameplayFrame(new TimeSpan(milliseconds * TimeSpan.TicksPerMillisecond));
        }


        public ReplayFrame GetReplayFrame(TimeSpan t)
        {
            return ReplayFrames.ElementAt(ReplayFrameIndex[ReplayFrameIndexKeys.ElementAt(ReplayFrameIndexKeys.BinarySearch(t))]);
        } 

        public ReplayFrame GetReplayFrame(long milliseconds)
        {
            return GetReplayFrame(new TimeSpan(milliseconds * TimeSpan.TicksPerMillisecond));
        }

        public TimeSpan GetOD300Milliseconds()
        {
            return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * (78.0f - Beatmap.GetOverallDifficulty() * 6.0f)));
        }
        public TimeSpan GetOD100Milliseconds()
        {
            return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * (138.0f - Beatmap.GetOverallDifficulty() * 8.0f)));
        }
        public TimeSpan GetOD50Milliseconds()
        {
            return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * (198.0f - Beatmap.GetOverallDifficulty() * 10.0f)));
        }
        public TimeSpan GetARMilliseconds()
        {
            if ((Mods & Mods.HardRock) == Mods.HardRock)
            {
                return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * ((1800.0f - (Math.Min(Beatmap.GetApproachRate() * 1.4f, 10.0f)) * 120.0f) - (Math.Max((Math.Min(Beatmap.GetApproachRate() * 1.4f, 10.0f) - 5.0f) * 30.0f, 0.0f)))));
            }
            return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * ((1800.0f - (Beatmap.GetApproachRate()) * 120.0f) - (Math.Max((Beatmap.GetApproachRate() - 5.0f) * 30.0f, 0.0f)))));
        }
        public float GetCSRadius()
        {
            if ((Mods & Mods.HardRock) == Mods.HardRock)
            {
                return 4.0f * (12.0f - Math.Min(Beatmap.GetCircleSize() * 1.3f, 10.0f));
            }
            return 4.0f * (12.0f - Beatmap.GetCircleSize());
        }
        public void HandleUntil(TimeSpan time)
        {
            bool x = true;
            while (EventList.Count > 0 && x)
            {
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
    }
}
