using System;
using System.IO;

namespace PdfMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = "23820420453326.pkpass";
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
