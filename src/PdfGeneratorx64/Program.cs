using PdfiumViewer;
using System;

namespace PdfGeneratorx64
{
    // Projeto: PdfGeneratorx64 (Compilado como x64)
    class Program
    {
        static int Main(string[] args )
        {
            if (Environment.Is64BitProcess)
            {
                throw new InvalidOperationException("Esta Aplicação deve rodar em x64!");
            }

            string args2 = @"C:\Temp\Boleto_0000124786.pdf";

            //if (args.Length != 1)
            //{
                //Console.WriteLine("Uso: PdfGeneratorx64.exe <input.pdf>");
                //return 1;
            //} 

            try
            {
                //string inputPath = args[0];

                using (var document = PdfDocument.Load(args2).CreatePrintDocument())
                {
                    document.Print();
                }

                Console.WriteLine("OK"); // Indica sucesso
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
                return 2;
            }
        }
    }
}
