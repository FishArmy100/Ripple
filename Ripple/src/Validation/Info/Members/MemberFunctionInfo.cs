using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Validation.Info.Functions;
using Ripple.Lexing;
using Raucse;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Errors;
using Ripple.AST;

namespace Ripple.Validation.Info.Members
{
    public class MemberFunctionInfo : ClassMember
    {
        public readonly string DeclaringTypeName;
        public readonly bool IsUnsafe;
        public readonly Token NameToken;
        public readonly Option<ThisMemberFuncParamType> ThisParam;
        public readonly IReadOnlyList<ParameterInfo> Parameters;
        public readonly IReadOnlyList<Token> Lifetimes;
        public readonly TypeInfo ReturnType;

        public readonly FuncPtrInfo FunctionType;

        public string Name => NameToken.Text;

        public MemberFunctionInfo(string declaringTypeName, MemberVisibility visibility, bool isUnsafe, Token nameToken, Option<ThisMemberFuncParamType> thisParam, IReadOnlyList<ParameterInfo> parameters, IReadOnlyList<Token> lifetimes, TypeInfo returnType, FuncPtrInfo functionType) : base(visibility)
        {
            DeclaringTypeName = declaringTypeName;
            IsUnsafe = isUnsafe;
            NameToken = nameToken;
            ThisParam = thisParam;
            Parameters = parameters;
            Lifetimes = lifetimes;
            ReturnType = returnType;
            FunctionType = functionType;
        }

        public static Result<MemberFunctionInfo, List<ValidationError>> FromASTMemberFunction(MemberFunctionDecl memberFunc, IReadOnlyList<string> primaries, string declaringTypeName, IReadOnlyList<Token> declaringTypeLifetimes, MemberVisibility visibility)
        {
            var info = TypeInfoGeneratorVisitor.GenerateFromMemberFuncDecl(memberFunc, primaries, declaringTypeName);
            return info.Match(
                ok =>
                {
                    List<Token> parameterNames = memberFunc.Parameters.ParamList.Select(p => p.Second).ToList();
                    List<ParameterInfo> parameterInfos = ok.Parameters.Zip(parameterNames, (t, n) => new ParameterInfo(n, t)).ToList();
                    Option<ThisMemberFuncParamType> thisType = GetThisParamType(memberFunc.Parameters.ThisParameter);

                    var (lifetimes, errors) = CheckLifetimes(memberFunc.GenericParameters, declaringTypeLifetimes);

                    if (errors.Any())
                        return errors;

                    return new MemberFunctionInfo(declaringTypeName, visibility, memberFunc.UnsafeToken.HasValue, memberFunc.NameToken, thisType, parameterInfos, lifetimes, ok.Returned, ok);
                },
                fail =>
                {
                    return new Result<MemberFunctionInfo, List<ValidationError>>(fail);
                });

        }

        private static Option<ThisMemberFuncParamType> GetThisParamType(Option<ThisFunctionParameter> thisParameter)
        {
            return thisParameter.Match(
                ok =>
                {
                    if(ok.MutToken.HasValue && ok.RefToken.HasValue)
                    {
                        return ThisMemberFuncParamType.RefMutThis;
                    }
                    else if(ok.RefToken.HasValue)
                    {
                        return ThisMemberFuncParamType.RefThis;
                    }
                    else
                    {
                        return ThisMemberFuncParamType.This;
                    }
                },
                () =>
                {
                    return new Option<ThisMemberFuncParamType>();
                });
        }
    }
}
