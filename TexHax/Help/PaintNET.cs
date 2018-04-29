using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexHax.Help
{
    class PaintNET
    {
        public void Help()
        {
            Console.WriteLine(
                @"Edit the .dds using Paint.NET
save with these parameters:
 - DTX5 (Interpolated Alpha)
 - Iterative Adjustmend (slow/HQ)
 - Perceptive
 - Generate MipMaps
");
        }
    }
}
