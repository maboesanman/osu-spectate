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
        //
        //
        //TimeSpan offset = TimeSpan.FromSeconds(86) + TimeSpan.FromSeconds(24) + TimeSpan.FromSeconds(155);
        TimeSpan offset = TimeSpan.Zero;
        float rate = 1.0f;
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

            OsuSkin Skin = new OsuSkin(@"C:\Program Files (x86)\osu!\Skins\Aesthetic\", true);
            //OsuSkin Skin = new OsuSkin(@"\\", true);
            Beatmap = new OsuStandardBeatmap(@"C:\Program Files (x86)\osu!\Songs\93523 Tatsh - IMAGE -MATERIAL- Version 0\Tatsh - IMAGE -MATERIAL- Version 0 (Scorpiour) [Scorpiour].osu");
            //GameplayInputList.Add(new OsuStandardReplay(@"C:\Program Files (x86)\osu!\Replays\-GN - xi - FREEDOM DiVE [FOUR DIMENSIONS] (2015-12-20) Osu.osr", Beatmap, true));
            //GameplayInputList.Add(new OsuStandardReplay(@"C:\Program Files (x86)\osu!\Replays\cptnXn - xi - FREEDOM DiVE [FOUR DIMENSIONS] (2014-05-11) Osu.osr", Beatmap, true));
            //GameplayInputList.Add(new OsuStandardReplay(@"C:\Program Files (x86)\osu!\Replays\Cookiezi - xi - FREEDOM DiVE [FOUR DIMENSIONS] (2016-01-18) Osu.osr", Beatmap, true));

            //Beatmap = new OsuStandardBeatmap(@"C:\Program Files (x86)\osu!\Songs\203309 Ni-Sokkususu - Shukusai no Elementalia\Ni-Sokkususu - Shukusai no Elementalia (Silynn) [Kneesocks].osu");
            GameplayInputList.Add(new OsuStandardReplay(@"C:\Program Files (x86)\osu!\Replays\Cookiezi - Tatsh - IMAGE -MATERIAL- Version 0 [Scorpiour] (2015-12-08) Osu.osr", Beatmap, true));


            MyArrangement.Views.Add(new ViewContainer(-1f, -1f, 2f, 2f, new SongBackgroundView(Beatmap, .8f, Color.Black, BackgroundImageFitType.MAXIMUM_FIT)));
            MyArrangement.Views.Add(new ViewContainer(-1.0f, -1.0f, 2.0f, 2.0f, new OsuStandardGameplayView(Beatmap, GameplayInputList[0], Skin, Audio)));
            //MyArrangement.Views.Add(new ViewContainer(-1.0f + 2.0f / 3, -1.0f, 2.0f / 3, 2.0f, new OsuStandardGameplayView(Beatmap, GameplayInputList[1], Skin, Audio)));
            //MyArrangement.Views.Add(new ViewContainer(-1.0f+ 4.0f / 3, -1.0f, 2.0f/3, 2.0f, new OsuStandardGameplayView(Beatmap, GameplayInputList[2], Skin, Audio)));
            Audio = new AudioPlayer(GameplayInputList[0],rate);
            timer.Start();
            //VSync = VSyncMode.Off;
            //WindowState = WindowState.Fullscreen;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            //timer.Elapsed.Add(Audio.getCurrentTime().Subtract(timer.Elapsed));
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            for (int i=0;i<GameplayInputList.Count;i++)
            {
                GameplayInputList.ElementAt(i).HandleUntil(TimeSpan.FromMilliseconds(timer.Elapsed.TotalMilliseconds * rate).Add(offset));
            }
            MyArrangement.Draw(TimeSpan.FromMilliseconds(timer.Elapsed.TotalMilliseconds * rate).Add(offset), Width,Height);

            SwapBuffers();

            //Console.WriteLine((int)RenderFrequency);
            //System.GC.Collect();
            //System.GC.WaitForPendingFinalizers();

        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
            OnRenderFrame(new FrameEventArgs());//may delete this in the future
        }
        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            OnRenderFrame(new FrameEventArgs());//may delete this in the future
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            Audio.kill();
            //do i need to do any opengl stuff here?
        }
    }
}
