using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Transpiling
{
    struct TranspilerResult
    {
        public readonly string HeaderFileName;
        public readonly string HeaderFile;
        public readonly string SourceFileName;
        public readonly string SourceFile;

        public TranspilerResult(string headerFileName, string headerFile, string sourceFileName, string sourceFile)
        {
            HeaderFileName = headerFileName;
            HeaderFile = headerFile;
            SourceFileName = sourceFileName;
            SourceFile = sourceFile;
        }

        public string ToPrettyString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Header file: " + HeaderFileName + "\n");
            stringBuilder.AppendLine(HeaderFile);
            stringBuilder.AppendLine("Source file: " + SourceFileName + "\n");
            stringBuilder.AppendLine(SourceFile);

            return stringBuilder.ToString();
        }
    }
}
