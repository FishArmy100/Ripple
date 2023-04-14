using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raucse.Extensions;

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

        public char Current()
        {
            if (IsAtEnd())
                throw new ReaderAtEndExeption();

            return Src[Index];
        }

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

        public char Previous() => Src[Index - 1];
        public char? Peek(int offset = 1) => Index + offset < Src.Length ? Src[Index + offset] : null;

        public bool Match(params char[] cs)
        {
            if (IsAtEnd())
                return false;

            if(cs.Contains(Current()))
            {
                Advance();
                return true;
            }

            return false;
        }

        public bool CheckSequence(params char[] cs)
        {
            for(int i = 0; i < cs.Length; i++)
            {
                if(Peek(i) is char c)
                {
                    if (c != cs[i])
                        return false;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsAtEnd() { return Index >= Src.Length; }

        public char Consume(char c, string message)
        {
            if (!Match(c))
                throw new LexingExeption(message, Line, Column);

            return Previous();
        }
    }
}
