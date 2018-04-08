using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexHax.Help
{
    class PaintNET
    {
        public void Menu()
        {
            DdsFiles();
        }

        private void DdsFiles()
        {
            Console.WriteLine(
                "Edit the .dds file and save with:" +
                "\nGTX5 (interpolated Alpha)" +
                "\niterative adjustment (slow / HQ)" +
                "\nperceptive" +
                "\ngenerate MipMaps"
            );
        }
    }
}
