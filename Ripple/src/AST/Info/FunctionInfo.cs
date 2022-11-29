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
        public readonly Token NameToken;
        public readonly List<ParameterInfo> Parameters;
        public readonly TypeInfo ReturnType;

        public string Name => NameToken.Text;

        public FunctionInfo(FuncDecl func)
        {
            NameToken = func.Name;
            Parameters = func.Param.ParamList
                .ConvertAll(p => new ParameterInfo(p.Item2, TypeInfo.FromASTType(p.Item1)));

            ReturnType = TypeInfo.FromASTType(func.ReturnType);
        }

        public FunctionInfo(ExternalFuncDecl func)
        {
            NameToken = func.Name;
            Parameters = func.Parameters.ParamList
                .ConvertAll(p => new ParameterInfo(p.Item2, TypeInfo.FromASTType(p.Item1)));

            ReturnType = TypeInfo.FromASTType(func.ReturnType);
        }

        public FunctionInfo(Token name, List<ParameterInfo> parameters, TypeInfo returnType)
        {
            NameToken = name;
            Parameters = parameters;
            ReturnType = returnType;
        }
    }
}
