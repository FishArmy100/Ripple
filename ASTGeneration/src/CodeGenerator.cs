using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ASTGeneration.Utils.Extensions;
using ASTGeneration.Utils;

namespace ASTGeneration
{
    static class CodeGenerator
    {
        public static void GenerateCode(BasicCodeData data, List<NodeData> nodes, List<string> additionalUsings)
        {
            string visitorName = "I" + data.BaseName + "Visitor";

            foreach (NodeData node in nodes)
            {
                string src = GenerateCodeForNode(node, additionalUsings);
                WriteToFile(data.Directory, node.Name, src);
            }

            string baseSrc = GenerateBaseClassForNodes(data.BaseName, data.NamespaceName, visitorName);
            WriteToFile(data.Directory, data.BaseName, baseSrc);

            string[] nodeNames = nodes.ConvertAll(x => x.Name).ToArray();
            string visitorSrc = GenerateVisitorForNodes(visitorName, data.NamespaceName, nodeNames);
            WriteToFile(data.Directory, visitorName, visitorSrc);
        }

        public static string GenerateCodeForNode(NodeData nodeData, List<string> additionalUsings)
        {
            StringBuilder builder = new StringBuilder();
            BeginCodeFile(nodeData.NamespaceName, builder, additionalUsings);

            builder.AppendLine("\tclass " + nodeData.Name + " : " + nodeData.BaseName);
            builder.AppendLine("\t{");

            GenFeilds(nodeData, builder);

            builder.AppendLine();
            GenConstructor(nodeData, builder);

            builder.AppendLine();
            GenAcceptVisitorMethod(nodeData, builder);

            builder.AppendLine();
            GenGenericAcceptVisitorMethod(nodeData, builder);

            builder.AppendLine();
            GenGenericArgAcceptVisitorMethod(nodeData, builder);

            builder.AppendLine();
            GenEqualsOverride(nodeData, builder);

            builder.AppendLine();
            GenGetHashCodeOverride(nodeData, builder);

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
            builder.AppendLine("\t\tpublic abstract T " + Keywords.AcceptVisitorName + "<T>(" + visitorName + "<T> " + visitorName.FirstCharToLowerCase() + ");");
            builder.AppendLine("\t\tpublic abstract TReturn " + Keywords.AcceptVisitorName + "<TReturn, TArg>(" + visitorName + "<TReturn, TArg> " + visitorName.FirstCharToLowerCase() + ", TArg arg);");
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

            builder.AppendLine();

            builder.AppendLine("\tinterface " + name + "<T>");
            builder.AppendLine("\t{");

            foreach (string s in nodeNames)
                builder.AppendLine("\t\tpublic abstract T Visit" + s + "(" + s + " " + s.FirstCharToLowerCase() + ");");

            builder.AppendLine("\t}");

            builder.AppendLine();

            builder.AppendLine("\tinterface " + name + "<TReturn, TArg>");
            builder.AppendLine("\t{");

            foreach (string s in nodeNames)
                builder.AppendLine("\t\tpublic abstract TReturn Visit" + s +
                    "(" + s + " " + s.FirstCharToLowerCase() + ", TArg arg);");

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

        private static void GenEqualsOverride(NodeData nodeData, StringBuilder builder)
        {
            builder.AppendLine("\t\tpublic override bool Equals(object other)");
            builder.AppendLine("\t\t{");

            string valName = nodeData.Name.FirstCharToLowerCase();
            builder.AppendLine("\t\t\tif(other is " + nodeData.Name + " " + valName + ")");
            builder.AppendLine("\t\t\t{");

            if(nodeData.FeildData.Count > 0)
			{
                builder.Append("\t\t\t\treturn ");

                for (int i = 0; i < nodeData.FeildData.Count; i++)
                {
                    if (i != 0)
                        builder.Append(" && ");

                    string feildName = nodeData.FeildData[i].Value;

                    builder.Append(feildName + ".Equals(" + valName + "." + feildName + ")");
                }

                builder.AppendLine(";");
            }
			else
			{
                builder.AppendLine("\t\t\t\treturn true;");
			}

            
            builder.AppendLine("\t\t\t}");

            builder.AppendLine("\t\t\treturn false;");

            builder.AppendLine("\t\t}");
        }

        private static void GenGetHashCodeOverride(NodeData nodeData, StringBuilder builder)
        {
            builder.AppendLine("\t\tpublic override int GetHashCode()");
            builder.AppendLine("\t\t{");

            if(nodeData.FeildData.Count > 0)
			{
                builder.AppendLine("\t\t\tHashCode code = new HashCode();");
                foreach (var feild in nodeData.FeildData)
                    builder.AppendLine("\t\t\tcode.Add(" + feild.Value + ");");

                builder.AppendLine("\t\t\treturn code.ToHashCode();");
            }
			else
			{
                builder.AppendLine($"\t\t\treturn typeof({nodeData.Name}).Name.GetHashCode();");
			}
            

            builder.AppendLine("\t\t}");
        }

        private static void GenAcceptVisitorMethod(NodeData nodeData, StringBuilder builder)
        {
            builder.AppendLine("\t\tpublic override void " + Keywords.AcceptVisitorName + "(" + Nameing.GetVisitorName(nodeData) + " visitor)");
            builder.AppendLine("\t\t{");
            builder.AppendLine("\t\t\tvisitor." + Nameing.GetVisitorMethodName(nodeData) + "(this);");
            builder.AppendLine("\t\t}");
        }

        private static void GenGenericAcceptVisitorMethod(NodeData nodeData, StringBuilder builder)
        {
            builder.AppendLine("\t\tpublic override T " + Keywords.AcceptVisitorName + "<T>(" + Nameing.GetVisitorName(nodeData) + "<T> visitor)");
            builder.AppendLine("\t\t{");
            builder.AppendLine("\t\t\treturn visitor." + Nameing.GetVisitorMethodName(nodeData) + "(this);");
            builder.AppendLine("\t\t}");
        }

        private static void GenGenericArgAcceptVisitorMethod(NodeData nodeData, StringBuilder builder)
        {
            builder.AppendLine("\t\tpublic override TReturn " + Keywords.AcceptVisitorName + "<TReturn, TArg>(" + Nameing.GetVisitorName(nodeData) + "<TReturn, TArg> visitor, TArg arg)");
            builder.AppendLine("\t\t{");
            builder.AppendLine("\t\t\treturn visitor." + Nameing.GetVisitorMethodName(nodeData) + "(this, arg);");
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
    }
}
