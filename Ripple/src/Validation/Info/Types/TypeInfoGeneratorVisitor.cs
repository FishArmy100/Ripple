using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils;
using Ripple.Lexing;
using Ripple.AST.Utils;
using Ripple.AST;
using Raucse;
using Ripple.Validation.Errors;
using Ripple.Validation.Info.Lifetimes;
using Ripple.Validation.Info.Members;
using Ripple.Core;

namespace Ripple.Validation.Info.Types
{
    class TypeInfoGeneratorVisitor : ITypeNameVisitor<Result<TypeInfo, List<ValidationError>>>
    {
        private readonly IReadOnlyList<string> m_PrimaryTypes;
        private readonly List<string> m_ExternalLifetimes = new List<string>();
        private readonly Stack<FunctionLifetimes> m_ActiveLifetimes = new Stack<FunctionLifetimes>();
        private int m_FunctionPointerIndex = 0;

        private readonly bool m_RequireLifetimes;
        private readonly SafetyContext m_SafetyContext;

        public TypeInfoGeneratorVisitor(IReadOnlyList<string> primaryTypes, List<string> externalLifetimes, bool requireLifetimes, SafetyContext safetyContext)
        {
            m_PrimaryTypes = primaryTypes;
            m_ExternalLifetimes.AddRange(externalLifetimes);
            m_RequireLifetimes = requireLifetimes;
            m_SafetyContext = safetyContext;
        }

        public static Result<FuncPtrInfo, List<ValidationError>> GenerateFromFuncDecl(FuncDecl funcDecl, IReadOnlyList<string> primaryTypes)
		{
            SafetyContext context = new SafetyContext(!funcDecl.UnsafeToken.HasValue);
            TypeInfoGeneratorVisitor visitor = new TypeInfoGeneratorVisitor(primaryTypes, new List<string>(), true, context);
			
            List<Token> lifetimes = funcDecl.GenericParams.Match(
                ok => ok.Lifetimes.Select(l => l).ToList(), 
                () => new List<Token>());

			List<TypeName> parameterTypes = funcDecl.Param.ParamList.Select(p => p.First).ToList();
            FuncPtr funcPtr = new FuncPtr(new Token(), lifetimes, new Token(), parameterTypes, new Token(), new Token(), funcDecl.ReturnType);
            return funcPtr.Accept(visitor).Match(ok => new Result<FuncPtrInfo, List<ValidationError>>(ok as FuncPtrInfo), fail => new Result<FuncPtrInfo, List<ValidationError>>(fail));
		}

        public static Result<FuncPtrInfo, List<ValidationError>> GenerateFromExternalFuncDecl(ExternalFuncDecl funcDecl, IReadOnlyList<string> primaryTypes)
        {
            SafetyContext context = new SafetyContext(false);
            TypeInfoGeneratorVisitor visitor = new TypeInfoGeneratorVisitor(primaryTypes, new List<string>(), true, context);

            List<TypeName> parameterTypes = funcDecl.Parameters.ParamList.Select(p => p.First).ToList();
            FuncPtr funcPtr = new FuncPtr(new Token(), new Option<List<Token>>(), new Token(), parameterTypes, new Token(), new Token(), funcDecl.ReturnType);
            return funcPtr.Accept(visitor).Match(ok => new Result<FuncPtrInfo, List<ValidationError>>(ok as FuncPtrInfo), fail => new Result<FuncPtrInfo, List<ValidationError>>(fail));
        }

        public static Result<FuncPtrInfo, List<ValidationError>> GenerateFromMemberFuncDecl(MemberFunctionDecl memberFunc, IReadOnlyList<string> primaryTypes, string declaringTypeName)
        {
            SafetyContext context = new SafetyContext(!memberFunc.UnsafeToken.HasValue);
            TypeInfoGeneratorVisitor visitor = new TypeInfoGeneratorVisitor(primaryTypes, new List<string>(), true, context);

            List<Token> lifetimes = memberFunc.GenericParameters.Match(
                ok => ok.Lifetimes.Select(l => l).ToList(),
                () => new List<Token>());

            List<TypeName> parameterTypes = new List<TypeName>();

            memberFunc.Parameters.ThisParameter.Match(ok =>
            {
                TypeName thisType = new BasicType(new Token(declaringTypeName, new SourceLocation(), TokenType.Identifier, false));
                if(ok.RefToken.HasValue)
                {
                    thisType = new ReferenceType(thisType, ok.MutToken, ok.RefToken.Value, ok.LifetimeToken);
                }

                parameterTypes.Add(thisType);
            });

            parameterTypes.AddRange(memberFunc.Parameters.ParamList.Select(p => p.First).ToList());

            FuncPtr funcPtr = new FuncPtr(new Token(), lifetimes, new Token(), parameterTypes, new Token(), new Token(), memberFunc.ReturnType);
            return funcPtr.Accept(visitor).Match(
                ok => new Result<FuncPtrInfo, List<ValidationError>>(ok as FuncPtrInfo), 
                fail => new Result<FuncPtrInfo, List<ValidationError>>(fail));
        }

        public Result<TypeInfo, List<ValidationError>> VisitArrayType(ArrayType arrayType)
        {
            return arrayType.BaseType.Accept(this).Match(ok =>
            {
                int size = int.Parse(arrayType.Size.Text);
                if (ok is BasicTypeInfo b && b.Name == RipplePrimitives.VoidName)
                    return BadResult<TypeInfo>(new VoidPlacementError((arrayType.BaseType as BasicType).Identifier.Location));
                else
                    return GoodResult<TypeInfo>(new ArrayInfo(ok, size));
            },
            fail =>
            {
                return BadResult<TypeInfo>(fail);
            });
        }

        public Result<TypeInfo, List<ValidationError>> VisitBasicType(BasicType basicType)
        {
            string typeName = basicType.Identifier.Text;

            if (m_PrimaryTypes.Contains(typeName))
                return GoodResult<TypeInfo>(new BasicTypeInfo(typeName));
            return BadResult<TypeInfo>(new DefinitionError.Type(basicType.Identifier.Location, false, basicType));
        }

        public Result<TypeInfo, List<ValidationError>> VisitFuncPtr(FuncPtr funcPtr)
        {
            List<ValidationError> errors = new List<ValidationError>();
            List<TypeInfo> parameters = new List<TypeInfo>();
            List<string> lifetimes = new List<string>();

            funcPtr.Lifetimes.Match(ok => CheckLifetimes(ok, errors, lifetimes));
            m_ActiveLifetimes.Push(new FunctionLifetimes(m_FunctionPointerIndex, lifetimes));

            CheckParameters(funcPtr, errors, parameters);

            Option<TypeInfo> returned = funcPtr.ReturnType.Accept(this).Match(ok =>
            {
                return new Option<TypeInfo>(ok);
            },
            fail =>
            {
                errors.AddRange(fail);
                return new Option<TypeInfo>();
            });

            m_ActiveLifetimes.Pop();

            if (errors.Count > 0)
                return errors;

            FuncPtrInfo info = new FuncPtrInfo(m_FunctionPointerIndex, lifetimes.Count, parameters, returned.Value);
            m_FunctionPointerIndex++;
            return info;
        }

        private void CheckParameters(FuncPtr funcPtr, List<ValidationError> errors, List<TypeInfo> parameters)
        {
            foreach (TypeName name in funcPtr.Parameters)
            {
                name.Accept(this).Match(ok =>
                {
                    if (ok is BasicTypeInfo b && b.Name == RipplePrimitives.VoidName)
                        errors.Add(new VoidPlacementError((name as BasicType).Identifier.Location));
                    else
                        parameters.Add(ok);
                },
                fail =>
                {
                    errors.AddRange(fail);
                });
            }
        }

        private void CheckLifetimes(List<Token> lifetimeTokens, List<ValidationError> errors, List<string> lifetimes)
        {
            foreach (Token token in lifetimeTokens)
            {
                string lifetime = token.Text;
                if (IsLifetimeDeclared(lifetime) || lifetimes.Contains(lifetime))
                {
                    errors.Add(new DefinitionError.Lifetime(token.Location, true, lifetime));
                }
                else
                {
                    lifetimes.Add(lifetime);
                }
            }
        }

        private bool IsLifetimeDeclared(string lifetime) => 
            m_ExternalLifetimes.Contains(lifetime) || 
            m_ActiveLifetimes.Any(l => l.Lifetimes.Contains(lifetime));

		public Result<TypeInfo, List<ValidationError>> VisitGroupedType(GroupedType groupedType)
        {
            return groupedType.Type.Accept(this);
        }

        public Result<TypeInfo, List<ValidationError>> VisitPointerType(PointerType pointerType)
        {
            if (m_SafetyContext.IsSafe)
                return BadResult<TypeInfo>(UnsafeThingIsInSafeContextError.Type(pointerType.GetLocation(), pointerType));

            bool isMutable = pointerType.MutToken.HasValue;
            return pointerType.BaseType.Accept(this).Match(
                ok => new PointerInfo(isMutable, ok),
                fail => BadResult<TypeInfo>(fail));
        }

        public Result<TypeInfo, List<ValidationError>> VisitReferenceType(ReferenceType referenceType)
        {
            Option<Token> lifetime = referenceType.Lifetime ?? new Option<Token>();
            var result = CheckReferenceLifetime(lifetime, referenceType.Ampersand);
            if (result.IsError())
                return result.Error;

            bool isMutable = referenceType.MutToken.HasValue;
            return referenceType.BaseType.Accept(this).Match(
                ok => 
                {
                    if (ok is BasicTypeInfo b && b.Name == RipplePrimitives.VoidName)
                        return BadResult<TypeInfo>(new VoidPlacementError(referenceType.GetLocation()));
                    else
                        return GoodResult<TypeInfo>(new ReferenceInfo(isMutable, ok, result.Value)); 
                },
                fail => BadResult<TypeInfo>(fail));
        }

        private Result<Option<ReferenceLifetime>, List<ValidationError>> CheckReferenceLifetime(Option<Token> lifetimeOption, Token errorToken)
		{
            if (!lifetimeOption.HasValue() && m_RequireLifetimes)
                return BadResult<Option<ReferenceLifetime>>(new LifetimeRequiredError(errorToken.Location));

            if (!lifetimeOption.HasValue())
                return new Option<ReferenceLifetime>();

            Token lifetime = lifetimeOption.Value;
            string lifetimeText = lifetime.Text;

            if (m_ExternalLifetimes.Contains(lifetimeText))
                return new Option<ReferenceLifetime>(new ReferenceLifetime(new LifetimeInfo(lifetime)));

            foreach(FunctionLifetimes lifetimes in m_ActiveLifetimes)
			{
                int index = lifetimes.Lifetimes.IndexOf(lifetimeText);
                if (index >= 0)
                    return new Option<ReferenceLifetime>(new ReferenceLifetime(new GenericLifetime(index, m_FunctionPointerIndex)));
			}

            return BadResult<Option<ReferenceLifetime>>(new DefinitionError.Lifetime(lifetime.Location, false, lifetimeText));
        }

        private static Result<T, List<ValidationError>> GoodResult<T>(T info)
        {
            return new Result<T, List<ValidationError>>(info);
        }

        private static Result<T, List<ValidationError>> BadResult<T>(ValidationError error)
        {
            return new Result<T, List<ValidationError>>(new List<ValidationError> { error });
        }

        private static Result<T, List<ValidationError>> BadResult<T>(List<ValidationError> errors)
        {
            return new Result<T, List<ValidationError>>(errors);
		}

		private struct FunctionLifetimes
		{
            public readonly int FunctionIndex;
            public readonly List<string> Lifetimes;

			public FunctionLifetimes(int functionIndex, List<string> lifetimes)
			{
				FunctionIndex = functionIndex;
				Lifetimes = lifetimes;
			}
		}
	}
}
