using System;
using RippleUnitTests.Parsing;
using Raucse.FileManagement;
using System.Collections.Generic;
using Raucse;
using System.Linq;
using Raucse.Extensions;
using RippleUnitTests.RippleTesting;

namespace RippleUnitTests
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = "C:\\dev\\Ripple\\RippleUnitTests\\Tests\\ripple_unit_tests.ripl";
            string source = FileUtils.ReadFromFile(filePath).Value;
            var result = TestNodeParser.Lex(source);

            result.Match(
                ok =>
                {
                    List<RippleTest> tests = ok.Select(t => RippleTest.FromNode(t)).ToList();
                    TestRunner.RunTests(tests);
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
