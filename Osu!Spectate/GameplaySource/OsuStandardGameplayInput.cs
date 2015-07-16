﻿using System;
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
        GameplayFrame GetGameplayFrame(TimeSpan time);
        GameplayFrame GetGameplayFrame(long milliseconds);
        string GetPlayerName();
        OsuStandardBeatmap GetBeatmap();
        Mods GetMods();
        ReplayFrame GetReplayFrame(TimeSpan time);
        ReplayFrame GetReplayFrame(long milliseconds);
        TimeSpan GetOD300Milliseconds();
        TimeSpan GetOD100Milliseconds();
        TimeSpan GetOD50Milliseconds();
        TimeSpan GetARMilliseconds();
        float GetCSRadius();
        OsuStandardEventList GetEventList();
    }
}
