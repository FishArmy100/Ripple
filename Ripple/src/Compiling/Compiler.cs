 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Validation;
using Ripple.Utils;
using Ripple.AST;
using Ripple.Validation.Info.Statements;
using Ripple.Transpiling;
using Ripple.Compiling.CCompilation;
using Ripple.Transpiling.C_AST;
using System.Diagnostics;
using Raucse.Extensions;
using Raucse;
using Raucse.FileManagement;
using Ripple.Parsing.Errors;
using Ripple.AST.Utils;
using Ripple.Validation.Errors;

namespace Ripple.Compiling
{
    public class Compiler
    {
        public const string INTERMEDIATE_FOLDER_NAME = "intermediates";
        public const string BIN_FOLDER_NAME = "bin";
        public const string CLANG_LOG_FILE_NAME = "clang-log.txt";

        public const string LEXER_LOG_FILE = "lexer-debug.txt";
        public const string PARSER_DEBUG_FILE = "parser-debug.txt";
        public const string VALIDATOR_DEBUG_FILE = "validator-debug.txt";
        public const string TRANSPILER_DEBUG_FILE = "transpiler-debug.txt";
        public const string CCOMPILATION_DEBUG_FILE = "c-compilation-debug.txt";

        public const string COMPILER_ALL_DEBUG_FILE = "compiler-all-debug.txt";

        public readonly CompilerSettings Settings;

        public Compiler(CompilerSettings settings)
        {
            Settings = settings;
        }

        public Compiler()
        {
            Settings = new CompilerSettings();
        }

        public Result<List<Token>, List<CompilerError>> RunLexer(SourceData sourceFiles)
        {
            ClearLogInfo(sourceFiles.StartPath);
            return Lexer.Scan(sourceFiles).Match(
                ok => 
                { 
                    if(Settings.UseDebugging && Settings.StagesFlags.Is(DebugStagesFlags.Lexing))
                    {
                        string data = ok.Select(t => t.ToString()).Concat("\n");
                        LogInfo(data, sourceFiles.StartPath, DebugPhase.Lexing);
                    }

                    return ok;
                },
                fail => GetErrorResult<List<Token>, LexerError>(fail));
        }

        public Result<ProgramStmt, List<CompilerError>> RunParser(SourceData sourceFiles)
        {
            return RunLexer(sourceFiles).Match(
                    ok => Parser.Parse(ok, sourceFiles.StartPath).Match(
                        ok => 
                        {
                            if (Settings.UseDebugging && Settings.StagesFlags.Is(DebugStagesFlags.Parsing))
                            {
                                StringBuilder builder = new StringBuilder();
                                AstPrinter printer = new AstPrinter("\t", text => builder.AppendLine(text));
                                 printer.PrintAst(ok);
                                LogInfo(builder.ToString(), sourceFiles.StartPath, DebugPhase.Parsing);
                            }

                            return ok;
                        },
                        fail => GetErrorResult<ProgramStmt, ParserError>(fail)), 
                    fail => new Result<ProgramStmt, List<CompilerError>>(fail));
        }

        public Result<TypedProgramStmt, List<CompilerError>> RunValidator(SourceData sourceFiles)
        {
            return RunParser(sourceFiles).Match(
                    ok => Validator.ValidateAst(ok).Match(
                        ok =>
                        {
                            if (Settings.UseDebugging && Settings.StagesFlags.Is(DebugStagesFlags.Validation))
                                LogInfo("Not implemented yet.", sourceFiles.StartPath, DebugPhase.Validating);

                            return ok;
                        },
                        fail => GetErrorResult<TypedProgramStmt, ValidationError>(fail)),
                    fail => new Result<TypedProgramStmt, List<CompilerError>>(fail));
        }

        public Result<List<CFileInfo>, List<CompilerError>> RunTranspiler(SourceData sourceFiles)
        {
            return RunValidator(sourceFiles).Match(
                    ok =>
                    {
                        List<CFileInfo> cfiles = Transpiler.Transpile(ok);
                        if (Settings.UseDebugging && Settings.StagesFlags.Is(DebugStagesFlags.Transpiling))
                        {
                            StringBuilder builder = new StringBuilder();
                            foreach (CFileInfo info in cfiles)
                            {
                                builder.AppendLine($"### {info.RelativePath}:");
                                builder.AppendLine(info.Source);
                                builder.AppendLine();
                            }

                            LogInfo(builder.ToString(), sourceFiles.StartPath, DebugPhase.Transpiling);
                        }

                        return cfiles;
                    },
                    fail => new Result<List<CFileInfo>, List<CompilerError>>(fail));
        }

        public Result<string, List<CompilerError>> RunClangCompiler(SourceData sourceFiles)
        {
            return RunTranspiler(sourceFiles)
                .Match(ok =>
                {
                    string intermediatesDirectory = $"{sourceFiles.StartPath}\\{INTERMEDIATE_FOLDER_NAME}";
                    List<string> files = new List<string>();
                    foreach (CFileInfo info in ok)
                    {
                        FileUtils.WriteToFile($"{intermediatesDirectory}\\{info.RelativePath}", info.Source);
                        
                        if(info.FileType == CFileType.Source)
                            files.Add(info.RelativePath);
                    }

                    string outputPath = $"{sourceFiles.StartPath}\\{BIN_FOLDER_NAME}\\{sourceFiles.SourceName}.exe";
                    FileUtils.WriteToFile(outputPath, string.Empty); // makes sure the file exists
                    string logPath = $"{intermediatesDirectory}\\{CLANG_LOG_FILE_NAME}";

                    ClangCompilerInterface.CompileFiles(intermediatesDirectory, files, outputPath, logPath);


                    if (Settings.UseDebugging && Settings.StagesFlags.Is(DebugStagesFlags.CCompilation))
                        LogInfo(FileUtils.ReadFromFile(logPath).Value, sourceFiles.StartPath, DebugPhase.CCompilation);

                    return outputPath;
                },
                fail => new Result<string, List<CompilerError>>(fail));
        }

        public Result<Pair<string, int>, List<CompilerError>> CompileAndRun(SourceData source, params string[] args)
        {
            return RunClangCompiler(source).Match(
                ok =>
                {
                    Process proc = Process.Start("cmd.exe", $"/C {ok} {args.Concat()}");
                    proc.WaitForExit();
                    return new Pair<string, int>(ok, proc.ExitCode);
                },
                fail => new Result<Pair<string, int>, List<CompilerError>>(fail));
        }

        private void LogInfo(string logData, string startPath, DebugPhase phase)
        {
            if(Settings.UseSameFile)
            {
                string previous = FileUtils.ReadFromFile(GetSinglePath(startPath)).MatchOrEmpty();
                previous += $"{phase}:\n";
                previous += logData;
                previous += "\n\n";
                FileUtils.WriteToFile(GetSinglePath(startPath), previous);
            }
            else
            {
                FileUtils.WriteToFile(GetDebugFilePath(phase, startPath), logData);
            }
        }

        private void ClearLogInfo(string startPath)
        {
            if(Settings.UseSameFile)
            {
                FileUtils.WriteToFile(GetSinglePath(startPath), string.Empty);
            }
            else
            {
                FileUtils.WriteToFile(GetDebugFilePath(DebugPhase.Lexing, startPath), string.Empty);
                FileUtils.WriteToFile(GetDebugFilePath(DebugPhase.Parsing, startPath), string.Empty);
                FileUtils.WriteToFile(GetDebugFilePath(DebugPhase.Validating, startPath), string.Empty);
                FileUtils.WriteToFile(GetDebugFilePath(DebugPhase.Transpiling, startPath), string.Empty);
                FileUtils.WriteToFile(GetDebugFilePath(DebugPhase.CCompilation, startPath), string.Empty);
            }
        }

        private string GetSinglePath(string startPath)
        {
            return $"{startPath}\\{Settings.OutputRelativePath}\\{COMPILER_ALL_DEBUG_FILE}";
        }

        private string GetDebugFilePath(DebugPhase phase, string startPath)
        {
            return phase switch
            {
                DebugPhase.Lexing =>        $"{startPath}\\{Settings.OutputRelativePath}\\{LEXER_LOG_FILE}",
                DebugPhase.Parsing =>       $"{startPath}\\{Settings.OutputRelativePath}\\{PARSER_DEBUG_FILE}",
                DebugPhase.Validating =>    $"{startPath}\\{Settings.OutputRelativePath}\\{VALIDATOR_DEBUG_FILE}",
                DebugPhase.Transpiling =>   $"{startPath}\\{Settings.OutputRelativePath}\\{TRANSPILER_DEBUG_FILE}",
                DebugPhase.CCompilation =>  $"{startPath}\\{Settings.OutputRelativePath}\\{CCOMPILATION_DEBUG_FILE}",
                _ => throw new NotImplementedException(),
            };
        }

        private static Result<T, List<CompilerError>> GetErrorResult<T, E>(List<E> errors) where E : CompilerError
        {
            return new Result<T, List<CompilerError>>(errors.Cast<CompilerError>().ToList());
        }

        private enum DebugPhase
        {
            Lexing,
            Parsing,
            Validating,
            Transpiling,
            CCompilation,
        }
    }
}
