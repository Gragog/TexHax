using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TexHax
{
    class Decoder
    {
        string szsFileWithPath = "";
        string szsFileName = "";
        string newBfresFile = "";
        bool write = true;

        public void Decode()
        {
            GetSzsFile();

            newBfresFile = szsFileName;

            while (File.Exists(@"bfres\" + newBfresFile + ".bfres") && write == true) HandleConflict();

            if (!write) return;

            if (File.Exists(szsFileWithPath))
            {
                byte[] Data = File.ReadAllBytes(szsFileWithPath);
                if (Encoding.ASCII.GetString(Data.Take(4).ToArray()) == "Yaz0")
                {
                    // Successfully deterimed it's a Yaz0 file.
                    int Decompressed_Size = BitConverter.ToInt32(Data.Skip(4).Take(4).Reverse().ToArray(), 0);
                    Data = Data.Skip(16).ToArray();
                    byte[] Decompressed_Data = new byte[Decompressed_Size];

                    int Read_Position = 0;
                    int Write_Position = 0;
                    uint ValidBitCount = 0;
                    byte CurrentCodeByte = 0;

                    while (Write_Position < Decompressed_Size)
                    {
                        if (ValidBitCount == 0)
                        {
                            CurrentCodeByte = Data[Read_Position];
                            ++Read_Position;
                            ValidBitCount = 8;
                        }

                        if ((CurrentCodeByte & 0x80) != 0)
                        {
                            Decompressed_Data[Write_Position] = Data[Read_Position];
                            Write_Position++;
                            Read_Position++;
                        }
                        else
                        {
                            byte Byte1 = Data[Read_Position];
                            byte Byte2 = Data[Read_Position + 1];
                            Read_Position += 2;

                            uint Dist = (uint)(((Byte1 & 0xF) << 8) | Byte2);
                            uint CopySource = (uint)(Write_Position - (Dist + 1));

                            uint Byte_Count = (uint)(Byte1 >> 4);
                            if (Byte_Count == 0)
                            {
                                Byte_Count = (uint)(Data[Read_Position] + 0x12);
                                Read_Position++;
                            }
                            else
                            {
                                Byte_Count += 2;
                            }

                            for (int i = 0; i < Byte_Count; ++i)
                            {
                                Decompressed_Data[Write_Position] = Decompressed_Data[CopySource];
                                CopySource++;
                                Write_Position++;
                            }
                        }

                        CurrentCodeByte <<= 1;
                        ValidBitCount -= 1;
                    }

                    string File_Type = "bin";
                    // Check to see if our decompressed file has an extension
                    if (Decompressed_Data[0] != 0)
                    {
                        File_Type = Encoding.ASCII.GetString(Decompressed_Data.Take(4).ToArray()).ToLower();
                    }
                    string File_Path = Path.GetDirectoryName(szsFileWithPath) + @"\" + Path.GetFileNameWithoutExtension(szsFileWithPath);
                    FileStream Decompressed_File;
                    try { Decompressed_File = File.Create(File_Path + @"." + File_Type, Decompressed_Size); }
                    catch { Decompressed_File = File.Create(File_Path + @".bin", Decompressed_Size); }
                    Decompressed_File.Write(Decompressed_Data, 0, Decompressed_Size);
                    Decompressed_File.Flush();
                    Decompressed_File.Close();

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\nSuccessfully decompressed Yaz0 compressed file!\n");

                    if (!Directory.Exists("bfres\\"))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;

                        Console.WriteLine(@"Creating 'bfres\'");
                        Directory.CreateDirectory("bfres\\");
                        Console.WriteLine(" done\n");
                    }

                    File.Move(@"szs\" + szsFileName + ".fres", @"bfres\" + newBfresFile + ".bfres");
                }
                else
                {
                    Console.WriteLine("The file does not appear to be a valid Yaz0 compressed file!\n");
                }
            }
            else
            {
                Console.WriteLine("The file specified does not exist.\n");
            }
        }

        private void GetSzsFile()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("Decode which .szs?");

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
                    Console.WriteLine("Your input contains special characters.\nPlease remove any of them from the .bfres file, including spaces.");
                    continue;
                }

                if (isRegexValid && File.Exists(@"szs\" + input + ".szs"))
                {
                    validInput = true;
                    szsFileWithPath = @"szs\" + input + ".szs";
                    szsFileName = input;
                }
                else Console.WriteLine(@"szs\" + input + ".szs does not exist!");
            }
        }

        private void HandleConflict()
        {
            Console.WriteLine(
                "\n" + @"File 'bfres\" + szsFileName + ".bfres' already exists." +
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
                conflictInput = Console.ReadLine().ToLower();
                Console.ForegroundColor = ConsoleColor.Red;

                switch (conflictInput)
                {
                    case "o":
                        conflictValidInput = true;
                        File.Delete(@"bfres\" + szsFileName + ".bfres");
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
                input = Console.ReadLine().ToLower();
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