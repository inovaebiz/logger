using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoggerTeste
{
    class Program
    {
        static void Main(string[] args)
        {
            string inp = "go";

            while (!string.IsNullOrEmpty(inp))
            {
                inp = Console.ReadLine();
                new Logger.Logger().FazerLogAsync(new Exception("Exception de teste!"), "Isso é apenas um teste do Webservice!");
                Console.WriteLine("Log realizado\n");
            }
            
            
        }
    }
}
