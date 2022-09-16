using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;

namespace Ripple.Compiling
{
    struct CompilerError
    {
        public readonly string Message;
        public readonly Token Token;

        public CompilerError(string message, Token token)
        {
            Message = message;
            Token = token;
        }

        public CompilerError(LexerError error)
        {
            Message = error.Message;
            Token = new Token("Unknown", TokenType.Unknown, error.Line, error.Column);
        }

        public CompilerError(Parsing.ParserError error)
        {
            Message = error.Message;
            Token = error.Tok;
        }

        public CompilerError(Validation.ValidationError error)
        {
            Message = error.Message;
            Token = error.ErrorToken;
        }

        public override string ToString()
        {
            return Message + ": [" + Token.Line + ", " + Token.Column + "]";
        }
    }
}
