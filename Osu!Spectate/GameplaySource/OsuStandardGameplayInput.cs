using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using OsuSpectate.Audio;
using OsuSpectate.Beatmap;
using OsuSpectate.GameplaySource;
using OsuSpectate.Skin;
using OsuSpectate.View;

using ReplayAPI;
using OsuSpectate.GameplayEngine;

namespace OsuSpectate.GameplaySource
{
    public interface OsuStandardGameplayInput
    {
        string GetPlayerName();
        Mods GetMods();

        OsuStandardGameplayFrame GetGameplayFrame(TimeSpan time);
        OsuStandardGameplayFrame GetGameplayFrame(long milliseconds);
        ReplayFrame GetReplayFrame(TimeSpan time);
        ReplayFrame GetReplayFrame(long milliseconds);
        
        TimeSpan GetOD300Milliseconds();
        TimeSpan GetOD100Milliseconds();
        TimeSpan GetOD50Milliseconds();
        TimeSpan GetARMilliseconds();
        float GetCSRadius();

        OsuStandardBeatmap GetBeatmap();
        void HandleUntil(TimeSpan time);

        List<RenderObject> GetRenderList();
    }
}
