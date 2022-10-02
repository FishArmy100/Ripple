using System;
using System.Collections.Generic;
using System.Text;

namespace ASTGeneration
{
    struct BasicCodeData
    {
        public readonly string Directory;
        public readonly string NamespaceName;
        public readonly string BaseName;

        public BasicCodeData(string directory, string namespaceName, string baseName)
        {
            Directory = directory;
            NamespaceName = namespaceName;
            BaseName = baseName;
        }
    }
}
