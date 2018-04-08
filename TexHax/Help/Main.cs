using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TexHax.Help
{
    class Main
    {
        Basics basics;

        public void help()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;

            RunMode(AskForMode());

            //paintNET();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
        }

        private void paintNET()
        {
            Console.WriteLine(
                @"Edit the .dds using Paint.NET
save with these parameters:
 - DTX5 (Interpolated Alpha)
 - Iterative Adjustmend (slow/HQ)
 - Perceptive
 - Generate MipMaps
"
                );
        }

        private string AskForMode()
        {
            Console.WriteLine(
                "You need help for what?" +
                "\n1 - Where to start?" +
                "\n" +
                "\nq - quit"
                );

            string input = "";

            Regex regexItem = new Regex(@"^(([1-5]{1})|([q]{1}))$");
            bool validInput = false;
            while (!validInput)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                input = Console.ReadLine();

                if (regexItem.IsMatch(input)) validInput = true;
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Not a valid input!");
                }
            }

            return input;
        }

        private void RunMode(string mode)
        {
            switch (mode)
            {
                case "1":
                    GetBasics().Test();
                    break;

                case "q":
                    return;
            }
        }

        private Basics GetBasics()
        {
            if (basics == null)
            {
                basics = new Basics();
            }

            return basics;
        }
    }
}
