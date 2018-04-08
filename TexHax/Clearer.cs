using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TexHax
{
    class Clearer
    {
        string bfresStuff = "";

        public void Clear(bool wipe)
        {
            if (!wipe) GetBfresToClearStuff();
            if (wipe) if (!AskForWipe()) return;

            int counter = 0;
            long sizeBfres = 0;
            long sizeExtracted = 0;
            long sizeConvertedDds = 0;
            long sizeConvertedDdsLossy = 0;

            Console.ForegroundColor = ConsoleColor.Red;

            if (wipe)
            {
                if (Directory.Exists(@"bfres\"))
                {
                    DirectoryInfo bfresInfo = new DirectoryInfo(@"bfres\");
                    sizeBfres = bfresInfo.EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(file => file.Length);

                    Console.WriteLine(@"deleting 'bfres\'");
                    Directory.Delete(@"bfres\", true);
                    Console.WriteLine(" done (deleted " + sizeBfres + " bytes)\n");
                    counter++;
                }
            }

            if (Directory.Exists(@"Extracted\" + bfresStuff))
            {
                if (counter == 0) Console.WriteLine("");

                DirectoryInfo extractedInfo = new DirectoryInfo(@"Extracted\" + bfresStuff + @"\");
                sizeExtracted = extractedInfo.EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(file => file.Length);

                Console.WriteLine(@"deleting 'Extracted\" + bfresStuff + @"\'");
                Directory.Delete(@"Extracted\" + bfresStuff, true);
                Console.WriteLine(" done (deleted " + sizeExtracted + " bytes)\n");
                counter++;
            }

            if (Directory.Exists(@"Converted\dds\" + bfresStuff))
            {
                if (counter == 0) Console.WriteLine("");

                DirectoryInfo ConvertedDdsInfo = new DirectoryInfo(@"Converted\dds\" + bfresStuff + @"\");
                sizeConvertedDds = ConvertedDdsInfo.EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(file => file.Length);

                Console.WriteLine(@"deleting 'Converted\dds\" + bfresStuff + @"\'");
                Directory.Delete(@"Converted\dds\" + bfresStuff, true);
                Console.WriteLine(" done (deleted " + sizeConvertedDds + " bytes)\n");
                counter++;
            }

            if (Directory.Exists(@"Converted\dds_lossy\" + bfresStuff))
            {
                if (counter == 0) Console.WriteLine("");

                DirectoryInfo ConvertedDdsLossyInfo = new DirectoryInfo(@"Converted\dds_lossy\" + bfresStuff + @"\");
                sizeConvertedDdsLossy = ConvertedDdsLossyInfo.EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(file => file.Length);

                Console.WriteLine(@"deleting 'Converted\dds_lossy\" + bfresStuff + @"\'");
                Directory.Delete(@"Converted\dds_lossy\" + bfresStuff, true);
                Console.WriteLine(" done (deleted " + sizeConvertedDdsLossy + " bytes)\n");
                counter++;
            }

            Console.WriteLine("\nDeleted " + counter + " folders\nTotal size of " + (sizeBfres + sizeExtracted + sizeConvertedDds + sizeConvertedDdsLossy).ToString() + " bytes\n");
        }

        private void GetBfresToClearStuff()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("\nClear stuff from which .bfres?");

            Regex regexItem = new Regex("^[a-zA-Z0-9_-]{1,}$");

            bool isRegexValid = false;
            string input;
            bool validInput = false;
            while (!validInput)
            {
                isRegexValid = false;

                Console.ForegroundColor = ConsoleColor.Magenta;
                input = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Red;

                if (regexItem.IsMatch(input)) isRegexValid = true;
                else
                {
                    Console.WriteLine("Your input contains special characters.\nPlease remove any of them from the .bfres file, including spaces.");
                    continue;
                }

                if (isRegexValid && File.Exists(@"bfres\" + input + ".bfres"))
                {
                    validInput = true;
                    bfresStuff = input;
                }
                else Console.WriteLine(@"bfres\" + input + ".bfres does not exist!");
            }
        }

        /*public void Wipe()
        {
            if (!AskForWipe()) return;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("WIPING...");

            if (Directory.Exists(@"bfres\"))
            {
                Console.WriteLine(@"  deleting 'bfres\'");
                Directory.Delete(@"bfres\", true);
                Console.WriteLine("   done");
            }

            if (Directory.Exists(@"Extracted\"))
            {
                Console.WriteLine(@"  deleting 'Extracted\'");
                Directory.Delete(@"Extracted\", true);
                Console.WriteLine("   done");
            }

            if (Directory.Exists(@"Converted\"))
            {
                Console.WriteLine(@"  deleting 'Converted\'");
                Directory.Delete(@"Converted\", true);
                Console.WriteLine("   done");
            }

            Console.WriteLine("");
        }*/

        private bool AskForWipe()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Wiping will delete all folders that are that were created by this program\nThe 'szs' and 'Finished' folders will not be deleted\nDo you want to wipe? (y/N)");

            Console.ForegroundColor = ConsoleColor.Magenta;
            if (Console.ReadLine().ToLower() == "y") return true;

            return false;
        }
    }
}
