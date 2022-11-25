using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;

namespace Ripple.AST.Info
{
    class FunctionInfo
    {
        public readonly Token Name;
        public readonly List<ParameterInfo> Parameters;
        public readonly TypeInfo ReturnType;

        public FunctionInfo(Token name, List<ParameterInfo> parameters, TypeInfo returnType)
        {
            Name = name;
            Parameters = parameters;
            ReturnType = returnType;
        }
    }
}
