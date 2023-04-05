using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Validation;

namespace Ripple.Compiling
{
    public struct CompilerError
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
            Token = new Token();
        }

        public CompilerError(ParserError error)
        {
            Message = error.Message;
            Token = error.Tok;
        }

        public CompilerError(ValidationError error)
        {
            Message = error.Message;
            Token = error.ErrorToken;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
