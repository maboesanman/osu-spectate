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
    interface OsuStandardGameplayInput
    {
        public GameplayFrame GetGameplayFrame(TimeSpan time);
        public string GetPlayerName();
        public OsuStandardBeatmap GetBeatmap();
        public Mods GetMods();
    }
}
