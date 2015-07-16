using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuSpectate.GameplaySource
{
    public struct GameplayFrame
    {
        public ushort Count100;
        public ushort Count300;
        public ushort Count50;
        public ushort CountGeki;
        public ushort CountKatu;
        public ushort CountMiss;
        public float Life;
        public ushort Combo;
        public uint Score;
        public TimeSpan Time;
    }
}
