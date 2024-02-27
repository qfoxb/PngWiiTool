using System.Diagnostics;
using System.Drawing;
using System.IO.Compression;
namespace PngWiiTool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("PngWiiTool v0.1 written by qfoxb\nUsage: PngWiiTool <input.png> <output.png_wii>");
                return;
            }
            if (args.Length == 2)
            {
                string inputPng = args[0];
                string outputPngWii = args[1];
                

                // Convert PNG to TPL
                if (File.Exists("wimgt.exe"))
                {
                    var tplfile = inputPng+ ".tpl";
                    var wimgtArgs = "-d \"" + tplfile + "\" ENC -x TPL.CMPR \"" + inputPng + "\""; // Source: https://github.com/trojannemo/Nautilus/blob/master/Nautilus/NemoTools.cs
                    var Headers = new ImageHeaders(); // Source: https://github.com/trojannemo/Nautilus/blob/master/Nautilus/NemoTools.cs
                    Process.Start(new ProcessStartInfo("wimgt.exe")
                    {
                        Arguments = wimgtArgs,
                       // UseShellExecute = false,
                        //CreateNoWindow = true,

                    });
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
