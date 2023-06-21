using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RippleUnitTests.RippleTesting
{
    static class AdditionalTestCode
    {
        public const string CODE =
            "unsafe extern \"C\" func printf(char* msg) -> void;\n" +
            "func print(int num) -> void {}\n" +
            "func print(float num) -> void {}\n" +
            "func print(char c) -> void {}\n\n";
    }
}
