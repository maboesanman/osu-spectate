using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

using OsuSpectate.Beatmap;

namespace OsuSpectate.GameplaySource
{
    public interface RenderObject
    {
        string GetType();
        void Initialize(float x, float y, float width, float height, int windowWidth, int windowHeight);
    }
    
    public class RenderHitCircle : RenderObject
    {
        public OsuStandardHitCircle HitCircle;
        public OsuStandardGameplayInput GameplayInput;
        private bool Initialized;
        public RenderHitCircle(OsuStandardHitCircle h, OsuStandardGameplayInput r)
        {
            HitCircle = h;
            GameplayInput = r;
        }
        public void Initialize(float x, float y, float width, float height, int windowWidth, int windowHeight)
        {
            Initialized = true;
        }
        public string GetType()
        {
            return "HitCircle";
        }
    }
    public class RenderSlider : RenderObject
    {
        public OsuStandardSlider Slider;
        public OsuStandardGameplayInput GameplayInput;
        public Nullable<int> SliderBorderTexture;
        private bool Initialized;
        public RenderSlider(OsuStandardSlider s, OsuStandardGameplayInput r)
        {
            GameplayInput = r;
            Slider = s;
        }
        public void computeTexture()
        {
            SliderBorderTexture = Slider.beatmap.GetSliderTexture(Slider, GameplayInput.GetMods());
        }
        public void Initialize(float x, float y, float width, float height, int windowWidth, int windowHeight)
        {

            Initialized = true;
        }
        public string GetType()
        {
            return "Slider";
        }
        public void kill()
        {
            
        }
    }
    public class RenderSpinner : RenderObject
    {
        public OsuStandardSpinner Spinner;
        public int SliderBorderTexture;
        private bool Initialized;
        public void Initialize(float x, float y, float width, float height, int windowWidth, int windowHeight)
        {

            Initialized = true;
        }
        public string GetType()
        {
            return "Spinner";
        }
        public void kill()
        {
            GL.DeleteTexture(SliderBorderTexture);
        }
    }
}
