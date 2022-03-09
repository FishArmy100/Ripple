using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Input;
using System.IO;

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
            while(true)
            {
                Console.Write(">>>: ");
                string input = Console.ReadLine();

                if (input == "close")
                    break;

                if(input == "runf")
                {
                    Console.WriteLine("-------------------------------");
                    string[] lines = File.ReadAllLines("C:\\dev\\Ripple\\Ripple\\Tests\\TestRippleScript.txt");
                    string src = string.Join("\n", lines);
                    DebugSourceCode(src);
                }
                else
                {
                    Console.WriteLine("-------------------------------");
                    DebugSourceCode(input);
                }
            }
        }

        private static void DebugSourceCode(string src)
        {
            Utils.OperationResult<CompilerResult, CompilerError> result = Compiler.CompileSource(src);

            string scannerTokens = "";

            foreach (Token token in result.Result.Tokens)
            {
                scannerTokens += token.ToString() + ", ";
            }

            Console.WriteLine(scannerTokens);

            string errorString = "Errors: \n";
            foreach (CompilerError error in result.Errors)
            {
                errorString += "\t" + error.ErrorMessage;
                errorString += "\n";
            }

            Console.WriteLine(errorString);

            Console.WriteLine(ASTPrinter.PrintTree(result.Result.AST, "     "));
        }
    }
}
