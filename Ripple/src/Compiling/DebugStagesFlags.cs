using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Compiling
{
    [Flags]
    public enum DebugStagesFlags
    {
        None = 0,
        Lexing = 1 << 0,
        Parsing = 1 << 1,
        Validation = 1 << 2,
        Transpiling = 1 << 3,
        CCompilation = 1 << 4,
        All = Lexing | Parsing | Validation | Transpiling | CCompilation,
    }

    public static class DebugStagesFlagsExtensions
    {
        public static bool Is(this DebugStagesFlags self, params DebugStagesFlags[] flags)
        {
            if (flags.Length == 0)
                return false;

            return flags.All(f => self.HasFlag(f));
        }
    }
}
