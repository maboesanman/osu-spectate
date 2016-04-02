using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using osuElements.Replays;

namespace OsuSpectate.GameplayEngine
{
    public class GameplayFrameComparer : IComparer<OsuStandardGameplayFrame>
    {
        public int Compare(OsuStandardGameplayFrame Frame1, OsuStandardGameplayFrame Frame2)
        {
            return Frame1.Time.CompareTo(Frame2.Time);
        }
    }
}
