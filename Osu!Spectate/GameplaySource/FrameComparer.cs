using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ReplayAPI;

namespace OsuSpectate.GameplaySource
{
    class ReplayFrameComparer : IComparer<ReplayFrame>
    {
        public int Compare(ReplayFrame Frame1, ReplayFrame Frame2)
        {
            return Frame1.Time.CompareTo(Frame2.Time);
        }
    }
    class LifeFrameComparer : IComparer<LifeFrame>
    {
        public int Compare(LifeFrame Frame1, LifeFrame Frame2)
        {
            return Frame1.Time.CompareTo(Frame2.Time);
        }
    }
    class GameplayFrameComparer : IComparer<GameplayFrame>
    {
        public int Compare(GameplayFrame Frame1, GameplayFrame Frame2)
        {
            return Frame1.Time.CompareTo(Frame2.Time);
        }
    }
}
