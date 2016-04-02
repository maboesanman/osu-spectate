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

using osuElements.Db;
using osuElements.Replays;

namespace OsuSpectate
{
    public class Game : GameWindow
    {
        List<OsuStandardGameplayInput> GameplayInputList;
        ViewArrangement MyArrangement;
        AudioPlayer Audio;
        OsuStandardBeatmap Beatmap;
        Stopwatch timer = new Stopwatch();
        OsuDb songDB;
        //
        //
        TimeSpan offset = TimeSpan.Zero;
        
        float rate = 1.0f;
        float modRate = 1.0f;
        public Game(int w, int h)
            : base(w, h)
        {

            //offset = offset + TimeSpan.FromSeconds(20);
            //offset = offset + TimeSpan.FromSeconds(24);
            //offset = offset + TimeSpan.FromSeconds(155);
            songDB = new OsuDb();
            Console.WriteLine("Finished Reading osu.db");
            songDB.ReadFile();
            GL.Enable(EnableCap.Texture2D);
            GameplayInputList = new List<OsuStandardGameplayInput>();
            MyArrangement = new ViewArrangement();
        }



        protected override void OnLoad(EventArgs e)
        {
            
            base.OnLoad(e);
            base.Title = "osu!spectate";
            string ReplayPath = @"C:\Program Files (x86)\osu!\Replays\rrtyui - The Quick Brown Fox - The Big Black [WHO'S AFRAID OF THE BIG BLACK] (2014-05-17) Osu.osr";
            OsuSkin Skin= new OsuSkin(@"C:\Program Files (x86)\osu!\Skins\Aesthetic\", true);
            Replay temp = new Replay(ReplayPath, false);
            DbBeatmap dbb = songDB.FindHash(temp.BeatmapHash);
            Beatmap = new OsuStandardBeatmap(dbb.FullPath);
            temp = null;
            dbb = null;
            GameplayInputList.Add(new OsuStandardReplay(ReplayPath, Beatmap, true));
            if (GameplayInputList.ElementAt(0).GetMods().HasFlag(osuElements.Helpers.Mods.DoubleTime))
            {
                modRate = 1.5f;
            }
            if (GameplayInputList.ElementAt(0).GetMods().HasFlag(osuElements.Helpers.Mods.HalfTime))
            {
                modRate = .75f;
            }

            //GameplayInputList.Add(new OsuStandardReplay(@"C:\Program Files (x86)\osu!\Replays\rrtyui - Saiya - Remote Control [Insane] (2015-03-06) Osu.osr", Beatmap, true));
            //GameplayInputList.Add(new OsuStandardReplay(@"C:\Program Files (x86)\osu!\Replays\Rafis - Saiya - Remote Control [Insane] (2015-12-27) Osu.osr", Beatmap, true));
            //GameplayInputList.Add(new OsuStandardReplay(@"C:\Program Files (x86)\osu!\Replays\Cookiezi - Saiya - Remote Control [Insane] (2015-12-31) Osu.osr", Beatmap, true));

            MyArrangement.Views.Add(new ViewContainer(-1f, -1f, 2f, 2f, new SongBackgroundView(Beatmap, .8f, Color.Black, BackgroundImageFitType.MAXIMUM_FIT)));

            OsuStandardBalancedMultiView multiview = new OsuStandardBalancedMultiView(true, false,false);

            MyArrangement.Views.Add(new ViewContainer(-1f, -1f, 2f, 2f, multiview));

            multiview.views.Add(new OsuStandardGameplayView(Beatmap, GameplayInputList[0], Skin, Audio));
            
            /*
            GameplayInputList[0].HandleUntil(TimeSpan.FromDays(1));
            ((OsuStandardReplay)GameplayInputList[0]).GameplayEngine.OffsetList.Sort();
            for(int i=0;i< ((OsuStandardReplay)GameplayInputList[0]).GameplayEngine.OffsetList.Count;i++)
            {
                Console.WriteLine(((OsuStandardReplay)GameplayInputList[0]).GameplayEngine.OffsetList[i].TotalMilliseconds);
            }
            Console.WriteLine();
            Console.WriteLine(((OsuStandardReplay)GameplayInputList[0]).GameplayEngine.GetOD300Milliseconds().TotalMilliseconds);
            Console.WriteLine(((OsuStandardReplay)GameplayInputList[0]).GameplayEngine.GetOD100Milliseconds().TotalMilliseconds);
            Console.WriteLine(((OsuStandardReplay)GameplayInputList[0]).GameplayEngine.GetOD50Milliseconds().TotalMilliseconds);

            Console.ReadKey();
            */
            Audio = new AudioPlayer(GameplayInputList[0],rate*modRate);
            
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
                GameplayInputList.ElementAt(i).HandleUntil(TimeSpan.FromMilliseconds(timer.Elapsed.TotalMilliseconds * rate*modRate).Add(offset));
            }
            MyArrangement.Draw(TimeSpan.FromMilliseconds(timer.Elapsed.TotalMilliseconds * rate*modRate).Add(offset), Width,Height);

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
