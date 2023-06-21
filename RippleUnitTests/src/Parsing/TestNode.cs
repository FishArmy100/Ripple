using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RippleUnitTests.Parsing
{
    class TestNode
    {
        public readonly string Code;
        public readonly string Name;
        public readonly bool ShouldCompile;
        public readonly bool InsertMain;

        public TestNode(string code, string name, bool shouldCompile, bool insertMain)
        {
            Code = code;
            Name = name;
            ShouldCompile = shouldCompile;
            InsertMain = insertMain;
        }

        public override string ToString()
        {
            return $"Name: {Name}, Insert Main: {InsertMain}, Should Compile: {ShouldCompile}\nCode:\n{Code}";
        }
    }
}
