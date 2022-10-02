using System;
using System.Collections.Generic;
using System.Text;

namespace ASTGeneration.Utils
{
    static class Nameing
    {
        public static string GetVisitorName(NodeData nodeData) => "I" + nodeData.BaseName + "Visitor";
        public static string GetVisitorMethodName(NodeData nodeData) => "Visit" + nodeData.Name;
    }
}
