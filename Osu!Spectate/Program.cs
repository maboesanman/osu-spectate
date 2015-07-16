using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuSpectate
{
    class Program
    {
        static void Main(string[] args)
        {
            Game Window = new Game(640, 480);
            Window.Run();
        }
    }
}
