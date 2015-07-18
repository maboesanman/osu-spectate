using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuSpectate.View
{
    public class ViewArrangement
    {
        public List<ViewContainer> Views;

        public ViewArrangement()
        {
            Views = new List<ViewContainer>();
        }
        public void Draw(TimeSpan Time, int WindowWidth, int WindowHeight)
        {
            for (int i = 0; i < Views.Count; i++)
            {
                Views.ElementAt(i).Draw(Time, WindowWidth, WindowHeight);
            }
        }
    }
}
