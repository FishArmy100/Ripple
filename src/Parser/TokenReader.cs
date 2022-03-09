using System;
using System.Collections.Generic;
using System.Text;

namespace Ripple
{
    class TokenReader
    {
        public readonly List<Token> Tokens;
        public int Current { get; private set; }

        public TokenReader(List<Token> tokens)
        {
            Tokens = tokens;
            Current = 0;
        }

        /// <summary>
        /// If the current token is any of the types shown, advances forward
        /// </summary>
        /// <param name="types"></param>
        /// <returns>returns true if advanced successfuly</returns>
        public bool MatchCurrent(params TokenType[] types)
        {
            if(CheckCurrent(types))
            {
                AdvanceCurrent();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the current token is of any of the given types
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public bool CheckCurrent(params TokenType[] types)
        {
            if (IsAtEnd())
                return false;

            TokenType type = PeekCurrent().Type;
            foreach(TokenType t in types)
            {
                if (t == type)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Advances the current token
        /// </summary>
        /// <returns>returns the previous token after advancing</returns>
        public Token AdvanceCurrent()
        {
            if (IsAtEnd())
                return Token.Invalid;

            Current++;
            return Previous();
        }

        public bool IsAtEnd()
        {
            return Current >= (Tokens.Count - 1);
        }

        /// <summary>
        /// Looks at the current token based on the offset
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Token PeekCurrent(int offset = 0)
        {
            if (Current + offset >= Tokens.Count - 1)
                return Token.Invalid;

            return Tokens[Current + offset];
        }

        /// <summary>
        /// Looks at the previous token
        /// </summary>
        /// <returns></returns>
        public Token Previous() => PeekCurrent(-1);
    }
}
