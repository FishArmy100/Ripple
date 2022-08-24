using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Lexing
{
    class StringReader
    {
        public readonly string Src;
        public int Index { get; private set; }
        public int Line { get; private set; }
        public int Column { get; private set; }

        public StringReader(string src)
        {
            Src = src;
            Index = 0;
        }

        public char Current() => Src[Index];

        public char Advance()
        {
            char c = Current();
            Column++;
            if(c == '\n')
            {
                Line++;
                Column = 0;
            }
            Index++;
            return c;

        }
        

        public char? Peek(int offset = 1) => Index + offset < Src.Length ? Src[Index + offset] : null;

        public bool Match(params char[] cs)
        {
            if(cs.Contains(Current()))
            {
                Advance();
                return true;
            }

            return false;
        }

        public bool IsAtEnd() { return Index >= Src.Length; }
    }
}
