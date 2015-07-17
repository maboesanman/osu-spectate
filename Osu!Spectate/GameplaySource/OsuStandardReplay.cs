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
            GameplayFrames.Sort(new GameplayFrameComparer());
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
    }
}
