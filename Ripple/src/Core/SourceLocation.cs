using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;

namespace Ripple.Core
{
    public struct SourceLocation
    {
        public readonly int Start;
        public readonly int End;
        public readonly string File;

        public SourceLocation(int start, int end, string file)
        {
            Start = start;
            End = end;
            File = file;
        }

        public override bool Equals(object obj)
        {
            return obj is SourceLocation location &&
                   Start == location.Start &&
                   End == location.End &&
                   File == location.File;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Start, End, File);
        }

        public static bool operator ==(SourceLocation left, SourceLocation right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SourceLocation left, SourceLocation right)
        {
            return !(left == right);
        }

        public static SourceLocation operator+(SourceLocation left, SourceLocation right)
        {
            if (left.File != right.File)
                throw new ArgumentException("Source locations must be of the same file.");

            int start = Math.Min(left.Start, right.Start);
            int end = Math.Max(left.End, right.End);
            return new SourceLocation(start, end, left.File);
        }
    }
}
