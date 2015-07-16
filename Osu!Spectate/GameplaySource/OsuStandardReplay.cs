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

        }
    }
}
