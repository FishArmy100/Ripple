using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Compiling
{
    public class CompilerSettings
    {
        public DebugStagesFlags StagesFlags = DebugStagesFlags.None;
        public string OutputRelativePath = "logs";
        public bool UseSameFile = true;
        public bool UseDebugging = false;

        public CompilerSettings()
        {
        }

        public CompilerSettings(DebugStagesFlags stagesFlags, string outputRelativePath, bool useSameFile, bool useDebugging)
        {
            StagesFlags = stagesFlags;
            OutputRelativePath = outputRelativePath;
            UseSameFile = useSameFile;
            UseDebugging = useDebugging;
        }
    }
}
