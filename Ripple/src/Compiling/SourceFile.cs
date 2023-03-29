using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Ripple.Compiling
{
    struct SourceFile
    {
        public readonly string StartPath;
        public readonly string RelativePath;
        public string FullPath => StartPath + "\\" + RelativePath;
        public readonly string Name;

        public SourceFile(string startPath, string relativePath)
        {
            RelativePath = relativePath;
            StartPath = startPath;
            Name = Path.GetFileName(StartPath + RelativePath);
        }

        public string Read() => File.ReadAllText(FullPath);
    }
}
