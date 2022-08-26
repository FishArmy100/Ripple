using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ASTGeneration
{
    static class CodeGenerator
    {
        public static void GenerateCode(string dir, string namespaceName, string visitorName, string baseName, List<NodeData> nodes, List<string> additionalUsings)
        {
            foreach (NodeData node in nodes)
            {
                string src = GenerateCodeForNode(node, additionalUsings);
                WriteToFile(dir, node.Name, src);
            }

            string baseSrc = GenerateBaseClassForNodes(baseName, namespaceName, visitorName);
            WriteToFile(dir, baseName, baseSrc);

            string[] nodeNames = nodes.ConvertAll(x => x.Name).ToArray();
            string visitorSrc = GenerateVisitorForNodes(visitorName, namespaceName, nodeNames);
            WriteToFile(dir, visitorName, visitorSrc);
        }

        public static string GenerateCodeForNode(NodeData nodeData, List<string> additionalUsings)
        {
            StringBuilder builder = new StringBuilder();
            BeginCodeFile(nodeData.NamespaceName, builder, additionalUsings);

            builder.AppendLine("\tclass " + nodeData.Name + " : " + nodeData.BaseName);
            builder.AppendLine("\t{");

            builder.AppendLine();
            GenFeilds(nodeData, builder);

            builder.AppendLine();
            GenConstructor(nodeData, builder);

            builder.AppendLine();
            GenAcceptVisitorMethod(nodeData, builder);

            builder.AppendLine("\t}");
            EndCodeFile(builder);

            return builder.ToString();
        }

        public static string GenerateBaseClassForNodes(string name, string namespaceName, string visitorName)
        {
            StringBuilder builder = new StringBuilder();
            BeginCodeFile(namespaceName, builder, new List<string>());

            builder.AppendLine("\tabstract class " + name);
            builder.AppendLine("\t{");
            builder.AppendLine("\t\tpublic abstract void " + Keywords.AcceptVisitorName + "(" + visitorName + " " + visitorName.FirstCharToLowerCase() + ");");
            builder.AppendLine("\t}");

            EndCodeFile(builder);

            return builder.ToString();
        }

        public static string GenerateVisitorForNodes(string name, string namespaceName, string[] nodeNames)
        {
            StringBuilder builder = new StringBuilder();
            BeginCodeFile(namespaceName, builder, new List<string>());

            builder.AppendLine("\tinterface " + name);
            builder.AppendLine("\t{");

            foreach (string s in nodeNames)
                builder.AppendLine("\t\tpublic abstract void Visit" + s + "(" + s + " " + s.FirstCharToLowerCase() + ");");

            builder.AppendLine("\t}");

            EndCodeFile(builder);
            return builder.ToString();
        }

        private static void EndCodeFile(StringBuilder builder)
        {
            builder.AppendLine("}");
        }

        private static void BeginCodeFile(string namespaceName, StringBuilder builder, List<string> additionalUsings)
        {
            builder.AppendLine("using System;");

            foreach (string s in additionalUsings)
                builder.AppendLine("using " + s + ";");

            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine("namespace " + namespaceName);
            builder.AppendLine("{");
        }

        private static void GenAcceptVisitorMethod(NodeData nodeData, StringBuilder builder)
        {
            builder.AppendLine("\t\tpublic override void " + Keywords.AcceptVisitorName + "(" + nodeData.VisitorName + " visitor)");
            builder.AppendLine("\t\t{");
            builder.AppendLine("\t\t\tvisitor." + nodeData.VisitorMethod + "(this);");
            builder.AppendLine("\t\t}");
        }

        private static void GenFeilds(NodeData nodeData, StringBuilder builder)
        {
            foreach (KeyValuePair<string, string> feild in nodeData.FeildData)
                builder.AppendLine("\t\tpublic readonly " + feild.Key + " " + feild.Value + ";");
        }

        private static void GenConstructor(NodeData nodeData, StringBuilder builder)
        {
            builder.Append("\t\tpublic " + nodeData.Name + "(");
            for (int i = 0; i < nodeData.FeildData.Count; i++)
            {
                var feildData = nodeData.FeildData[i];

                if (i != 0)
                    builder.Append(", ");
                builder.Append(feildData.Key + " " + feildData.Value.FirstCharToLowerCase());

            }
            builder.AppendLine(")");
            builder.AppendLine("\t\t{");

            foreach (var feild in nodeData.FeildData)
            {
                builder.AppendLine("\t\t\tthis." + feild.Value + " = " + feild.Value.FirstCharToLowerCase() + ";");
            }

            builder.AppendLine("\t\t}");
        }

        private static void WriteToFile(string dir, string name, string data)
        {
            FileStream stream = GetOrCreateFile(dir, name);
            using StreamWriter writer = new StreamWriter(stream);
            writer.Write(data);
        }

        private static FileStream GetOrCreateFile(string dir, string name)
        {
            Directory.CreateDirectory(dir);
            return File.Create(GetPath(dir, name));
        }

        private static string GetPath(string dir, string name) => dir + "\\" + name + ".cs";

        private static string FirstCharToLowerCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && char.IsUpper(str[0]))
                return str.Length == 1 ? char.ToLower(str[0]).ToString() : char.ToLower(str[0]) + str[1..];

            return str;
        }
    }
}
