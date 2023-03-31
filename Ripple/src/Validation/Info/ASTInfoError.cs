using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;

namespace Ripple.Validation.Info
{
    public struct ASTInfoError
    {
        public readonly string Message;
        public readonly Token Token;

        public ASTInfoError(string message, Token token)
        {
            Message = message;
            Token = token;
        }
    }
}
