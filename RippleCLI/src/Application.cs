using System;
using System.Collections.Generic;
using Ripple.Compiling;
using System.IO;
using Ripple.Lexing;
using Sharprompt;
using Raucse.Extensions;
using Raucse.FileManagement;
using Ripple.AST.Utils;
using Raucse;
using Ripple.Core;
using System.Linq;

namespace RippleCLI
{
    class Application
    {
        private bool m_IsRunning = false;
        private static string PathSavePath => Directory.GetCurrentDirectory() + "/CompilerData/CurrentPath.txt";
        private static string ModeSavePath => Directory.GetCurrentDirectory() + "/CompilerData/CurrentMode.txt";

        private static CompilerMode? CurrentMode
        {
            get
            {
                var text = FileUtils.ReadFromFile(ModeSavePath);
                if (!text.HasValue())
                    return null;

                if(Enum.TryParse(typeof(CompilerMode), text.Value, out object mode))
                {
                    return (CompilerMode)mode;
                }

                return null;
            }
            set
            {
                FileUtils.WriteToFile(ModeSavePath, value.ToString());
            }
        }

        private static string CurrentPath
        {
            get
            {
                var text = FileUtils.ReadFromFile(PathSavePath);
                if (!text.HasValue())
                    return null;
                if (!text.Value.IsNullOrEmpty())
                    return text.Value.RemoveWhitespace();

                return null;
            }
            set
            {
                FileUtils.WriteToFile(PathSavePath, value);
            }
        }

        public void Run()
        {
            Console.WriteLine("Welcome to Ripple!:");
            m_IsRunning = true;
            while(m_IsRunning)
            {
                Console.Write(">>>: ");
                string input = Console.ReadLine();
                ProcessUserInput(input);
                Console.WriteLine("-------------------------------");
            }
        }

        private void ProcessUserInput(string input)
        {
            if(InputCommandHelper.TryGetCommand(input, out var command))
            {
                switch (command)
                {
                    case AppInputCommand.Run:
                        RunCompiler();
                        break;
                    case AppInputCommand.Close:
                        Close();
                        break;
                    case AppInputCommand.SelectFile:
                        SelectFile();
                        break;
                    case AppInputCommand.SelectFolder:
                        SelectFolder();
                        break;
                    case AppInputCommand.SelectMode:
                        SelectCompilerMode();
                        break;
                    case AppInputCommand.PrintInputPath:
                        PrintInputPath();
                        break;
                    case AppInputCommand.PrintCompilerMode:
                        PrintCompilerMode();
                        break;
                    case AppInputCommand.HelpCommand:
                        PrintCommands();
                        break;
                }
            }
            else
            {
                ConsoleHelper.WriteError("Unknown command: \'" + input + "\', try \'help\', to show a list of commands.");
            }
        }

        private static void PrintCommands()
        {
            Console.WriteLine("Commands:");
            foreach (string command in InputCommandHelper.GetCommands())
                Console.WriteLine(" - " + command);
        }

        private void PrintCompilerMode()
        {
            if (CurrentMode.HasValue)
                Console.WriteLine("Compiler Mode: " + CurrentMode.ToString());
            else
                ConsoleHelper.WriteError("No compiler mode selected.");
        }

        private static void PrintInputPath()
        {
            Console.WriteLine("Input path: " + CurrentPath);
        }

        private static void SelectCompilerMode()
        {
            CurrentMode = Prompt.Select<CompilerMode>("Select compiler mode");
        }

        private static void SelectFile()
        {
            if (string.IsNullOrEmpty(CurrentPath))
                CurrentPath = FileBrowser.SelectFile(Directory.GetCurrentDirectory(), FileExtensions.RippleFileExtension);
            else
                CurrentPath = FileBrowser.SelectFile(CurrentPath, FileExtensions.RippleFileExtension);
        }

        private static void SelectFolder()
        {
            if (string.IsNullOrEmpty(CurrentPath))
                CurrentPath = FileBrowser.SelectFolder(Directory.GetCurrentDirectory());
            else
                CurrentPath = FileBrowser.SelectFolder(CurrentPath);
        }

        private static void CompileSource(SourceData sourceFiles)
        {
            if(string.IsNullOrEmpty(CurrentPath))
            {
                ConsoleHelper.WriteError("No file is selected.");
                return;
            }

            Compiler compiler = GetCompiler();
            switch (CurrentMode)
            {
                case CompilerMode.Lexing:
                    var lexerResult = compiler.RunLexer(sourceFiles);
                    lexerResult.Match(
                        ok =>
                        {
                            if (compiler.Settings.UseDebugging)
                                return;

                            foreach (Token t in ok)
                                Console.WriteLine(t.ToString());
                        },
                        fail =>
                        {
                            PrintErrors(fail);
                        });
                    break;
                case CompilerMode.Parsing:
                    var parserResult = compiler.RunParser(sourceFiles);
                    parserResult.Match(
                        ok =>
                        {
                            if (compiler.Settings.UseDebugging)
                                return;

                            AstPrinter printer = new AstPrinter("   ");
                            printer.PrintAst(ok);
                        },
                        fail =>
                        {
                            PrintErrors(fail);
                        });
                    break;
                case CompilerMode.Validating:
                    var validationResult = compiler.RunValidator(sourceFiles);
                    validationResult.Match(
                        ok =>
                        {
                            if (compiler.Settings.UseDebugging)
                                return;

                            AstPrinter printer = new AstPrinter("   ");
                            ConsoleHelper.WriteLine("Validated successfully!");
                        },
                        fail =>
                        {
                            PrintErrors(fail);
                        });
                    break;
                case CompilerMode.Transpiling:
                    var transpilingResult = compiler.RunTranspiler(sourceFiles);
                    transpilingResult.Match(
                        ok =>
                        {
                            if (compiler.Settings.UseDebugging)
                                return;

                            StringMaker maker = new StringMaker();
                            maker.AppendLine("Files:");
                            maker.TabIn();
                            foreach(var file in ok)
                            {
                                maker.AppendLine(file.RelativePath);
                                maker.TabIn();
                                maker.AppendLines(file.Source.Split('\n'));
                                maker.TabOut();
                            }
                            maker.TabOut();
                            ConsoleHelper.WriteLine(maker.ToString());
                        },
                        fail => 
                        {
                            PrintErrors(fail);
                        });
                    break;
                case CompilerMode.Compiling:
                    var result = compiler.RunClangCompiler(sourceFiles);
                    result.Match(
                        ok => { },
                        fail =>
                        {
                            PrintErrors(fail);
                        });
                    break;
                case CompilerMode.Running:
                    var runningResult = compiler.CompileAndRun(sourceFiles);
                    runningResult.Match(
                        ok => 
                        {
                            Console.WriteLine();
                            Console.WriteLine($"Exited with code: {ok.Second}");
                        },
                        fail =>
                        {
                            PrintErrors(fail);
                        });
                    break;
            }
        }

        private static void PrintErrors(IEnumerable<CompilerError> errors)
        {
            ConsoleHelper.WriteError($"Compiled with {errors.Count()} errors:");
            foreach(string error in CompilerErrorFormatter.Format(errors))
            {
                ConsoleHelper.WriteError(error);
            }
        }

		private static void RunCompiler()
        {
            SourceData.FromPath(CurrentPath).Match(
                ok => CompileSource(ok),
                () => ConsoleHelper.WriteError("Invalid current path, please select a new path"));
        }

        private static Compiler GetCompiler()
        {
            CompilerSettings settings = new CompilerSettings 
            {
                StagesFlags = DebugStagesFlags.All,
                UseDebugging = false,
                UseSameFile = false
            };

            return new Compiler(settings);
        }

        private void Close()
        {
            m_IsRunning = false;
        }
    }
}
