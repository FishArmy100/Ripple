using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Input;
using System.IO;
using Ripple.Lexing;

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

            if(errors.Count > 0)
            {
                Console.WriteLine("Errors:");
                foreach (var error in errors)
                    Console.WriteLine(error.Message + ": [" + error.Line + ", " + error.Column + "]");
            }
            else
            {
                Console.WriteLine("Tokens:");
                foreach (var tok in toks)
                    Console.WriteLine(tok.ToPrettyString());
            }
        }
    }
}
