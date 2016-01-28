using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

using ReplayAPI;

using OsuSpectate.Beatmap;
using OsuSpectate.GameplayEngine;

namespace OsuSpectate.GameplaySource
{
    public class OsuStandardReplay : Replay, OsuStandardGameplayInput
    {
        
        public OsuStandardBeatmap Beatmap;

        private SortedDictionary<TimeSpan, int> ReplayFrameIndex;
        private List<TimeSpan> ReplayFrameIndexKeys;
        private SortedDictionary<TimeSpan, int> LifeFrameIndex;
        private List<TimeSpan> LifeFrameIndexKeys;

        private OsuStandardGameplayEngine GameplayEngine;
        
        TimeSpan CurrentTime;
        

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


            GameplayEngine = new OsuStandardGameplayEngine(beatmap, GetMods());
            
            for( int i=1; i<ReplayFrames.Count;i++)
            {
                if (ReplayFrames[i].Keys!= ReplayFrames[i-1].Keys)
                {
                    if (ReplayFrames[i].Keys == ReplayAPI.Keys.None)
                    {
                        new ReplayReleaseEvent(ReplayFrames[i]);
                    }
                    else
                    {
                        if ((ReplayFrames[i].Keys & ReplayAPI.Keys.K1) == ReplayAPI.Keys.K1)
                        {
                            if ((ReplayFrames[i - 1].Keys & ReplayAPI.Keys.K1) != ReplayAPI.Keys.K1)
                            {
                                GameplayEngine.AddClickEvent(ReplayFrames[i]);
                            }
                        }
                        if ((ReplayFrames[i].Keys & ReplayAPI.Keys.K2) == ReplayAPI.Keys.K2)
                        {
                            if ((ReplayFrames[i - 1].Keys & ReplayAPI.Keys.K2) != ReplayAPI.Keys.K2)
                            {
                                GameplayEngine.AddClickEvent(ReplayFrames[i]);
                            }
                        }
                        if ((ReplayFrames[i].Keys & ReplayAPI.Keys.M1) == ReplayAPI.Keys.M1)
                        {
                            if ((ReplayFrames[i - 1].Keys & ReplayAPI.Keys.M1) != ReplayAPI.Keys.M1)
                            {
                                GameplayEngine.AddClickEvent(ReplayFrames[i]);
                            }
                        }
                        if ((ReplayFrames[i].Keys & ReplayAPI.Keys.M2) == ReplayAPI.Keys.M2)
                        {
                            if ((ReplayFrames[i - 1].Keys & ReplayAPI.Keys.M2) != ReplayAPI.Keys.M2)
                            {
                                GameplayEngine.AddClickEvent(ReplayFrames[i]);
                            }
                        }

                    }
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
        public OsuStandardGameplayFrame GetGameplayFrame(TimeSpan t)
        {
            return GameplayEngine.getGameplayFrame(t);
        }
        public OsuStandardGameplayFrame GetGameplayFrame(long milliseconds)
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
            GameplayEngine.HandleUntil(time);
        }
        public OsuStandardBeatmap GetBeatmap()
        {
            return Beatmap;
        }
        public List<RenderObject> GetRenderList()
        {
            return GameplayEngine.getRenderList();
        }
    }
}
