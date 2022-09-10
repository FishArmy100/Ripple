using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;

namespace Ripple.Validation
{
    class TypeCheckExeption : Exception
    {
        public readonly Token ErrorToken;

        public TypeCheckExeption(string message, Token errorToken) : base(message)
        {
            ErrorToken = errorToken;
        }
    }
}
