using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Transpiling.C_AST;

namespace Ripple.Transpiling
{
    public class CFileInfo
    {
        public readonly string RelativePath;
        public readonly string Source;
        public readonly CFileType FileType;

        public CFileInfo(string relativePath, string source, CFileType fileType)
        {
            RelativePath = relativePath;
            Source = source;
            FileType = fileType;
        }
    }
}
