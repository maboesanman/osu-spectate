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
        public View MyView;
        public ViewContainer(float x, float y, float w, float h, View v)
        {
            OriginX = x;
            OriginY = y;
            Width = w;
            Height = h;
            MyView = v;
        }

        
        public void Draw(TimeSpan Time, int WindowWidth, int WindowHeight)
        {
            MyView.Draw(Time, OriginX, OriginY, Width, Height, WindowWidth, WindowHeight);
        }
    }
}
