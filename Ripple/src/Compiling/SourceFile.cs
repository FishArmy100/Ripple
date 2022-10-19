using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Compiling
{
    struct SourceFile
    {
        public readonly string Path;
        public readonly string Source;

        public SourceFile(string path, string source)
        {
            Path = path;
            Source = source;
        }
    }
}
