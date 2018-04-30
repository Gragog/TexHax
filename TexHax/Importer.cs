using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;

namespace TexHax
{
    class Importer
    {
        string target = "";
        string ddsFile = "";
        string minMip = "";
        string texPos = "";
         int wait = Settings.import_waitForSec;
        string cParams = ""; // ddsWithExtenison + @" bfres\" + target + ".bfres " + Convert.ToInt32(minMipInt) + " " + Convert.ToInt32(texPosInt);

        public void Import()
        {
            SetTarget();

            string mode = GetMode();

            switch (mode)
            {
                case "1":
                    cParams = @"bfres\" + target + ".bfres";
                    break;
                case "2":
                    AskForDDS();
                    AskForMinMip();
                    AskForTexPos();
                    cParams = @"Converted\dds\" + target + @"\" + ddsFile + ".dds" +
                        @" bfres\" + target + ".bfres " +
                        minMip + " " +
                        texPos + " " +
                        wait;
                    // Console.WriteLine("Debug (cParams):   |" + cParams + "|  ");
                    break;
                case "3":
                    ImportAll();
                    CleanUp();
                    return;
            }

            //RunProgramPython();
            RunProgramNoPython();

            CleanUp();
        }

        private string GetMode()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine(
                "\nWhat to do?" +
                "\n1 - List texture positions" +
                "\n2 - Import .dds into .bfres" +
                "\n3 - Import all possible .dds into .bfres"
                );
            Regex regexItem = new Regex(@"^([1-3]{1})$");

            string input = "";

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                input = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Red;

                if (regexItem.IsMatch(input)) return input;

                Console.WriteLine("Not a valid mode.");
            }
        }

        private void RunProgramPython()
        {
            //string filename = Path.Combine(@"\", "quickbms.exe");
            Console.ForegroundColor = ConsoleColor.Cyan;
            string cParams = GetPath() + @"\res\hax.py " + GetPath() + @"\bfres\test.bfres";
            Process proc = new Process();
            proc.StartInfo.FileName = @"C:\Python27\python.exe";
            proc.StartInfo.Arguments = cParams;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.Start();

            StreamWriter mySW = proc.StandardInput;

            proc.WaitForExit();

            Console.WriteLine(cParams);
        }

        private void RunProgramNoPython()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(
                "\nRunning 'hax.exe'...\n\n" +
                "======================================================================\n" +
                "======================================================================\n"
                );

            Console.ForegroundColor = ConsoleColor.Cyan;

            Process proc = new Process();
            proc.StartInfo.FileName = @"res\hax\hax.exe";
            proc.StartInfo.Arguments = cParams;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = false; // exe output to my program todo
            proc.Start();

            // string[] test = new string[1000];
            StreamWriter mySW = proc.StandardInput;
            
            proc.WaitForExit();

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(
                "\n" +
                "======================================================================\n" +
                "======================================================================\n"
                );

            // Thread.Sleep(250);

            Console.ForegroundColor = ConsoleColor.Green;
        }

        void ImportAll()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Getting texture positions...\n");

            Console.ForegroundColor = ConsoleColor.Cyan;
            
            string procParams = @"bfres\" + target + ".bfres";

            Process proc = new Process();
            proc.StartInfo.FileName = @"res\hax\hax.exe";
            proc.StartInfo.Arguments = procParams;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true; // exe output to my program todo
            proc.Start();

            string[] output = proc.StandardOutput.ReadToEnd().Split('\n');

            /**
            while (!proc.StandardOutput.EndOfStream)
            {
                Console.WriteLine(proc.StandardOutput.ReadLine());
                list.Add(proc.StandardOutput.ReadLine());
            } */
            // https://stackoverflow.com/questions/4291912/process-start-how-to-get-the-output

            proc.WaitForExit();

            int i = 0;
            List<string> texturePositions = new List<string>();
            List<string> textureNames = new List<string>();

            Regex isNumber = new Regex(@"^\d+");
            foreach (string line in output)
            {
                if (isNumber.IsMatch(line))
                {
                    i++;
                    int whitespaceIndex = line.IndexOf(' ');
                    textureNames.Add(line.Substring(whitespaceIndex + 1));
                    texturePositions.Add(line.Substring(0, whitespaceIndex));
                }
            }

            Console.WriteLine("Found " + i + " texture positions\n\nGetting .dds files...");

            string[] ddsFiles = Directory.GetFiles(@"Converted\dds\" + target + @"\", "*.dds");

            foreach (string item in ddsFiles)
            {
                Console.WriteLine(item);

                /* cParams = @"Converted\dds\" + target + @"\" + item + ".dds" +
                        @" bfres\" + target + ".bfres " +
                        minMip + " " +
                        texPos + " " +
                        wait; */
            }
        }

        private string GetPath()
        {
            string path = System.Reflection.Assembly.GetEntryAssembly().Location;
            string fileName = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
            return path.Replace(@"\" + fileName, "");
        }

        private void SetTarget()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("Which .bfres to edit?");

            Regex regexItem = new Regex("^[a-zA-Z0-9_-]{1,}$");

            bool isRegexValid = false;
            string input;
            bool validInput = false;
            while (!validInput)
            {
                isRegexValid = false;

                Console.ForegroundColor = ConsoleColor.Magenta;
                input = Console.ReadLine().ToLower();
                Console.ForegroundColor = ConsoleColor.Red;

                if (regexItem.IsMatch(input)) isRegexValid = true;
                else
                {
                    Console.WriteLine("Your input contains special characters or including spaces, which is now allowed.");
                    continue;
                }

                if (isRegexValid && File.Exists(@"bfres\" + input + ".bfres"))
                {
                    target = input;
                    validInput = true;
                }
                else Console.WriteLine(@"bfres\" + input + ".bfres does not exist!");
            }
        }

        private void AskForDDS()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("\nWhich .dds to insert?\n.dds must not be added");

            Regex regexItem = new Regex("^[a-zA-Z0-9._-]{1,}$");

            bool isRegexValid = false;
            string input = "";
            bool validInput = false;
            while (!validInput)
            {
                isRegexValid = false;

                Console.ForegroundColor = ConsoleColor.Magenta;
                input = Console.ReadLine().ToLower();
                Console.ForegroundColor = ConsoleColor.Red;

                if (regexItem.IsMatch(input)) isRegexValid = true;
                else
                {
                    Console.WriteLine("Your input contains spaces or other not allowed special characters, which is now allowed.");
                    continue;
                }

                if (isRegexValid && File.Exists(@"Converted\dds\" + target + @"\" + input + ".dds"))
                {
                    validInput = true;
                }
                else Console.WriteLine(@"Converted\dds\" + target + @"\" + input + ".dds does not exist!");
            }

            ddsFile = input;
        }

        private void AskForMinMip()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("\nWhat is the minimal MipMap?\nLeave blank to let the application tell you the BFRES MinMip.\n(In that case, you need to reimport using that MinMip.)");

            Regex regexItem = new Regex(@"^\d*$");

            bool isRegexValid = false;
            string input = "";
            while (!isRegexValid)
            {
                isRegexValid = false;

                Console.ForegroundColor = ConsoleColor.Magenta;
                input = Console.ReadLine().ToLower();
                Console.ForegroundColor = ConsoleColor.Red;

                if (regexItem.IsMatch(input)) isRegexValid = true;
                else
                { // todo anpassen
                    Console.WriteLine("Your input must contain digits only.");
                    continue;
                }
            }

            if (input == "") minMip = "0";
            else minMip = input;
        }

        private void AskForTexPos()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("\nImport to which TexturePosition?\nIf you don't know, let them list for you in the other mode.");

            Regex regexItem = new Regex(@"^\d{1,3}$");

            bool isRegexValid = false;
            string input = "";
            while (!isRegexValid)
            {
                isRegexValid = false;

                Console.ForegroundColor = ConsoleColor.Magenta;
                input = Console.ReadLine().ToLower();
                Console.ForegroundColor = ConsoleColor.Red;

                if (regexItem.IsMatch(input)) isRegexValid = true;
                else
                { // todo anpassen
                    Console.WriteLine("Your input must contain digits only.");
                    continue;
                }
            }

            texPos = input;
        }

        private void CleanUp()
        {
            string[] gtxFiles = Directory.GetFiles(@"Converted\dds\" + target + "\\", "*.gtx");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Cleaninig up:\n");

            foreach (string gtxFile in gtxFiles)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("  deleting '" + gtxFile + "'...");
                File.Delete(gtxFile);
                Console.WriteLine("   done\n");
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Cleaned up\n");
        }
    }
}
