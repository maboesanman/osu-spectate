using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuSpectate.GameplayEngine
{
    public struct OsuStandardGameplayFrame : IComparable<OsuStandardGameplayFrame>
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

        public OsuStandardGameplayFrame(TimeSpan t)
        {
            Count100 = 0;
        Count300=0;
        Count50=0;
        CountGeki=0;
        CountKatu=0;
        CountMiss=0;
        Life=0.0f;
        Combo=0;
        Score=0;
        Time = t;
        }

        public int CompareTo(OsuStandardGameplayFrame other)
        {
            return this.Time.CompareTo(other.Time);
        }
        
    }
}
