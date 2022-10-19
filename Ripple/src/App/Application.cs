using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Compiling;
using Ripple.Parsing;
using Ripple.AST;
using System.IO;
using Ripple.Utils;
using Ripple.Utils.Extensions;

namespace Ripple.App
{
    class Application
    {
        private bool m_IsRunning = false;
        private static string PathSavePath => Directory.GetCurrentDirectory() + "/CompilerData/CurrentPath.txt";
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
                }
            }
            else
            {
                ConsoleHelper.WriteLineError("Unknown command: " + input);
            }
        }

        private void SelectFile()
        {
            if (string.IsNullOrEmpty(CurrentPath))
                CurrentPath = FileBrowser.SelectFile(Directory.GetCurrentDirectory(), Core.FileExtensions.RippleFileExtension);
            else
                CurrentPath = FileBrowser.SelectFile(CurrentPath, Core.FileExtensions.RippleFileExtension);
        }

        private void SelectFolder()
        {
            if (string.IsNullOrEmpty(CurrentPath))
                CurrentPath = FileBrowser.SelectFolder(Directory.GetCurrentDirectory());
            else
                CurrentPath = FileBrowser.SelectFolder(CurrentPath);
        }

        private void CompileSource(List<SourceFile> sourceFiles)
        {
            if(string.IsNullOrEmpty(CurrentPath))
            {
                ConsoleHelper.WriteLineError("No file is selected.");
                return;
            }

            var parserResult = Compiler.RunParser(sourceFiles);
            parserResult.Match(
                ok =>
                {
                    AstPrinter printer = new AstPrinter("  ");
                    printer.PrintAst(ok);
                },
                fail =>
                {
                    foreach (CompilerError compilerError in fail)
                        ConsoleHelper.WriteLineError(compilerError.ToString());
                });
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
