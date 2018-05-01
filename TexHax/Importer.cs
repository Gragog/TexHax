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

            Console.ForegroundColor = ConsoleColor.Green;
        }

        void ImportAll()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Getting texture positions...");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            string procParams = @"bfres\" + target + ".bfres";

            Process proc = new Process();
            proc.StartInfo.FileName = @"res\hax\hax.exe";
            proc.StartInfo.Arguments = procParams;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true; // exe output to my program todo
            proc.Start();

            string[] output = proc.StandardOutput.ReadToEnd().Split('\n');

            #region alt input
            /*
            * while (!proc.StandardOutput.EndOfStream)
            * {
            *     Console.WriteLine(proc.StandardOutput.ReadLine());
            *     list.Add(proc.StandardOutput.ReadLine());
            * }
            * // https://stackoverflow.com/questions/4291912/process-start-how-to-get-the-output
            */
            #endregion

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
                    textureNames.Add(line.Substring(whitespaceIndex + 1).TrimEnd('\r').ToLower());
                    texturePositions.Add(line.Substring(0, whitespaceIndex));
                }
            }

            Console.Write("\rFound " + i + " texture positions      \nGetting .dds files...");

            string[] ddsFilesWithPath = Directory.GetFiles(@"Converted\dds\" + target + @"\", "*.dds");
            string[] ddsFiles = new string[ddsFilesWithPath.Length];
            Console.Write("\rFound " + ddsFilesWithPath.Length + " .dds file" + (ddsFilesWithPath.Length == 1 ? "" : "s") + "            \nFinding matches...");

            // save file names without path to array
            for (int j = 0; j < ddsFilesWithPath.Length; j++)
            {
                string newFileName = ddsFilesWithPath[j].Replace(".dds", "");

                if (newFileName.Contains(@"\"))
                {
                    newFileName = newFileName.Substring(newFileName.LastIndexOf(@"\") + 1);
                }
                // Console.WriteLine(newFileName);

                ddsFiles[j] = newFileName;
            }

            List<string> allParams = new List<string>();
            for (i = 0; i < texturePositions.Count; i++)
            {
                int pos = Array.IndexOf(ddsFiles, textureNames[i]);

                if (pos != -1)
                {
                    string fileName = ddsFilesWithPath[pos];

                    // Console.WriteLine(fileName);

                    allParams.Add(fileName + @" bfres\" + target + ".bfres 0 " +
                            texturePositions[i] + " " +
                            wait);
                }
            }

            Console.Write("\rFound " + allParams.Count + " match" + (allParams.Count == 1 ? "" : "es") + "         \nStart importing "
                + (allParams.Count == 1 ? "it" : "them") + "...\n\n");

            int k = 0;
            foreach (string currentParam in allParams)
            {
                DrawProgress(++k, allParams.Count, currentParam);

                proc.StartInfo.Arguments = currentParam;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true; // exe output to my program todo
                proc.Start();
                proc.WaitForExit();
            }

            sw.Stop();
            Console.WriteLine("\n\nImported in " + sw.Elapsed.Minutes + ":" + sw.Elapsed.Seconds + "." + sw.Elapsed.Milliseconds + " minutes\n");
        }

        void DrawProgress(int current, int total, string param)
        {
            int percentage = (100 * current / total);

            string toWrite = percentage < 10 ? "0" + percentage.ToString() : percentage.ToString();
            string anotherToWrite = current < 10 ? "0" + current.ToString() : current.ToString();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\r" + anotherToWrite + "/" + total + " " + toWrite + "% {");
            Console.ForegroundColor = ConsoleColor.Green;

            int progressBarWidth = Console.WindowWidth - 20;
            for (int i = 0; i < progressBarWidth; i++)
            {
                int myPercentage = (percentage * progressBarWidth) / 100;

                Console.Write(i <= myPercentage ? (i == myPercentage ? "=>" : "=") : " ");
            }

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.Write("}");
        }

        string GetPath()
        {
            string path = System.Reflection.Assembly.GetEntryAssembly().Location;
            string fileName = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
            return path.Replace(@"\" + fileName, "");
        }

        void SetTarget()
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
                input = Console.ReadLine();
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

        void AskForDDS()
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
                input = Console.ReadLine();
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

        void AskForMinMip()
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
                input = Console.ReadLine();
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

        void AskForTexPos()
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
                input = Console.ReadLine();
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

        void CleanUp()
        {
            string[] gtxFiles = Directory.GetFiles(@"Converted\dds\" + target + "\\", "*.gtx");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Cleaninig up:\n");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  deleting " + gtxFiles.Length + " file" + (gtxFiles.Length == 1 ? "" : "s") + "...");

            foreach (string gtxFile in gtxFiles)
            {
                File.Delete(gtxFile);
            }

            Console.WriteLine("   done\n");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Cleaned up\n");
        }
    }
}
