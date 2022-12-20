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
        public readonly bool IsUnsafe;
        public readonly Token NameToken;
        public readonly List<ParameterInfo> Parameters;
        public readonly List<LifetimeInfo> Lifetimes;
        public readonly TypeInfo ReturnType;

        public string Name => NameToken.Text;

        public FunctionInfo(FuncDecl func)
        {
            IsUnsafe = true;
            NameToken = func.Name;
            Parameters = func.Param.ParamList
                .ConvertAll(p => new ParameterInfo(p.Item2, TypeInfo.FromASTType(p.Item1)));

            Lifetimes = func.GenericParams.HasValue() ? 
                func.GenericParams.Value.Lifetimes.ConvertAll(l => new LifetimeInfo(l)) : 
                new List<LifetimeInfo>();

            ReturnType = TypeInfo.FromASTType(func.ReturnType);
        }

        public FunctionInfo(ExternalFuncDecl func)
        {
            IsUnsafe = true;
            NameToken = func.Name;
            Parameters = func.Parameters.ParamList
                .ConvertAll(p => new ParameterInfo(p.Item2, TypeInfo.FromASTType(p.Item1)));

            Lifetimes = new List<LifetimeInfo>();

            ReturnType = TypeInfo.FromASTType(func.ReturnType);
        }

        public FunctionInfo(bool isUnsafe, Token name, List<LifetimeInfo> genericParameters, List<ParameterInfo> parameters, TypeInfo returnType)
        {
            IsUnsafe = isUnsafe;
            NameToken = name;
            Lifetimes = genericParameters;
            Parameters = parameters;
            ReturnType = returnType;
        }
    }
}
