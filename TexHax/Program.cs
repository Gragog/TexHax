﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TexHax.Help;

namespace TexHax
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            FirstRun();

            while (true)
            {
                Start();
            }

            Console.ReadLine();
        }

        public static void Start()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.Title = "TexHax - Texture extractor, converter and importer for .szs and .bfres files";

            Console.WriteLine(
                "Main Menu - Select Mode:" +
                "\n1 - Decode .szs" +
                "\n2 - Extract from .bfres" +
                "\n3 - Convert extracted .gtx to .dds" +
                "\n4 - Import .dds into a .bfres or do stuff related to this" +
                "\n5 - Pack a .bfres into .szs" +
                "\n" +
                "\ns - Settings" +
                "\nf - Copy a .bfres to 'Finished\', in case a .bfres is what you need" +
                "\nh - Help" +
                "\nc - Clear stuff belonging to a .bfres" +
                "\nw - Wipe everything except 'szs\'" +
                        // ",'bfres_backup\'" +
                        " and 'Finished\'" +
                "\n" +
                "\ne - exit"
            );

            string input = "";

            Regex regexItem = new Regex(@"^(([1-6]{1})|([ehcfsw]{1}))$");
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

            Console.ForegroundColor = ConsoleColor.Green;

            switch (input)
            {
                case "1":
                    Decoder decoder = new Decoder();
                    Console.Clear();
                    decoder.Decode();
                    break;
                case "2":
                    Extractor extractor = new Extractor();
                    Console.Clear();
                    extractor.Extract();
                    break;
                case "3":
                    Converter converter = new Converter();
                    Console.Clear();
                    converter.Convert();
                    break;
                case "4":
                    Importer importer = new Importer();
                    Console.Clear();
                    importer.Import();
                    break;
                case "5":
                    Encoder encoder = new Encoder();
                    Console.Clear();
                    encoder.Encode();
                    break;

                case "s":
                    Settings settings = new Settings();
                    Console.Clear();
                    settings.Navigate();
                    break;
                case "f":
                    Finisher finisher = new Finisher();
                    Console.Clear();
                    finisher.CopyBfresToFinishFolder();
                    break;
                case "c":
                    Clearer clearer = new Clearer();
                    Console.Clear();
                    clearer.Clear(false);
                    break;
                case "h":
                    Help.Main help = new Help.Main();
                    Console.Clear();
                    help.Help();
                    break;
                case "w":
                    Clearer wiper = new Clearer();
                    Console.Clear();
                    wiper.Clear(true);
                    break;

                case "e":
                    System.Environment.Exit(0);
                    break;
            }
        }

        private static void FirstRun()
        {
            if (!Directory.Exists("szs\\")) Directory.CreateDirectory("szs\\");
        }
    }
}
