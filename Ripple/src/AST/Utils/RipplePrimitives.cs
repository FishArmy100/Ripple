using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Validation;
using Ripple.Validation.Info;
using Ripple.Lexing;
using Ripple.Validation.Info.Types;

namespace Ripple.AST.Utils
{
    public static class RipplePrimitives
    {
        public const string Int32Name = "int";
        public const string Float32Name = "float";
        public const string BoolName = "bool";
        public const string VoidName = "void";
        public const string CharName = "char";

        public static readonly BasicTypeInfo Int32 = GenPrimative(Int32Name);
        public static readonly BasicTypeInfo Float32 = GenPrimative(Float32Name);
        public static readonly BasicTypeInfo Bool = GenPrimative(BoolName);
        public static readonly BasicTypeInfo Char = GenPrimative(CharName);
        public static readonly BasicTypeInfo Void = GenPrimative(VoidName);

        public static readonly PointerInfo CString = new PointerInfo(false, Char);

        private static BasicTypeInfo GenPrimative(string name)
        {
            return new BasicTypeInfo(name);
        }
    }
}
