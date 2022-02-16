using System;

namespace Ripple
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Run Ripple Code:");
            while(true)
            {
                Console.Write(">>>: ");
                string input = Console.ReadLine();

                ScanResult result = Scanner.GetTokens(input);

                string t = "Tokens: ";
                foreach (Token token in result.Tokens)
                {
                    t +=  "(" + token.ToString() + "), ";
                }
                Console.WriteLine(t);

                string e = "Errors: ";
                foreach (ScannerError error in result.ScannerErrors)
                {
                    e += "(" + error.ToString() + "), ";
                }
                Console.WriteLine(e);

                if (input == "close")
                    break;
            }
        }
    }
}
