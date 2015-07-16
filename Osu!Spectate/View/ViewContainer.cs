using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuSpectate.View
{
    public class ViewContainer
    {
        public float OriginX;
        public float OriginY;
        public float Width;
        public float Height;
        private View View;
        public ViewContainer(float x, float y, float w, float h, View v)
        {
            OriginX = x;
            OriginY = y;
            Width = w;
            Height = h;
            View = v;
        }
        public void draw(TimeSpan Time, int WindowWidth, int WindowHeight)
        {
            View.Draw(Time, OriginX, OriginY, Width, Height, WindowWidth, WindowHeight);
        }
    }
}
