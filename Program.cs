using System;

namespace Ripple
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            RunRippleCode();
        }

        private static void RunRippleCode()
        {
            Console.WriteLine("Run Ripple Code:");
            while (true)
            {
                Console.Write(">>>: ");
                string input = Console.ReadLine();

                if (input == "close")
                    break;

                ScanResult result = Scanner.GetTokens(input);

                string scannerTokens = "";

                foreach(Token token in result.Tokens)
                {
                    scannerTokens += token.ToString() + ", ";
                }

                Console.WriteLine(scannerTokens);

                string scannerErrors = "";
                foreach(ScannerError error in result.ScannerErrors)
                {
                    scannerErrors += error.ToString();
                }

                if (result.HasError)
                    Console.WriteLine("Scanner Errors: " + scannerErrors);

                ParserResult parserResult = Parser.Parse(result.Tokens);
                if (!parserResult.HasError)
                {
                    Console.WriteLine(ASTPrinter.PrintTree(parserResult.ParsedExpression));
                    Console.WriteLine(Transpiler.TranspileExpression(parserResult.ParsedExpression));
                }
                else
                {
                    Console.WriteLine(parserResult.Errors[0].Message);
                }
            }
        }
    }
}
