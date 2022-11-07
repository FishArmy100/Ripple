using System;
using System.Collections.Generic;
using System.Text;

namespace ASTGeneration
{
    struct NodeData
    {
        public readonly string NamespaceName;
        public readonly string BaseName;
        public readonly string Name;
        public readonly List<KeyValuePair<string, string>> FeildData;

        public NodeData(string namespaceName, string baseName, string name, List<KeyValuePair<string, string>> feildData)
        {
            NamespaceName = namespaceName;
            BaseName = baseName;
            Name = name;
            FeildData = feildData;
        }
    }
}
