using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raucse.Extensions;
using Raucse;
using Ripple.Compiling;
using Ripple.Core;

namespace Ripple.Lexing
{
    public static class Lexer
    {
        public static Result<List<Token>, List<LexerError>> Scan(SourceData source)
        {
            List<Token> tokens = new List<Token>();
            List<LexerError> errors = new List<LexerError>();
            TokenBuilder builder = new TokenBuilder();

            foreach(SourceFile file in source.Files)
            {
                string text = file.Read();
                builder.SetSource(text, file.FullPath);

                while(true)
                {
                    var result = builder.Next();
                    if (!result.HasValue())
                        break;

                    result.Value.Match(ok => tokens.Add(ok), fail => errors.Add(fail));
                }

                tokens.Add(new Token(file.FullPath, new SourceLocation(text.Length - 1, text.Length - 1, file.FullPath), TokenType.EOF, false));
            }

            if (errors.Any())
                return errors;

            return tokens;
        }
    }
}
