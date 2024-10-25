using System;
using System.Collections.Generic;
using System.Text;

namespace JeffJamGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (MainGame mg = new MainGame())
                mg.Run();
        }
    }
}
