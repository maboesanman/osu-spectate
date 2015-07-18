using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Game(int w, int h)
            : base(w, h)
        {
            GL.Enable(EnableCap.Texture2D);

        }



        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            base.Title = "osu!spectate";
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            base.Title = "osu!spectate";




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
       //     music.kill();
            //do i need to do any opengl stuff here?
        }
    }
}
