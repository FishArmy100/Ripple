using System;
using System.Collections.Generic;
using Ripple.Compiling;
using System.IO;
using Ripple.Utils;
using Ripple.Lexing;
using Sharprompt;
using Ripple.AST.Utils;
using Ripple.Utils.Extensions;
using Ripple.Transpiling.C_AST;
using Ripple.Transpiling.SourceGeneration;
using Ripple.Transpiling.ASTConversion;

namespace Ripple.App
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
                if (FileUtils.ReadFile(ModeSavePath, out string text))
                    return (CompilerMode)Enum.Parse(typeof(CompilerMode), text);

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
                if (FileUtils.ReadFile(PathSavePath, out string text))
                    return text.RemoveWhitespace();

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
                ConsoleHelper.WriteLineError("Unknown command: \'" + input + "\', try \'help\', to show a list of commands.");
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
                ConsoleHelper.WriteLineError("No compiler mode selected.");
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
                CurrentPath = FileBrowser.SelectFile(Directory.GetCurrentDirectory(), Core.FileExtensions.RippleFileExtension);
            else
                CurrentPath = FileBrowser.SelectFile(CurrentPath, Core.FileExtensions.RippleFileExtension);
        }

        private static void SelectFolder()
        {
            if (string.IsNullOrEmpty(CurrentPath))
                CurrentPath = FileBrowser.SelectFolder(Directory.GetCurrentDirectory());
            else
                CurrentPath = FileBrowser.SelectFolder(CurrentPath);
        }

        private static void CompileSource(List<SourceFile> sourceFiles)
        {
            if(string.IsNullOrEmpty(CurrentPath))
            {
                ConsoleHelper.WriteLineError("No file is selected.");
                return;
            }

            switch (CurrentMode)
            {
                case CompilerMode.Lexing:
                    var lexerResult = Compiler.RunLexer(sourceFiles);
                    lexerResult.Match(
                        ok =>
                        {
                            foreach (Token token in ok)
                                Console.WriteLine(token.ToPrettyString());
                        },
                        fail =>
                        {
                            foreach (CompilerError compilerError in fail)
                                ConsoleHelper.WriteLineError(compilerError.ToString());
                        });
                    break;
                case CompilerMode.Parsing:
                    var parserResult = Compiler.RunParser(sourceFiles);
                    parserResult.Match(
                        ok =>
                        {
                            AstPrinter printer = new AstPrinter("   ");
                            printer.PrintAst(ok);
                        },
                        fail =>
                        {
                            foreach (CompilerError compilerError in fail)
                                ConsoleHelper.WriteLineError(compilerError.ToString());
                        });
                    break;
                case CompilerMode.Validating:
                    var validationResult = Compiler.RunValidator(sourceFiles);
                    validationResult.Match(
                        ok =>
                        {
                            AstPrinter printer = new AstPrinter("   ");
                            ConsoleHelper.WriteLine("Validated successfully!");
                        },
                        fail =>
                        {
                            foreach (CompilerError compilerError in fail)
                                ConsoleHelper.WriteLineError(compilerError.ToString());
                        });
                    break;
                case CompilerMode.Transpiling:
                    var transpilingResult = Compiler.RunTranspiler(sourceFiles);
                    transpilingResult.Match(
                        ok =>
                        {
                            ConsoleHelper.WriteLine("C Source:");
                            ConsoleHelper.WriteLine(ok);
                        },
                        fail => 
                        {
                            foreach (CompilerError compilerError in fail)
                                ConsoleHelper.WriteLineError(compilerError.ToString());
                        });
                    break;
            }
        }

		private void RunCompiler()
        {
            if(FileUtils.ReadFolder(CurrentPath, out FolderData data))
            {
                CompileSource(GetSourceFiles(data));
            }
            else if(FileUtils.ReadFile(CurrentPath, out string src))
            {
                CompileSource(new List<SourceFile> { new SourceFile(CurrentPath, src) });
            }
            else
            {
                ConsoleHelper.WriteLineError("Invalid current path, please select a new path");
            }
        }

        private List<SourceFile> GetSourceFiles(FolderData folderData)
        {
            List<SourceFile> sourceFiles = new List<SourceFile>();
            foreach((string path, string src) in folderData.Files)
            {
                sourceFiles.Add(new SourceFile(path, src));
            }

            foreach(FolderData subFolder in folderData.Folders)
            {
                sourceFiles.AddRange(GetSourceFiles(subFolder));
            }

            return sourceFiles;
        }

        private void Close()
        {
            m_IsRunning = false;
        }
    }
}
