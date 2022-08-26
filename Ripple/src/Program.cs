using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Input;
using System.IO;
using Ripple.Lexing;
using Ripple.AST;
using Ripple.Parsing;

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

                if (input == "runf")
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
            (var toks, var errors) = Lexer.Scan(src);
            bool hasError = false;

            if(errors.Count > 0)
            {
                Console.WriteLine("Lexing Errors:");
                foreach (var error in errors)
                    Console.WriteLine(error.Message + ": [" + error.Line + ", " + error.Column + "]");

                hasError = true;
            }

            (var expr, var parsingErrors) = Parser.Parse(toks);

            if(parsingErrors.Count > 0)
            {
                Console.WriteLine("Parsing Errors:");
                foreach (var error in parsingErrors)
                    Console.WriteLine(error.Message + ": [" + error.Tok.Line + ", " + error.Tok.Column + "]");

                hasError = true;
            }

            if(!hasError)
            {
                AstPrinter printer = new AstPrinter("  ");
                printer.PrintAst(expr);
            }
        }
    }
}
