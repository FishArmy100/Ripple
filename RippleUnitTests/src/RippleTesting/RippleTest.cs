using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RippleUnitTests.Parsing;
using Raucse.Strings;

namespace RippleUnitTests.RippleTesting
{
    class RippleTest
    {
        public readonly string Name;
        public readonly string Code;
        public readonly bool ShouldCompile;

        public RippleTest(string name, string code, bool shouldCompile)
        {
            Name = name;
            Code = code;
            ShouldCompile = shouldCompile;
        }

        public static RippleTest FromNode(TestNode node, string tab = "\t")
        {
            string code = node.Code;
            if(node.InsertMain)
            {
                string[] lines = code.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                StringMaker maker = new StringMaker(tab);
                maker.AppendLine("func main() -> void\n{");
                maker.TabIn();
                maker.AppendLines(lines);
                maker.TabOut();
                maker.AppendLine("}");
                code = maker.ToString();
            }

            return new RippleTest(node.Name, AdditionalTestCode.CODE + code, node.ShouldCompile);
        }
    }
}
