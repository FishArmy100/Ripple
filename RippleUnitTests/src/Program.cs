using System;
using RippleUnitTests.Lexing;
using Raucse.FileManagement;
using System.Collections.Generic;
using Raucse;
using System.Linq;
using Raucse.Extensions;

namespace RippleUnitTests
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = "C:\\dev\\Ripple\\RippleUnitTests\\Tests\\ripple_unit_tests.ripl";
            string source = FileUtils.ReadFromFile(filePath).Value;
            var result = TestNodeLexer.Lex(source);

            result.Match(
                ok =>
                {
                    foreach (var val in ok)
                    {
                        ConsoleHelper.WriteMessage($"Settings: {val.Arguments.Select(a => $"{a.Type}{a.Value.Match(ok => $":({ok})", () => "")}").Concat(", ")}");
                        ConsoleHelper.WriteMessage($"Code:\n{val.Code}\n");
                    }
                },
                fail => 
                {
                    foreach (var error in fail)
                    {
                        ConsoleHelper.WriteError(error.ToString());
                    }
                });

        }
    }
}
