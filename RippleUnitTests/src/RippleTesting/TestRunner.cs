using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Compiling;
using Raucse.FileManagement;
using Raucse;
using Raucse.Strings;

namespace RippleUnitTests.RippleTesting
{
    static class TestRunner
    {

        public static void RunTests(List<RippleTest> tests)
        {
            CompilerSettings settings = new CompilerSettings
            {
                StagesFlags = DebugStagesFlags.None,
                UseSameFile = false,
                UseDebugging = false
            };

            Compiler compiler = new Compiler(settings);

            foreach(RippleTest test in tests)
            {
                SourceData data = PrepareTest(test.Code);
                RunTest(test, compiler, data, TestMode.Validation);
            }
        }

        private static SourceData PrepareTest(string code)
        {
            string path = FileUtils.ApplicationDirectoryPath + "\\test.ripl";
            FileUtils.WriteToFile(path, code);
            return SourceData.FromPath(path).Value;
        }

        private static void RunTest(RippleTest test, Compiler compiler, SourceData source, TestMode mode)
        {
            var result = mode switch
            {
                TestMode.Lexing => compiler.RunLexer(source).Match(
                    ok => new Option<List<CompilerError>>(), 
                    fail => new Option<List<CompilerError>>(fail)),
                TestMode.Parsing => compiler.RunParser(source).Match(
                    ok => new Option<List<CompilerError>>(),
                    fail => new Option<List<CompilerError>>(fail)),
                TestMode.Validation => compiler.RunValidator(source).Match(
                    ok => new Option<List<CompilerError>>(),
                    fail => new Option<List<CompilerError>>(fail)),
                _ => throw new ArgumentException("This should not be called"),
            };

            if(test.ShouldCompile)
            {
                if(result.HasValue())
                {
                    ConsoleHelper.WriteError($"Test '{test.Name}' did not compile when it should have");
                    PrintErrors(result.Value);
                }
                else
                {
                    ConsoleHelper.WriteMessage($"Test '{test.Name}' has successfully compiled!");
                }
            }
            else
            {
                if(result.HasValue())
                {
                    ConsoleHelper.WriteMessage($"Test '{test.Name}' has successfully not compiled!");
                }
                else
                {
                    ConsoleHelper.WriteError($"Test '{test.Name}' has compiled when it should not have");
                }
            }
        }

        private static void PrintErrors(IEnumerable<CompilerError> errors)
        {
            StringMaker maker = new StringMaker();
            int errorNumber = 0;
            foreach(CompilerError error in errors)
            {
                errorNumber++;
                maker.AppendLine("Errors:");
                maker.TabIn(TabModes.Number);
                maker.AppendLine(error.GetMessage());
                maker.TabOut();
            }

            ConsoleHelper.WriteError(maker.ToString());
        }
    }
}
