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

namespace Ripple.Compiling
{
    public static class Compiler
    {
        public const string INTERMEDIATE_FOLDER_NAME = "intermediates";
        public const string BIN_FOLDER_NAME = "bin";
        public const string CLANG_LOG_FILE_NAME = "clang-log.txt";

        public static Result<List<Token>, List<CompilerError>> RunLexer(SourceData sourceFiles)
        {
            return Lexer.Scan(sourceFiles).ConvertToCompilerResult(e => e);
        }

        public static Result<ProgramStmt, List<CompilerError>> RunParser(SourceData sourceFiles)
        {
            return RunLexer(sourceFiles).Match(
                    ok => Parser.Parse(ok, sourceFiles.StartPath).ConvertToCompilerResult(e => e), 
                    fail => new Result<ProgramStmt, List<CompilerError>>(fail));
        }

        public static Result<TypedProgramStmt, List<CompilerError>> RunValidator(SourceData sourceFiles)
        {
            return RunParser(sourceFiles).Match(
                    ok => Validator.ValidateAst(ok).ConvertToCompilerResult(e => e),
                    fail => new Result<TypedProgramStmt, List<CompilerError>>(fail));
        }

        public static Result<List<CFileInfo>, List<CompilerError>> RunTranspiler(SourceData sourceFiles)
        {
            return RunValidator(sourceFiles).Match(
                    ok => Transpiler.Transpile(ok),
                    fail => new Result<List<CFileInfo>, List<CompilerError>>(fail));
        }

        public static Result<string, List<CompilerError>> RunClangCompiler(SourceData sourceFiles)
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
                    return outputPath;
                },
                fail => new Result<string, List<CompilerError>>(fail));
        }

        public static Result<Pair<string, int>, List<CompilerError>> CompileAndRun(SourceData source, params string[] args)
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

        private static Result<TSuccess, List<CompilerError>> ConvertToCompilerResult<TSuccess, TError>(this Result<TSuccess, List<TError>> self, Converter<TError, CompilerError> converter)
        {
            return self.Match(
                ok   => new Result<TSuccess, List<CompilerError>>(ok),
                fail => fail.ConvertAll(converter));
        }
    }
}
