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

        public TokenReader(List<Token> tokens, int startIndex) : this(tokens)
        {
            Index = startIndex;
        }

        public List<Token> GetTokens() => m_Tokens;

        public Token Current() => m_Tokens[Index];
        public Token Previous() => Peek(-1).Value;
        public Token Advance()
        {
            if (IsAtEnd())
                throw new ReaderAtEndExeption();
            return m_Tokens[Index++];
        }

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

        public bool PeekMatch(int offset, params TokenType[] types)
        {
            return Peek(offset) is Token t && t.IsType(types);
        }

        public Token? Peek(int offset = 1) => Index + offset < m_Tokens.Count ? m_Tokens[Index + offset] : null;

        public bool IsAtEnd() => Index >= m_Tokens.Count;

        public Token Consume(TokenType tokenType, string errorMessage)
        {
            if (IsAtEnd())
                throw new ParserExeption(Last(), errorMessage);

            if (Match(tokenType))
                return Previous();

            throw new ParserExeption(Current(), errorMessage);
        }

        public void SyncronizeTo(Func<TokenReader, bool> predicate)
        {
            while (true)
            {
                if (IsAtEnd() || predicate(this))
                    break;

                Advance();
            }
        }

        public void SyncronizeTo(params TokenType[] types)
        {
            while (true)
            {
                if (IsAtEnd() || types.Contains(Current().Type))
                    break;

                Advance();
            }
        }
    }
}
