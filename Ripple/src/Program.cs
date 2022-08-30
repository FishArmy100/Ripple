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

            var result = Parser.Parse(toks);

            if(result is Utils.Result<List<Statement>, List<ParserError>>.Fail f)
            {
                Console.WriteLine("Parsing Errors:");
                foreach (var error in f.Error)
                    Console.WriteLine(error.Message + ": [" + error.Tok.Line + ", " + error.Tok.Column + "]");

                hasError = true;
            }

            if(!hasError && result is Utils.Result<List<Statement>, List<ParserError>>.Ok ok)
            {
                AstPrinter printer = new AstPrinter("  ");
                foreach(Statement statement in ok.Data)
                    printer.PrintAst(statement);
            }
        }
    }
}
