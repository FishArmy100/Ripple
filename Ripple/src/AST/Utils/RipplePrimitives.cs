﻿using System;
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
    static class RipplePrimitives
    {
        public const string Int32Name = "int";
        public const string Float32Name = "float";
        public const string BoolName = "bool";
        public const string VoidName = "void";
        public const string CharName = "char";

        public static readonly TypeInfo Int32 = GenPrimative(Int32Name);
        public static readonly TypeInfo Float32 = GenPrimative(Float32Name);
        public static readonly TypeInfo Bool = GenPrimative(BoolName);
        public static readonly TypeInfo Char = GenPrimative(CharName);
        public static readonly TypeInfo Void = GenPrimative(VoidName);

        private static TypeInfo GenPrimative(string name)
        {
            return new BasicTypeInfo(false, name);
        }
    }
}
