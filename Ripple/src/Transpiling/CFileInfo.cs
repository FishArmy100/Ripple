using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Transpiling
{
    class CFileInfo
    {
        public readonly string RelativePath;
        public readonly string Source;

        public CFileInfo(string relativePath, string source)
        {
            RelativePath = relativePath;
            Source = source;
        }
    }
}
