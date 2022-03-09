using System;
using System.Collections.Generic;
using System.Text;

namespace Ripple
{
    class SourceReader
    {
        public readonly string Source;
        public int Start { get; private set; }
        public int Current { get; private set; }
        public int Line { get; private set; }

        public SourceReader(string source)
        {
            Source = source;
            Start = 0;
            Current = 0;
            Line = 1;
        }

        public char AdvanceCurrent()
        {
            if (IsAtEnd())
                return '\0';

            if (PeekCurrent() == '\n')
                Line++;

            return Source[Current++];
        }

        public string AdvanceCurrent(int length)
        {
            string value = "";
            for (int i = 0; i < length; i++)
                value += AdvanceCurrent();
            return value;
        }

        public char PeekCurrent(int offset = 0)
        {
            if (Current + offset >= Source.Length)
                return '\0';

            return Source[Current + offset];
        }

        public bool IsAtEnd()
        {
            return Current >= Source.Length;
        }

        public void UpdateStart()
        {
            Start = Current;
        }

        public bool MatchChar(char c)
        {
            if(PeekCurrent() == c)
            {
                AdvanceCurrent();
                return true;
            }

            return false;
        }

        public string GetStartToCurrentString()
        {
            return Source[Start..Current];
        }
    }
}
