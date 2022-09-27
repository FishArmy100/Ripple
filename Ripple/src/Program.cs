using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Input;
using System.IO;
using Ripple.Compiling;

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
            var lexingResult = Compiler.RunLexer(src);

            lexingResult.Match(
                ok => {
                    foreach (Lexing.Token token in ok)
                        Console.WriteLine(token.ToPrettyString());
                },
                fail => {
                    foreach (CompilerError error in fail)
                        Console.WriteLine(error);
                });
        }
    }
}
