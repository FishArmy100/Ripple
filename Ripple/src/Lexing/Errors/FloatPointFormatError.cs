using Ripple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Lexing.Errors
{
    class FloatPointFormatError : LexerError
    {
        public FloatPointFormatError(SourceLocation location) : base(location)
        {
        }

        public override string GetMessage() => "Float literal must have a digit after the decimal point. you can use .0";
    }
}
