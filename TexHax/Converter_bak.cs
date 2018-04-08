using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexHax
{
    class Converter_bak
    {
        Process proc = new Process();
        string target;
        string[] files;
        string cParams = @"-printinfo";

        // for %%f in (Convert/*.gtx) do texconv2 -i Convert/%%f -o OutDDS/%%~nf.dds -printinfo
        public void Convert()
        {
            target = "DK";
            files = Directory.GetFiles(@"Extracted\" + target + @"\");
            RunProgram();
        }

        private void RunProgram()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            proc.StartInfo.FileName = @"res\texconv2.exe";
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.UseShellExecute = false;

            RunConvert("lossless");
            RunConvert("lossy");
            }

        private void RunConvert(string kind)
        {
            foreach (string file in files)
            {
                string input = file;
                string output = file.Replace("Extracted", "Converted/" + GenerateOutputKind(kind)).Replace("gtx", "dds");

                cParams = "-i " + input + GenerateFormat(kind) + " -o " + output + " -printinfo";

                proc.StartInfo.Arguments = cParams;
                proc.Start();
                StreamWriter mySW = proc.StandardInput;

                proc.WaitForExit();

                if (kind == "lossless")
                {
                    ConvertToLossless(output);
                }
            }
        }

        private string GenerateOutputKind(string kind)
        {
            if (kind == "lossless") return "dds";
            return "dds_lossy";
        }

        private string GenerateFormat(string kind)
        {
            if (kind == "lossless") return " -f GX2_SURFACE_FORMAT_TCS_R8_G8_B8_A8_UNORM";
            return "";
        }

        private void ConvertToLossless(string output)
        {
            // -i OutDDS_Lossless/%%f -o OutDDS_Lossless/%%~nf.dds

            cParams = "-i " + output + " -o " + output + " -printinfo";

            proc.StartInfo.Arguments = cParams;
            proc.Start();
            StreamWriter mySW = proc.StandardInput;

            proc.WaitForExit();
        }
    }
}
