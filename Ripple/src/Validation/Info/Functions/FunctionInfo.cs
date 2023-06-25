using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Utils;
using Ripple.Validation.Info.Types;
using Ripple.AST;
using Raucse;
using Ripple.Validation.Errors;

namespace Ripple.Validation.Info.Functions
{
    public class FunctionInfo
    {
        public readonly bool IsUnsafe;
        public readonly Token NameToken;
        public readonly List<ParameterInfo> Parameters;
        public readonly List<Token> Lifetimes;
        public readonly TypeInfo ReturnType;

        public readonly FuncPtrInfo FunctionType;

        public string Name => NameToken.Text;

        private FunctionInfo(FuncPtrInfo functionType, bool isUnsafe, Token name, List<Token> lifetimes, List<ParameterInfo> parameters, TypeInfo returnType)
        {
            FunctionType = functionType;
            IsUnsafe = isUnsafe;
            NameToken = name;
            Lifetimes = lifetimes;
            Parameters = parameters;
            ReturnType = returnType;
        }

        public static Result<FunctionInfo, List<ValidationError>> FromASTFunction(FuncDecl funcDecl, IReadOnlyList<string> primaries)
        {
            var result = TypeInfoGeneratorVisitor.GenerateFromFuncDecl(funcDecl, primaries);
            return result.Match(ok =>
            {
				FuncPtrInfo funcPtr = ok;
				bool isUnsafe = funcDecl.UnsafeToken.HasValue;

                List<Token> lifetimes = funcDecl.GenericParams.Match(
                    ok => ok.Lifetimes.Select(l => l).ToList(),
                    () => new List<Token>());

                List<Token> parameterNames = funcDecl.Param.ParamList.Select(p => p.Second).ToList();
                var errors = parameterNames.FindDuplicates()
                    .Select(d => new ParameterWithSameNameError(d.Location, d.Text))
                    .Cast<ValidationError>()
                    .ToList();

                if (errors.Any())
                    return errors;

                List<ParameterInfo> parameterInfos = funcPtr.Parameters.Zip(parameterNames, (t, n) => new ParameterInfo(n, t)).ToList();

                FunctionInfo info = new FunctionInfo(ok, isUnsafe, funcDecl.Name, lifetimes, parameterInfos, funcPtr.Returned);
                return new Result<FunctionInfo, List<ValidationError>>(info);
            },
            fail =>
            {
                return new Result<FunctionInfo, List<ValidationError>>(fail);
            });
        }

        public static Result<FunctionInfo, List<ValidationError>> FromASTExternalFunction(ExternalFuncDecl funcDecl, IReadOnlyList<string> primaries)
        {
            var result = TypeInfoGeneratorVisitor.GenerateFromExternalFuncDecl(funcDecl, primaries);
            return result.Match(ok =>
            {
                List<Token> parameterNames = funcDecl.Parameters.ParamList.Select(p => p.Second).ToList();

                List<ParameterInfo> parameterInfos = ok.Parameters.Zip(parameterNames, (t, n) => new ParameterInfo(n, t)).ToList();

                FunctionInfo info = new FunctionInfo(ok, true, funcDecl.Name, new List<Token>(), parameterInfos, ok.Returned);
                return new Result<FunctionInfo, List<ValidationError>>(info);
            },
            fail =>
            {
                return new Result<FunctionInfo, List<ValidationError>>(fail);
            });
        }

        public override bool Equals(object obj)
        {
            return obj is FunctionInfo info &&
                   IsUnsafe == info.IsUnsafe &&
                   FunctionType.Equals(info.FunctionType) &&
                   Name == info.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IsUnsafe, FunctionType, Name);
        }
    }
}
