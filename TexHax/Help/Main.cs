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

        public void Help()
        {
            while (true)
            {
                if (!RunMode(AskForMode())) break;
            }

            Console.WriteLine();
        }

        private string AskForMode()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;

            Console.WriteLine(
                "You need help for what?" +
                "\n1 - Where to start?" +
                "\n2 - Saving edited files" +
                "\n" +
                "\nq - quit" +
                "\n"
                );

            Console.SetWindowPosition(0, 0);

            string input = "";

            Regex regexItem = new Regex(@"^(([1-5]{1})|([q]{1}))$");
            bool validInput = false;
            while (!validInput)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                input = Console.ReadLine().ToLower();

                if (regexItem.IsMatch(input)) validInput = true;
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Not a valid input!");
                }
            }

            return input;
        }

        private bool RunMode(string mode)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Clear();

            switch (mode)
            {
                case "1":
                    GetBasics().Help();
                    return true;
                case "2":
                    //PaintNET pdn = new PaintNET();
                    (new PaintNET()).Help();
                    return true;

                case "q":
                    return false;
            }

            return true;
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
