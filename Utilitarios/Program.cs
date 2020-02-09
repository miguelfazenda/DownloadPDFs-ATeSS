using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Utilitarios
{
    static class Program
    {
        [Verb("teste", HelpText = "")]
        public class TestOptions
        {
        }

        static int Main(string[] args)
        {
            //ExcelToWintouch.Run(@"c:\Users\Miguel\Desktop\Faturas.12T.xlsx", "Folha2", @"c:\Users\Miguel\Desktop\teste.txt");
            //Console.WriteLine("Não implementado ainda :(");

            int ret = CommandLine.Parser.Default.ParseArguments<ExcelToWintouch.ExcelToWintouchOptions, TestOptions>(args)
                .MapResult(
                  (ExcelToWintouch.ExcelToWintouchOptions opts) => ExcelToWintouch.Run(opts),
                  errs => 1);

            return ret;
        }
    }
}
