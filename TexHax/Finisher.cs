using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TexHax
{
    class Finisher
    {
        string bfresFile = "";
        string newBfresFile = "";
        bool write = true;

        public void CopyBfresToFinishFolder()
        {
            GetBfresToCopy();

            newBfresFile = bfresFile;

            // Creating 'Finished\'
            if (!Directory.Exists(@"Finished\"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine("\nCreating 'Finished\\'");
                Directory.CreateDirectory(@"Finished\");
                Console.WriteLine(" done");
            }

            while (File.Exists(@"Finished\" + newBfresFile + ".bfres") && write == true) HandleConflict();

            if (write)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine("\nCopying 'bfres\\" + bfresFile + @".bfres' to 'Finished\" + newBfresFile + ".bfres'");
                File.Copy(@"bfres\" + bfresFile + ".bfres", @"Finished\" + newBfresFile + ".bfres");
                Console.WriteLine(" done\n");
            }
        }

        private void GetBfresToCopy()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("\nCopy which .bfres?");

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
                    newBfresFile = input;
                }
                else Console.WriteLine(@"bfres\" + input + ".bfres does not exist!");
            }
        }

        private void HandleConflict()
        {
            Console.WriteLine(
                "\n" + @"File 'Finished\" + newBfresFile + ".bfres' already exists." +
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
                        File.Delete(@"Finished\" + newBfresFile + ".bfres");
                        write = true;
                        break;
                    case "r":
                        conflictValidInput = true;
                        write = true;
                        GetNewBfresFileName();
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

        private void GetNewBfresFileName()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("\nNew name?\nDo not add .bfres");

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
                    newBfresFile = input;
                }
            }
        }
    }
}
