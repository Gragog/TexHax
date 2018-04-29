using System;
using System.IO;
using System.Diagnostics;
using static System.Threading.Thread;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TexHax
{
    class Extractor
    {
        string bfresFile;

        public void Extract()
        {
            //string path = GetPath();

            GetBfresToExtract();

            Prepare();

            /* if (!File.Exists(@"bfres_backup\" + bfresFile + ".bfres"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine("\n\n  Backing up " + bfresFile + ".bfres");
                File.Copy(@"bfres\" + bfresFile + ".bfres", @"bfres_backup\" + bfresFile + ".bfres");
                Console.WriteLine("   done");
            } */

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(
                "\n\nRunning 'quickbms.exe'...\n\n" +
                "======================================================================\n" +
                "======================================================================"
                );

            Sleep(500);

            RunProgram();

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(
                "\n" +
                "======================================================================\n" +
                "======================================================================"
                );

            Sleep(250);
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("\n" + @"Renaming 'Extracted\" + bfresFile + @".bfres\'" + " to " + @"Extracted\" + bfresFile + @"\");
            Directory.Move(@"Extracted\" + bfresFile + @".bfres", @"Extracted\" + bfresFile + @"\");
            Console.WriteLine(" done\n");
        }

        private void GetBfresToExtract()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("Extract from which .bfres?");

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
                    bfresFile = input;
                }
                else Console.WriteLine(@"bfres\" + input + ".bfres does not exist!");
            }
        }

        private void Prepare()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\nPreparing:");

            CreateFolders();

            CleanUp();
        }

        private void CreateFolders()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  Creating Folders:");
            if (!Directory.Exists(@"Extracted\"))
            {
                Console.WriteLine("\n" + @"    creating 'Extracted\'...");
                Directory.CreateDirectory("Extracted");
                Console.WriteLine("     done");
            }

            /* if (!Directory.Exists(@"bfres_backup\"))
            {
                Console.WriteLine("\n" + @"    creating 'bfres_backup\'...");
                Directory.CreateDirectory("bfres_backup");
                Console.WriteLine("     done");
            } */
            Console.WriteLine("\n  All folders are there");
        }

        private void CleanUp()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n  Cleaning up...");
            if (Directory.Exists(@"Extracted\" + bfresFile + @".bfres\"))
            {
                Console.WriteLine("    " + @"deleting 'Extracted\" + bfresFile + @".bfres\'...");
                Directory.Delete(@"Extracted\" + bfresFile + @".bfres\", true);
                Console.WriteLine("     done");
            }
            if (Directory.Exists(@"Extracted\" + bfresFile + @"\"))
            {
                Console.WriteLine("    " + @"deleting 'Extracted\" + bfresFile + @"\'...");
                Directory.Delete(@"Extracted\" + bfresFile + @"\", true);
                Console.WriteLine("     done");
            }
            Console.Write("\n  Cleaned up");
        }

        private void RunProgram()
        {
            //string filename = Path.Combine(@"\", "quickbms.exe");
            Console.ForegroundColor = ConsoleColor.Cyan;
            string cParams = @"res\BFRES_Textures.bms bfres\" + bfresFile + @".bfres Extracted";
            Process proc = new Process();
            proc.StartInfo.FileName = @"res\quickbms.exe";
            proc.StartInfo.Arguments = cParams;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.Start();

            StreamWriter mySW = proc.StandardInput;

            proc.WaitForExit();
        }

        private string GetPath()
        {
            string path = System.Reflection.Assembly.GetEntryAssembly().Location;
            return path.Replace("TexHax.exe", "");
        }
    }
}
