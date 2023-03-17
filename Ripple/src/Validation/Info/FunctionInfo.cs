using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Utils;
using Ripple.Validation.Info.Types;
using Ripple.AST;

namespace Ripple.Validation.Info
{
    class FunctionInfo
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

        /// <summary>
        /// For internal uses only, does not perform any safety checks. For builtins like the print() function, ect
        /// </summary>
        /// <param name="isUnsafe"></param>
        /// <param name="name"></param>
        /// <param name="returned"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static FunctionInfo CreateFunction(bool isUnsafe, string name, TypeInfo returned, List<ParameterInfo> parameters)
		{
            return new FunctionInfo(
                new FuncPtrInfo(false, 0, 0, parameters.Select(p => p.Type).ToList(), returned), 
                isUnsafe, 
                new Token(name, TokenType.Identifier, -1, -1), 
                new List<Token>(), 
                parameters, 
                returned);
		}

        public static Result<FunctionInfo, List<ASTInfoError>> FromASTFunction(FuncDecl funcDecl, IReadOnlyList<string> primaries)
        {
            var result = TypeInfoGeneratorVisitor.GenerateFromFuncDecl(funcDecl, primaries);
            return result.Match(ok =>
            {
				FuncPtrInfo funcPtr = ok;
				bool isUnsafe = funcDecl.UnsafeToken.HasValue;

                List<Token> lifetimes = funcDecl.GenericParams.Match(
                    ok => ok.Lifetimes.Select(l => l).ToList(),
                    () => new List<Token>());

                List<Token> parameterNames = funcDecl.Param.ParamList.Select(p => p.Item2).ToList();

                List<ParameterInfo> parameterInfos = funcPtr.Parameters.Zip(parameterNames, (t, n) => new ParameterInfo(n, t)).ToList();

                FunctionInfo info = new FunctionInfo(ok, isUnsafe, funcDecl.Name, lifetimes, parameterInfos, funcPtr.Returned);
                return new Result<FunctionInfo, List<ASTInfoError>>(info);
            },
            fail =>
            {
                return new Result<FunctionInfo, List<ASTInfoError>>(fail);
            });
        }

        public static Result<FunctionInfo, List<ASTInfoError>> FromASTExternalFunction(ExternalFuncDecl funcDecl, IReadOnlyList<string> primaries)
        {
            var result = TypeInfoGeneratorVisitor.GenerateFromExternalFuncDecl(funcDecl, primaries);
            return result.Match(ok =>
            {
                FuncPtrInfo funcPtr = ok as FuncPtrInfo;

                List<Token> parameterNames = funcDecl.Parameters.ParamList.Select(p => p.Item2).ToList();

                List<ParameterInfo> parameterInfos = funcPtr.Parameters.Zip(parameterNames, (t, n) => new ParameterInfo(n, t)).ToList();

                FunctionInfo info = new FunctionInfo(ok, true, funcDecl.Name, new List<Token>(), parameterInfos, funcPtr.Returned);
                return new Result<FunctionInfo, List<ASTInfoError>>(info);
            },
            fail =>
            {
                return new Result<FunctionInfo, List<ASTInfoError>>(fail);
            });
        }
    }
}
