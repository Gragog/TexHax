using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexHax.Help
{
    class Basics
    {
        public void Help()
        {
            Console.WriteLine(
                @"- Start by getting a .szs file and put it in the szs folder.

- If you've a .bfres file, put it in the bfres folder.

- Next, decode the .szs by running mode '1 - Decode .szs' from the main menu,
    which will give you the .bfres from the .szs file.

- After that, you can extract the raw textures from that .bfres.
    Run '2 - Extract from .bfres' from the main menu.
    This will extract the textures into the 'Extracted' folder.

- Now you'll need to convert the raw .gtx files into a known file-format
    by selecting '3 - Convert extracted .gtx to .dds' from the main menu.
    The converted files are in 'Extracted/dds/[bfres_name]'

- To edit a texture, simply open a .dds file in a graphic program that supports .dds
    for example Paint.NET. Photoshot and GIMP are also possible, but they need a plugin for .dds files
    (read the 'Saving edited files' help for the parameters the new .dds needs to have)
    (NOTE: do NOT use .dds files from 'Converted/dds_lossy', they won't work!)

- After editing and saving a texture, you need to import it into the .bfres again.
    use '4 - Import .dds into a .bfres' from the main menu.
    Start by listing the texture positions and remember the one you want to import (or write it down).
    Next, run '4' again, but select 'Import .dds into .bfres'. This will ask you for your edited .dds.
    The file you enter must be in the folder 'Converted/dds/[bfres_name]'.

- When you have imported all textures you wanted to, the final step is to put your .bfres back to a .szs
    Select '5 - Pack a .bfres into .szs', which let you choose between two ways of how to compress it
    - 'fast' is meant for small files (< 10 MB). Compressing bigger files using 'fast' might
        let your console crash when loading the file. It also increases the file size.
    - 'big file' takes some time, but gives smaller results that will work for big files
    The new .szs is put to 'Finished/szs'
");
        }
    }
}
