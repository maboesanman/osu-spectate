using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuSpectate.View
{
    class OsuStandardBalancedMultiView : View
    {
        public List<OsuStandardGameplayView> views;
        public bool centerHorizontal = true;
        public bool flipViews = false;
        public bool stretchViews = false;
        int gridWidth=2;
        int gridHeight=2;
        float currentHeight = 0;
        float currentWidth = 0;

        public OsuStandardBalancedMultiView(bool centerH, bool stretch, bool flip)
        {
            views = new List<OsuStandardGameplayView>();
            centerHorizontal = centerH;
            flipViews = flip;
            stretchViews = stretch;
        }

        public void Draw(TimeSpan time, float x, float y, float width, float height, int windowWidth, int windowHeight)
        {
            
            if(!((currentHeight==windowHeight*height)&&(currentWidth==windowWidth*width)))
            {
                currentHeight = windowHeight * height;
                currentWidth = windowWidth * width;

                List<int> potentialWidth = new List<int>();
                List<int> potentialHeight = new List<int>();
                for (int m = 1; m <= views.Count; m++)
                {
                    for (int n = 1; n * m -Math.Min(n, m) < views.Count; n++)
                    {
                        if(m*n>=views.Count)
                        {
                            potentialHeight.Add(m);
                            potentialWidth.Add(n);
                        }
                    }
                }
                float wastedSpace = width * height*windowWidth*windowHeight;
                float temp;
                for(int i=0;i<potentialHeight.Count;i++)
                {
                    temp = WastedSpace(potentialWidth.ElementAt(i), potentialHeight.ElementAt(i), width * windowWidth, height * windowHeight, views.Count);
                    if (temp<wastedSpace)
                    {
                        wastedSpace = temp;
                        gridHeight = potentialHeight.ElementAt(i);
                        gridWidth = potentialWidth.ElementAt(i);
                    }
                }
            }
            if (centerHorizontal)
            {
                int[] rowCounts = new int[gridHeight];
                for(int j=0;j<gridHeight;j++)
                {
                    rowCounts[j] = gridWidth;
                }
                for(int j=0;j<gridWidth*gridHeight-views.Count;j++)
                {
                    if((rowCounts[0]==rowCounts[gridHeight-1])^flipViews)
                    {
                        rowCounts[0]--;
                    }
                    else
                    {
                        rowCounts[gridHeight - 1]--;
                    }
                }
                int i = 0;
                for(int relativeY=0;relativeY<gridHeight;relativeY++)
                {
                    float shiftX = (gridWidth - rowCounts[relativeY]) * .5f;
                    for (int relativeX = 0; relativeX < rowCounts[relativeY]; relativeX++)
                    {
                        if (!stretchViews)
                        {
                            views.ElementAt(i++).Draw(time, x + (relativeX + shiftX) * width / gridWidth, y + (gridHeight - 1 - relativeY) * height / gridHeight, width / gridWidth, height / gridHeight, windowWidth, windowHeight);
                        }
                        else
                        {
                            views.ElementAt(i++).Draw(time, x + relativeX * width / rowCounts[relativeY], y + (gridHeight - 1 - relativeY) * height / gridHeight, width / rowCounts[relativeY], height / gridHeight, windowWidth, windowHeight);
                        }
                    }
                }
            } else
            {
                int[] colCounts = new int[gridWidth];
                for (int j = 0; j < gridWidth; j++)
                {
                    colCounts[j] = gridHeight;
                }
                for (int j = 0; j < gridWidth * gridHeight - views.Count; j++)
                {
                    if ((colCounts[0] == colCounts[gridWidth - 1])^flipViews)
                    {
                        colCounts[0]--;
                    }
                    else
                    {
                        colCounts[gridWidth - 1]--;
                    }
                }
                int i = 0;
                for (int relativeX = 0; relativeX < gridWidth; relativeX++)
                {
                    float shiftY = (gridHeight - colCounts[relativeX]) * .5f;
                    for (int relativeY = 0; relativeY < colCounts[relativeX]; relativeY++)
                    {
                        if (!stretchViews)
                        {
                            views.ElementAt(i++).Draw(time, x + (relativeX) * width / gridWidth, y + (gridHeight - 1 - (relativeY + shiftY)) * height / gridHeight, width / gridWidth, height / gridHeight, windowWidth, windowHeight);
                        }
                        else
                        {
                            views.ElementAt(i++).Draw(time, x + (relativeX) * width / gridWidth, y + (colCounts[relativeX] - 1 - (relativeY)) * height / colCounts[relativeX], width / gridWidth, height / colCounts[relativeX], windowWidth, windowHeight);
                        }
                    }
                }
            }
        }
        private float WastedSpace(int n, int m, float w, float h, int x)
        {
            float area = 0.0f;
            if(19*h*n<15*w*m)
            {
                area = ((w / n) - (15 * h) / (19 * m)) * w / m;
            }
            if(19*h*n>15*w*m)
            {
                area = ((h / m) - (15 * w) / (19 * n)) * w / n;
            }
            area = area * x;
            area += ((m * n - x) * w * h / m / n);
            return area;
        }
    }
}
