using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TexHax
{
    class Settings
    {
        public static int import_waitForSec = 1;

        public static int yaz0_maxBack = 0x1000; //was 0x600

        public void Navigate()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;

            Console.WriteLine(
                "Change what?" +
                "\n1 - Waiting time for importing" +
                "\n2 - maxBack value for compressing huge files"
                );

            string input = "";

            Regex regexItem = new Regex(@"^(([1-2]{1})|([q]{1}))$");
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

            Console.Clear();
            switch (input)
            {
                case "1":
                    SetImport_waitForSec();
                    break;
                case "2":
                    SetYaz0_maxBack();
                    break;

                case "q":
                    return;
            }
        }

        private void SetImport_waitForSec()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;

            Console.WriteLine("\nNew value (default: 1):");

            string input = "";
            Regex regexItem = new Regex(@"^\d{1,3}$");

            bool isRegexValid = false;
            while (!isRegexValid)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                input = Console.ReadLine();

                if (input == "q")

                Console.ForegroundColor = ConsoleColor.Red;
                if (regexItem.IsMatch(input)) isRegexValid = true;
                else Console.WriteLine("Input contains characters or more than 2 digits!");
            }

            import_waitForSec = Convert.ToInt32(input);

            Console.WriteLine("");
        }

        private void SetYaz0_maxBack()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;

            Console.WriteLine("\nNew value (default: 1):");

            string input = "";
            Regex regexItem = new Regex(@"^\d{1,4}$");

            bool isRegexValid = false;
            while (!isRegexValid)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                input = Console.ReadLine();

                Console.ForegroundColor = ConsoleColor.Red;
                if (regexItem.IsMatch(input)) isRegexValid = true;
                else Console.WriteLine("Input contains characters or more than 4 digits!");
            }

            yaz0_maxBack = Convert.ToInt32(input);

            Console.WriteLine("");
        }

    }
}
