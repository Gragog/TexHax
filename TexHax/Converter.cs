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
    class Converter
    {
        Process proc = new Process();
        string target;
        string[] files;
        string cParams = @"-printinfo";

        // for %%f in (Convert/*.gtx) do texconv2 -i Convert/%%f -o OutDDS/%%~nf.dds -printinfo
        public void Convert()
        {
            GetTarget();

            Prepare();

            files = Directory.GetFiles(@"Extracted\" + target + @"\");
            RunProgram();
        }

        private void GetTarget()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("Which folder inside of 'Extracted' to convert?");

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
                    Console.WriteLine("Your input contains special characters or including spaces, which is now allowed.");
                    continue;
                }

                if (isRegexValid && Directory.Exists(@"Extracted\" + input + @"\"))
                {
                    validInput = true;
                    target = input;
                }
                else Console.WriteLine(@"Extracted\" + input + " does not exist!");
            }
        }

        private void Prepare()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\nPreparing:");

            if (!Directory.Exists(@"Converted\"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine("\n  Creating Folder:\n\n    creating 'Extracted\'...");
                Directory.CreateDirectory(@"Converted\");
                Console.WriteLine("\n   done");
            }
            else CleanUp();

            ReCreateFolders();

            Sleep(1500);
        }

        private void CleanUp()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n  Cleaning up...");
            if (Directory.Exists(@"Converted\dds\" + target))
            {
                Console.WriteLine("    " + @"deleting 'Converted\dds\" + target + @"\'...");
                Directory.Delete(@"Converted\dds\" + target + @"\", true);
                Console.WriteLine("     done");
            }

            if (Directory.Exists(@"Converted\dds_lossy\" + target))
            {
                Console.WriteLine("    " + @"deleting 'Converted\dds_lossy\" + target + @"\'...");
                Directory.Delete(@"Converted\dds_lossy\" + target + @"\", true);
                Console.WriteLine("     done");
            }
            Console.WriteLine("\n  Cleaned up");
        }

        private void ReCreateFolders()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("\n  Recreating Folders:");

            Console.WriteLine("    " + @"creating 'Converted\dds\" + target + @"\'...");
            Directory.CreateDirectory(@"Converted\dds\" + target + @"\");
            Console.WriteLine("     done");

            Console.WriteLine("    " + @"creating 'Converted\dds_lossy\" + target + @"\'...");
            Directory.CreateDirectory(@"Converted\dds_lossy\" + target + @"\");
            Console.WriteLine("     done");

            Console.WriteLine("    " + @"creating 'Converted\dds_prepare\" + target + @"\'...");
            Directory.CreateDirectory(@"Converted\dds_prepare\" + target + @"\");
            Console.WriteLine("     done");

            Console.WriteLine("\n  Folders recreated");
        }

        private void RunProgram()
        {
            proc.StartInfo.FileName = @"res\texconv2.exe";
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.UseShellExecute = false;

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(
                "\n\nRunning 'quickbms.exe'...\n\n" +
                "======================================================================\n" +
                "======================================================================"
                );

            Sleep(500);

            Console.ForegroundColor = ConsoleColor.Cyan;
            ConvertToLossy();
            ConvertToLosslessPrepare();
            ConvertToLossless();

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(
                "\n" +
                "======================================================================\n" +
                "======================================================================\n"
                );

            Sleep(250);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(@"Deleting 'Converted\dds_prepare'");
            Directory.Delete(@"Converted\dds_prepare\", true);
            Console.WriteLine(" done\n");

            Console.ForegroundColor = ConsoleColor.Green;
        }

        private void ConvertToLossy()
        {
            foreach (string file in files)
            {
                string input = file;
                string output = file.Replace("Extracted", "Converted/dds_lossy").Replace("gtx", "dds");

                cParams = "-i " + input + " -o " + output + " -printinfo";

                proc.StartInfo.Arguments = cParams;
                proc.Start();
                StreamWriter mySW = proc.StandardInput;

                proc.WaitForExit();
            }
        }

        private void ConvertToLosslessPrepare()
        {
            foreach (string file in files)
            {
                string input = file;
                string output = file.Replace("Extracted", "Converted/dds_prepare/");

                cParams = "-i " + input + " -f GX2_SURFACE_FORMAT_TCS_R8_G8_B8_A8_UNORM" + " -o " + output + " -printinfo";

                proc.StartInfo.Arguments = cParams;
                proc.Start();
                StreamWriter mySW = proc.StandardInput;

                proc.WaitForExit();
            }
        }

        private void ConvertToLossless()
        {
            string[] prepareFiles = Directory.GetFiles(@"Converted\dds_prepare\" + target + @"\");

            foreach (string file in prepareFiles)
            {

                string input = file;
                string output = file.Replace(@"Converted\dds_prepare", "Converted/dds/").Replace("gtx", "dds");

                cParams = "-i " + input + " -o " + output;

                proc.StartInfo.Arguments = cParams;
                proc.Start();
                StreamWriter mySW = proc.StandardInput;

                proc.WaitForExit();
            }
        }
    }
}
