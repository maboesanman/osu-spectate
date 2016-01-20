﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace OsuSpectate
{
    public class Computation
    {
        public static float XComputation(float x, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight)
        {
            float xrelative;
            float w = width * windowWidth;
            float h = height * windowHeight;
            if(w/h>19.0f/15.0f)
            {
                xrelative = ((x + 48.0f) / (608.0f) * (h * 19.0f / 15.0f) + ((w - h * 19.0f / 15.0f) / (2.0f))) / w;
            }
            else
            {
                xrelative = (x + 48.0f) / (608.0f);
            }
            return width * xrelative + OriginX;
        }
        public static float YComputation(float y, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight)
        {
            float yrelative;
            float w = width * windowWidth;
            float h = height * windowHeight;
            if (w / h > 19.0f / 15.0f)
            {
                yrelative = (384-y + 48.0f) / (480.0f);
            }
            else
            {
                yrelative = ((384-y + 48.0f) / (480.0f) * (w * 15.0f / 19.0f) + ((h - w * 15.0f / 19.0f) / (2.0f))) / h;
            }
            return height * yrelative + OriginY;
        }
        /*
        public static float XComputation(float x, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight)
        {
            if (windowWidth * width / windowHeight / height < 19.0f / 15.0f)
            {
                return ((x + 48) / 608.0f * width * windowWidth) / windowWidth + OriginX;
            }
            else
            {
                return (width * windowWidth / 2.0f - height * windowHeight * 5.0f / 9.0f + (x / 512.0f) * height * windowHeight * 5.0f / 4.5f) / windowWidth + OriginX;
            }
        }
        public static float YComputation(float y, float OriginX, float OriginY, float width, float height, int windowWidth, int windowHeight)
        {
            if (windowWidth * width / windowHeight / height < 10.0f / 9.0f)
            {
                return (height * windowHeight / 2.0f - width * windowWidth * 3.0f / 8.0f + (1.0f - y / 384.0f) * width * windowWidth * 3.0f / 4.0f) / windowHeight + OriginY;
            }
            else
            {
                return (height * windowHeight / 2.0f - height * windowHeight * 5.0f / 12.0f + (1.0f - y / 384.0f) * height * windowHeight * 5.0f / 6.0f) / windowHeight + OriginY;
            }
        }
        */
        public static float Distance(PointF p1, PointF p2)
        {
            return (float)Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }
    }
}
