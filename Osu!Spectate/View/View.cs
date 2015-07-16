﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuSpectate.View
{
    public interface View
    {
        void Draw(TimeSpan time, float x, float y, float width, float height, int windowWidth, int windowHeight);
    }
}
