using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;

namespace Ripple
{
    class CompilerResult
    {
        public readonly List<Token> Tokens;
        public readonly AbstractSyntaxTree AST;

        public CompilerResult(List<Token> tokens, AbstractSyntaxTree aST)
        {
            Tokens = tokens;
            AST = aST;
        }
    }
}
