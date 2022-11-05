﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Validation;
using Ripple.Transpiling;
using Ripple.Utils;
using Ripple.AST;

namespace Ripple.Compiling
{
    static class Compiler
    {
        public static CompilerResult<List<Token>> RunLexer(List<SourceFile> sourceFiles)
        {
            var results = sourceFiles
                .ConvertAll(f => Lexer.Scan(f.Source))
                .ConvertAll(lr => lr.ConvertToCompilerResult(e => new CompilerError(e)));

            List<Token> tokens = new List<Token>();
            List<CompilerError> compilerErrors = new List<CompilerError>();

            foreach(var result in results)
            {
                result.Match(
                    ok =>
                    {
                        tokens.AddRange(ok);
                    },
                    fail =>
                    {
                        compilerErrors.AddRange(fail);
                    });
            }

            if (compilerErrors.Count > 0)
                return new CompilerResult<List<Token>>.Fail(compilerErrors);
            else
                return new CompilerResult<List<Token>>.Ok(tokens);
        }

        public static CompilerResult<ProgramStmt> RunParser(List<SourceFile> sourceFiles)
        {
            return RunLexer(sourceFiles)
                .Match(ok => Parser.Parse(ok)
                .ConvertToCompilerResult(e => new CompilerError(e)));
        }

        private static CompilerResult<TSuccess> ConvertToCompilerResult<TSuccess, TError>(this Result<TSuccess, List<TError>> self, Converter<TError, CompilerError> converter)
        {
            return self switch
            {
                Result<TSuccess, List<TError>>.Ok ok => new CompilerResult<TSuccess>.Ok(ok.Data),
                Result<TSuccess, List<TError>>.Fail fail => new CompilerResult<TSuccess>.Fail(fail.Error.ConvertAll(converter)),
                _ => throw new ArgumentException("Result has a different result than ok, or fail")
            };
        }
    }
}