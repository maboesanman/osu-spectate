using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

using OsuSpectate.Audio;
using OsuSpectate.Beatmap;
using OsuSpectate.GameplaySource;
using OsuSpectate.Skin;
using OsuSpectate.View;

namespace OsuSpectate
{
    public class Game : GameWindow
    {
        List<OsuStandardGameplayInput> GameplayInputList;
        ViewArrangement MyArrangement;
        AudioPlayer Audio;
        OsuStandardBeatmap Beatmap;
        Stopwatch timer = new Stopwatch();

        public Game(int w, int h)
            : base(w, h)
        {
            GL.Enable(EnableCap.Texture2D);
            GameplayInputList = new List<OsuStandardGameplayInput>();
            MyArrangement = new ViewArrangement();
        }



        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            base.Title = "osu!spectate";
            Beatmap = new OsuStandardBeatmap(@"C:\Program Files (x86)\osu!\Songs\177663 Utagumi Setsugetsuka - Maware! Setsugetsuka chiptune Remix\Utagumi Setsugetsuka - Maware! Setsugetsuka chiptune Remix (jonathanlfj) [MawareXtrA].osu");
            GameplayInputList.Add(new OsuStandardReplay(@"C:\Program Files (x86)\osu!\Replays\[Toy] - Utagumi Setsugetsuka - Maware! Setsugetsuka chiptune Remix [MawareXtrA] (2015-06-06) Osu.osr",Beatmap, true));
            MyArrangement.Views.Add(new ViewContainer(-1.0f,-1.0f,2.0f,2.0f,new SongBackgroundView(Beatmap,200,Color.Black,0)));
            Audio = new AudioPlayer(GameplayInputList.ElementAt(0));
            timer.Start();
            timer.Elapsed.Add(Audio.getCurrentTime().Subtract(timer.Elapsed));
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            base.Title = "osu!spectate";

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(Color.Black);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            for(int i=0;i<GameplayInputList.Count;i++)
            {
                GameplayInputList.ElementAt(i).HandleUntil(timer.Elapsed);
            }
            MyArrangement.Draw(new TimeSpan(0L),Width,Height);

            SwapBuffers();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();

        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            Audio.kill();
            //do i need to do any opengl stuff here?
        }
    }
}
