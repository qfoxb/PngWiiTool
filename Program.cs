using System.Diagnostics;
using System.Runtime.InteropServices;
namespace PngWiiTool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("PngWiiTool v0.2 written by qfoxb\nUsage: PngWiiTool <input.png> <output.png_wii>");
                return;
            }
            if (args.Length == 2)
            {
                string inputPng = args[0];
                string outputPngWii = args[1];


                // detect if wimgt is present
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
                {
                    if (!File.Exists("wimgt.exe"))
                    {
                        Console.WriteLine("wimgt.exe not found. Please place wimgt.exe in the same directory as PngWiiTool.");
                        return;
                    }
                } else
                {
                    if (!File.Exists("wimgt"))
                    {
                        Console.WriteLine("wimgt not found. Please place wimgt in the same directory as PngWiiTool.");
                        return;
                    }
                }
                // convert file to tpl using wimgt
                {
                    var tplfile = inputPng+ ".tpl";
                    var wimgtArgs = "-d \"" + tplfile + "\" ENC -x TPL.CMPR \"" + inputPng + "\""; // Source: https://github.com/trojannemo/Nautilus/blob/master/Nautilus/NemoTools.cs
                    var Headers = new ImageHeaders(); // Source: https://github.com/trojannemo/Nautilus/blob/master/Nautilus/NemoTools.cs
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        Process.Start("wimgt.exe", wimgtArgs).WaitForExit();
                    } else
                    {
                        Process.Start("wimgt", wimgtArgs).WaitForExit();
                    }   
                    // convert tpl to png_wii by cramming headers into the tpl file
                   // Code between these comments is from Nautilus. Source: https://github.com/trojannemo/Nautilus/blob/master/Nautilus/NemoTools.cs
                   var binaryReader = new BinaryReader(File.OpenRead(tplfile));
                    var binaryWriter = new BinaryWriter(new FileStream(outputPngWii, FileMode.Create));
                    binaryReader.BaseStream.Position = 64L;
                    binaryWriter.Write(Headers.wii_256x256);
                    var buffer = new byte[64];
                    int num;
                    do
                    {
                        num = binaryReader.Read(buffer, 0, 64);
                        if (num > 0)
                            binaryWriter.Write(buffer);
                    } while (num > 0);
                    binaryWriter.Dispose();
                    binaryReader.Dispose();
                    //End Nautilus code
                    File.Delete(tplfile);

                }
            }

        }
    }
}
