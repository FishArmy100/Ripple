using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace ASTGeneration
{
    static class AstGenerator
    {
        public static void Generate(string directoryName, string namespaceName, string baseName, List<string> nodes, List<string> additionalUsings)
        {
            var data = GenerateNodeData(namespaceName, baseName, nodes);
            string visitorName = GetVisitorName(baseName);
            CodeGenerator.GenerateCode(directoryName, namespaceName, visitorName, baseName, data, additionalUsings);
        }

        private static List<NodeData> GenerateNodeData(string namespaceName, string baseName, List<string> nodes)
        {
            List<NodeData> nodeDatas = new List<NodeData>();

            foreach (string nodeSrc in nodes)
            {
                string[] nameFeildsPair = nodeSrc.Split(':');
                string name = nameFeildsPair[0].RemoveWhitespace();
                List<KeyValuePair<string, string>> feilds = new List<KeyValuePair<string, string>>();

                foreach (string feild in nameFeildsPair[1].Split(';'))
                {
                    string[] typeNamePair = feild.Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    feilds.Add(new KeyValuePair<string, string>(typeNamePair[0].RemoveWhitespace(), typeNamePair[1].RemoveWhitespace()));
                }

                nodeDatas.Add(new NodeData(
                                 namespaceName, baseName, name,
                                 feilds, GetVisitorName(baseName),
                                 GetVisitorMethodName(name)));
            }

            return nodeDatas;
        }

        private static string GetVisitorName(string baseName) => "I" + baseName + "Visitor";
        private static string GetVisitorMethodName(string name) => "Visit" + name;


        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }
    }
}
