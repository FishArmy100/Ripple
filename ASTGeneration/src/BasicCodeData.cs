using System;
using System.Collections.Generic;
using System.Text;
using ASTGeneration.Utils;

namespace ASTGeneration
{
    struct BasicCodeData
    {
        public readonly string Directory;
        public readonly string NamespaceName;
        public readonly string BaseName;
        public readonly List<Pair<string, string>> BaseProperties;

        public BasicCodeData(string directory, string namespaceName, string baseName, List<Pair<string, string>> baseProperties)
        {
            Directory = directory;
            NamespaceName = namespaceName;
            BaseName = baseName;
            BaseProperties = baseProperties;
        }
    }
}
