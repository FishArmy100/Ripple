using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using ASTGeneration.Utils.Extensions;
using ASTGeneration.Utils;

namespace ASTGeneration
{
    static class AstGenerator
    {
        public static void Generate(string directoryName, string namespaceName, string baseName, string baseProperties, List<string> nodeNames, List<string> additionalUsings)
        {
            var nodes = GenerateNodeData(namespaceName, baseName, nodeNames);
            var props = ParseBaseProperties(baseProperties);
            BasicCodeData codeData = new BasicCodeData(directoryName, namespaceName, baseName, props);
            CodeGenerator.GenerateCode(codeData, nodes, additionalUsings);
        }

        private static List<Pair<string, string>> ParseBaseProperties(string baseProperties)
        {
            return baseProperties.Split(';').Where(s => !string.IsNullOrEmpty(s)).Select(p =>
            {
                string[] param = p.Trim().Split(' ');
                return new Pair<string, string>(param[0], param[1]);
            }).ToList();
        }

        private static List<NodeData> GenerateNodeData(string namespaceName, string baseName, List<string> nodes)
        {
            List<NodeData> nodeDatas = new List<NodeData>();

            foreach (string nodeSrc in nodes)
            {
                string[] nameFeildsPair = nodeSrc.Split(':');
                string name = nameFeildsPair[0].RemoveWhitespace();
                List<KeyValuePair<string, string>> feilds = new List<KeyValuePair<string, string>>();

                if (nameFeildsPair.Length > 1)
                {
                    foreach (string feild in nameFeildsPair[1].Split(';'))
                    {
                        string[] typeNamePair = feild.Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToArray();
                        feilds.Add(new KeyValuePair<string, string>(typeNamePair[0].RemoveWhitespace(), typeNamePair[1].RemoveWhitespace()));
                    }
                }

                nodeDatas.Add(new NodeData(namespaceName, baseName, name, feilds));
            }

            return nodeDatas;
        }
    }
}
