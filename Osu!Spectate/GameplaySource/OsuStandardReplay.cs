using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

using osuElements.Replays;
using osuElements.Helpers;
using osuElements.Beatmaps;

using OsuSpectate.Beatmap;
using OsuSpectate.GameplayEngine;

namespace OsuSpectate.GameplaySource
{
    public class OsuStandardReplay : Replay, OsuStandardGameplayInput
    {
        
        public OsuStandardBeatmap Beatmap;
        private BeatmapManager Manager;
        private SortedDictionary<TimeSpan, int> ReplayFrameIndex;
        private List<TimeSpan> ReplayFrameIndexKeys;
        private SortedDictionary<TimeSpan, int> LifeFrameIndex;
        private List<TimeSpan> LifeFrameIndexKeys;

        public OsuStandardGameplayEngine GameplayEngine;
        
        TimeSpan CurrentTime;
        

        public OsuStandardReplay(string replayFile, OsuStandardBeatmap beatmap, bool fullLoad = false) : base(replayFile, fullLoad)
        {
            ReadFile();
            Beatmap = beatmap;
            Manager = new BeatmapManager(Beatmap);
            Manager.SetMods(GetMods());
            Manager.SliderCalculations();
            Manager.CalculateDifficlty();
            Manager.CalculateStacking();
            ReplayFrameIndex = new SortedDictionary<TimeSpan, int>();
            for (int i = 0; i < ReplayFrames.Count(); i++)
            {
                ReplayFrameIndex[new TimeSpan(ReplayFrames.ElementAt(i).Time*TimeSpan.TicksPerMillisecond)]= i;
            }
            ReplayFrameIndexKeys = ReplayFrameIndex.Keys.ToList();

            LifeFrameIndex = new SortedDictionary<TimeSpan,int>();
            for (int i = 0; i <LifebarFrames.Count(); i++)
            {
                LifeFrameIndex[new TimeSpan(LifebarFrames.ElementAt(i).Time * TimeSpan.TicksPerMillisecond)] = i;
            }
            LifeFrameIndexKeys = LifeFrameIndex.Keys.ToList();
            GameplayEngine = new OsuStandardGameplayEngine(this);
            
            for( int i=1; i<ReplayFrames.Count;i++)
            {
                bool added = false;
                if (ReplayFrames[i].Keys!= ReplayFrames[i-1].Keys)
                {
                    if (ReplayFrames[i].Keys == ReplayKeys.None)
                    {
                        GameplayEngine.AddReleaseEvent(ReplayFrames[i]);
                    }
                    else
                    {
                        if (ReplayFrames[i].Keys.HasFlag(ReplayKeys.K1))
                        {
                            if (!ReplayFrames[i-1].Keys.HasFlag(ReplayKeys.K1))
                            {
                                if(!added)
                                    GameplayEngine.AddClickEvent(ReplayFrames[i]);
                                added = true;
                            }
                        }
                        if (ReplayFrames[i].Keys.HasFlag(ReplayKeys.K2))
                        {
                            if (!ReplayFrames[i - 1].Keys.HasFlag(ReplayKeys.K2))
                            {
                                if (!added)
                                    GameplayEngine.AddClickEvent(ReplayFrames[i]);
                                added = true;
                            }
                        }
                        if (ReplayFrames[i].Keys.HasFlag(ReplayKeys.M1))
                        {
                            if (!ReplayFrames[i - 1].Keys.HasFlag(ReplayKeys.M1))
                            {
                                if (!added)
                                    GameplayEngine.AddClickEvent(ReplayFrames[i]);
                                added = true;
                            }
                        }
                        if (ReplayFrames[i].Keys.HasFlag(ReplayKeys.M2))
                        {
                            if (!ReplayFrames[i - 1].Keys.HasFlag(ReplayKeys.M2))
                            {
                                if (!added)
                                    GameplayEngine.AddClickEvent(ReplayFrames[i]);
                                added = true;
                            }
                        }

                    }
                }
            }

            List<HitObject> ModdedHitObjects = Manager.GetHitObjects();
            for (int i = 0; i < ModdedHitObjects.Count; i++)
            {
                HitObject ho = ModdedHitObjects.ElementAt(i);
                if (ho.Type.HasFlag(HitObjectType.Slider))
                {
                    Beatmap.GetSliderTexture((Slider)ho, GetMods());
                }
            }
        }
        public string GetPlayerName()
        {
            return UserName;
        }
        public Mods GetMods()
        {
            return Enabled_Mods;
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
            //return ReplayFrames[ReplayFrameIndex[KeyNext]];
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
            //ResultFrame.Position = osuElements.Position.Lerp(Frame1.Position, Frame2.Position,timeScale);
            ResultFrame.Position = Frame1.Position;
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

        public BeatmapManager GetBeatmapManager()
        {
            return Manager;
        }
    }
}
