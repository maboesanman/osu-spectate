using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ReplayAPI;

namespace OsuSpectate.GameplaySource
{
    public class OsuStandardReplay : Replay, OsuStandardGameplayInput
    {
        public List<GameplayFrame> GameplayFrames;
        public OsuStandardReplay(string replayFile, bool fullLoad = false) : base(replayFile, fullLoad)
        {
            ReplayFrames.Sort(new ReplayFrameComparer());
            LifeFrames.Sort(new LifeFrameComparer());
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
            
            return GameplayFrames.ElementAt(GameplayFrames.BinarySearch(new GameplayFrame { Time = t }));

        }
        public GameplayFrame GetGameplayFrame(long milliseconds)
        {
            return GetGameplayFrame(new TimeSpan(milliseconds * TimeSpan.TicksPerMillisecond));
        }


        public ReplayFrame GetReplayFrame(TimeSpan time)
        {
            throw new NotImplementedException();
        }

        public ReplayFrame GetReplayFrame(long milliseconds)
        {
            throw new NotImplementedException();
        }

        public TimeSpan GetOD300Milliseconds()
        {
            throw new NotImplementedException();
        }

        public TimeSpan GetOD100Milliseconds()
        {
            throw new NotImplementedException();
        }

        public TimeSpan GetOD50Milliseconds()
        {
            throw new NotImplementedException();
        }

        public TimeSpan GetARMilliseconds()
        {
            throw new NotImplementedException();
        }

        public float GetCSRadius()
        {
            throw new NotImplementedException();
        }

        public OsuStandardEventList GetEventList()
        {
            throw new NotImplementedException();
        }

        public Beatmap.OsuStandardBeatmap GetBeatmap()
        {
            throw new NotImplementedException();
        }
    }
}
