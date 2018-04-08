using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TexHax
{
    class Encoder
    {
        string bfresFile = "";
        string szsTarget = "";
        string useFast = "";
        string encoder = "";
        bool write = true;

        public void Encode()
        {
            GetBfresToEncode();

            AskForMode();

            szsTarget = bfresFile;

            if (!Directory.Exists(@"Finished\"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Finished Folder does not exit.\nType 'f' from the main menu to create a finished .bfres\n");
                return;
            }

            // Creating 'Finished\'
            if (!Directory.Exists(@"Finished\szs\"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine("\nCreating 'Finished\\szs\\'");
                Directory.CreateDirectory(@"Finished\szs\");
                Console.WriteLine(" done");
            }

            while (File.Exists(@"Finished\szs\" + szsTarget + ".szs") && write == true) HandleConflict();

            if (write)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;

                if (encoder == "default")
                {
                    Console.WriteLine("\nEncoding 'Finished\\" + bfresFile + @".bfres' to 'Finished\" + szsTarget + ".bfres.yaz0'. This will take a while\n");

                    RunDefaultEncoder(bfresFile);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\nRenaming '" + szsTarget + ".bfres.yaz0' to '" + szsTarget + ".szs'...");
                    File.Move(@"Finished\" + bfresFile + ".bfres.yaz0", @"Finished\szs\" + szsTarget + ".szs");
                    Console.WriteLine(" done\n");
                    return;
                }

                Console.WriteLine("\nEncoding 'Finished\\" + bfresFile + @".bfres' to 'Finished\szs\" + szsTarget + ".szs'. This will take a while\n");

                RunInCodeEncoder();
            }
        }

        private void GetBfresToEncode()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("\nEncode which .bfres in 'Finished\'?");

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

                if (isRegexValid && File.Exists(@"Finished\" + input + ".bfres"))
                {
                    validInput = true;
                    bfresFile = input;
                    szsTarget = input;
                }
                else Console.WriteLine(@"Finished\" + input + ".bfres does not exist!");
            }
        }

        private void HandleConflict()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(
                "\n" + @"File 'Finished\szs\" + szsTarget + ".szs' already exists." +
                "\nWhat to do?" +
                "\no - overwrite existing file" +
                "\nr - rename new file" +
                "\nc - cancel"
                );

            string conflictInput = "";
            bool conflictValidInput = false;
            while (!conflictValidInput)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                conflictInput = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Red;

                switch (conflictInput)
                {
                    case "o":
                        conflictValidInput = true;
                        File.Delete(@"Finished\szs\" + szsTarget + ".szs");
                        write = true;
                        break;
                    case "r":
                        conflictValidInput = true;
                        write = true;
                        GetNewSzsFileName();
                        break;
                    case "e":
                        conflictValidInput = true;
                        write = false;
                        break;
                    case "c":
                        conflictValidInput = true;
                        write = false;
                        break;

                    default:
                        conflictValidInput = false;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("no valid input");
                        break;
                }
            }
        }

        private void GetNewSzsFileName()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("\nNew name?\nDo not add .szs");

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

                if (regexItem.IsMatch(input)) { validInput = true; isRegexValid = true; }
                else
                {
                    Console.WriteLine("Your input contains special characters.\nPlease remove any of them from the .bfres file, including spaces.");
                    continue;
                }

                if (isRegexValid)
                {
                    szsTarget = input;
                }
            }
        }

        private void AskForMode()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(
                "How do you want to compress?" +
                "\n1 - default" +
                "\n2 - fast (might increase file size)" +
                "\n3 - compress big file (WILL take a long time)" +
                "\n"
                );

            Regex fastRegex = new Regex("^[123]{1}$");
            string input = "";
            bool isRegexValid = false;
            while (!isRegexValid)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;

                input = Console.ReadLine().ToLower();

                if (!fastRegex.IsMatch(input))
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine("Invalid input");
                    continue;
                }

                isRegexValid = true;
            }

            switch (input)
            {
                case "1":
                    encoder = "default";
                    break;
                case "2":
                    encoder = "default";
                    useFast = "Fast";
                    break;
                case "3":
                    encoder = "inCode";
                    break;
            }
        }

        private void RunDefaultEncoder(string bfresFile)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Process proc = new Process();
            proc.StartInfo.FileName = @"res\yaz0enc" + useFast + ".exe";
            proc.StartInfo.Arguments = @"Finished\" + bfresFile + ".bfres";
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.Start();

            StreamWriter mySW = proc.StandardInput;

            proc.WaitForExit();
        }

        private void RunInCodeEncoder()
        {
            Compressor.YAZ0 encoder = new Compressor.YAZ0();

            File.WriteAllBytes(@"Finished\szs\" + szsTarget + ".szs", encoder.Compress(bfresFile));
        }
    }
}
