using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;

namespace Ripple.Parsing
{
    class TokenReader
    {
        private readonly List<Token> m_Tokens;
        public int Index { get; private set; }

        public TokenReader(List<Token> tokens)
        {
            m_Tokens = tokens;
            Index = 0;
        }

        public Token Current() => m_Tokens[Index];
        public Token Previous() => Peek(-1).Value;
        public Token Advance() => m_Tokens[Index++];

        public Token Last() => m_Tokens[m_Tokens.Count - 1];

        public bool Match(params TokenType[] types)
        {
            if(types.Contains(Current().Type))
            {
                Advance();
                return true;
            }

            return false;
        }

        public Token? Peek(int offset = 1) => Index + offset < m_Tokens.Count ? m_Tokens[Index + offset] : null;

        public bool IsAtEnd() => Index >= m_Tokens.Count;
    }
}
