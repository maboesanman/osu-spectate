using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OsuSpectate.Audio;
using OsuSpectate.Beatmap;
using OsuSpectate.GameplaySource;
using OsuSpectate.Skin;
using OsuSpectate.View;

using ReplayAPI;

namespace OsuSpectate.GameplaySource
{
    public interface OsuStandardGameplayInput
    {
        public GameplayFrame GetGameplayFrame(TimeSpan time);
        public GameplayFrame GetGameplayFrame(long milliseconds);
        public string GetPlayerName();
        public OsuStandardBeatmap GetBeatmap();
        public Mods GetMods();
        public ReplayFrame GetReplayFrame(TimeSpan time);
        public ReplayFrame GetReplayFrame(long milliseconds);
        public TimeSpan GetOD300Milliseconds();
        public TimeSpan GetOD100Milliseconds();
        public TimeSpan GetOD50Milliseconds();
        public TimeSpan GetARMilliseconds();
        public float GetCSRadius();
        public OsuStandardEventList GetEventList();
    }
}
