using System;
using System.IO;

namespace PdfMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please pass filename as argument.");
                return;
            }
            string filename = args[0];
            Console.WriteLine($"Converting: {filename}");
            TicketPdfMaker t = new TicketPdfMaker(filename);

            string outputFilename = Path.ChangeExtension(filename, "pdf");
            Console.WriteLine($"Output file: {outputFilename}");
            // Do conversion
            t.Test(false, outputFilename);
            return;
        }
    }
}
