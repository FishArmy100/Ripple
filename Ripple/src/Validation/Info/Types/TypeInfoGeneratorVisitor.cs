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

namespace Ripple.Validation.Info.Types
{
    class TypeInfoGeneratorVisitor : ITypeNameVisitor<Result<TypeInfo, List<ASTInfoError>>>
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

        public static Result<FuncPtrInfo, List<ASTInfoError>> GenerateFromFuncDecl(FuncDecl funcDecl, IReadOnlyList<string> primaryTypes)
		{
            SafetyContext context = new SafetyContext(!funcDecl.UnsafeToken.HasValue);
            TypeInfoGeneratorVisitor visitor = new TypeInfoGeneratorVisitor(primaryTypes, new List<string>(), true, context);
			
            List<Token> lifetimes = funcDecl.GenericParams.Match(
                ok => ok.Lifetimes.Select(l => l).ToList(), 
                () => new List<Token>());

			List<TypeName> parameterTypes = funcDecl.Param.ParamList.Select(p => p.Item1).ToList();
            FuncPtr funcPtr = new FuncPtr(null, new Token(), lifetimes, new Token(), parameterTypes, new Token(), new Token(), funcDecl.ReturnType);
            return funcPtr.Accept(visitor).Match(ok => new Result<FuncPtrInfo, List<ASTInfoError>>(ok as FuncPtrInfo), fail => new Result<FuncPtrInfo, List<ASTInfoError>>(fail));
		}

        public static Result<FuncPtrInfo, List<ASTInfoError>> GenerateFromExternalFuncDecl(ExternalFuncDecl funcDecl, IReadOnlyList<string> primaryTypes)
        {
            SafetyContext context = new SafetyContext(false);
            TypeInfoGeneratorVisitor visitor = new TypeInfoGeneratorVisitor(primaryTypes, new List<string>(), true, context);

            List<TypeName> parameterTypes = funcDecl.Parameters.ParamList.Select(p => p.Item1).ToList();
            FuncPtr funcPtr = new FuncPtr(null, new Token(), new Option<List<Token>>(), new Token(), parameterTypes, new Token(), new Token(), funcDecl.ReturnType);
            return funcPtr.Accept(visitor).Match(ok => new Result<FuncPtrInfo, List<ASTInfoError>>(ok as FuncPtrInfo), fail => new Result<FuncPtrInfo, List<ASTInfoError>>(fail));
        }

        public Result<TypeInfo, List<ASTInfoError>> VisitArrayType(ArrayType arrayType)
        {
            return arrayType.BaseType.Accept(this).Match(ok =>
            {
                bool isMutable = arrayType.MutToken.HasValue;
                int size = int.Parse(arrayType.Size.Text);
                if (ok is BasicTypeInfo b && b.Name == RipplePrimitives.VoidName)
                    return BadResult<TypeInfo>(new ASTInfoError("Void can only be used in the context of a return type, or as a void*.", (arrayType.BaseType as BasicType).Identifier));
                else
                    return GoodResult<TypeInfo>(new ArrayInfo(isMutable, ok, size));
            },
            fail =>
            {
                return BadResult<TypeInfo>(fail);
            });
        }

        public Result<TypeInfo, List<ASTInfoError>> VisitBasicType(BasicType basicType)
        {
            string typeName = basicType.Identifier.Text;
            bool isMutable = basicType.MutToken.HasValue;

            if (m_PrimaryTypes.Contains(typeName))
                return GoodResult<TypeInfo>(new BasicTypeInfo(isMutable, typeName));
            return BadResult<TypeInfo>(new ASTInfoError("Type name '" + typeName + "' has not been defiend.", basicType.Identifier));
        }

        public Result<TypeInfo, List<ASTInfoError>> VisitFuncPtr(FuncPtr funcPtr)
        {
            bool isMutable = funcPtr.MutToken.HasValue;
            List<ASTInfoError> errors = new List<ASTInfoError>();
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

            FuncPtrInfo info = new FuncPtrInfo(isMutable, m_FunctionPointerIndex, lifetimes.Count, parameters, returned.Value);
            m_FunctionPointerIndex++;
            return info;
        }

        private void CheckParameters(FuncPtr funcPtr, List<ASTInfoError> errors, List<TypeInfo> parameters)
        {
            foreach (TypeName name in funcPtr.Parameters)
            {
                name.Accept(this).Match(ok =>
                {
                    if (ok is BasicTypeInfo b && b.Name == RipplePrimitives.VoidName)
                        errors.Add(new ASTInfoError("Void can only be used in the context of a return type, or as a void*.", (name as BasicType).Identifier));
                    else
                        parameters.Add(ok);
                },
                fail =>
                {
                    errors.AddRange(fail);
                });
            }
        }

        private void CheckLifetimes(List<Token> lifetimeTokens, List<ASTInfoError> errors, List<string> lifetimes)
        {
            foreach (Token token in lifetimeTokens)
            {
                string lifetime = token.Text;
                if (IsLifetimeDeclared(lifetime) || lifetimes.Contains(lifetime))
                {
                    errors.Add(new ASTInfoError("Lifetime '" + token.Text + "' has already been defined.", token));
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

		public Result<TypeInfo, List<ASTInfoError>> VisitGroupedType(GroupedType groupedType)
        {
            return groupedType.Type.Accept(this);
        }

        public Result<TypeInfo, List<ASTInfoError>> VisitPointerType(PointerType pointerType)
        {
            if (m_SafetyContext.IsSafe)
                return BadResult<TypeInfo>(new ASTInfoError("Pointer type cannot be used in a safe context.", pointerType.Star));

            bool isMutable = pointerType.MutToken.HasValue;
            return pointerType.BaseType.Accept(this).Match(
                ok => new PointerInfo(isMutable, ok),
                fail => BadResult<TypeInfo>(fail));
        }

        public Result<TypeInfo, List<ASTInfoError>> VisitReferenceType(ReferenceType referenceType)
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
                        return BadResult<TypeInfo>(new ASTInfoError("Void can only be used in the context of a return type, or as a void*.", (referenceType.BaseType as BasicType).Identifier));
                    else
                        return GoodResult<TypeInfo>(new ReferenceInfo(isMutable, ok, result.Value)); 
                },
                fail => BadResult<TypeInfo>(fail));
        }

        private Result<Option<ReferenceLifetime>, List<ASTInfoError>> CheckReferenceLifetime(Option<Token> lifetimeOption, Token errorToken)
		{
            if (!lifetimeOption.HasValue() && m_RequireLifetimes)
                return BadResult<Option<ReferenceLifetime>>(new ASTInfoError("Lifetime is required in this context.", errorToken));

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

            return BadResult<Option<ReferenceLifetime>>(new ASTInfoError("Lifetime '" + lifetimeText + "' is not defined", lifetime));
        }

        private static Result<T, List<ASTInfoError>> GoodResult<T>(T info)
        {
            return new Result<T, List<ASTInfoError>>(info);
        }

        private static Result<T, List<ASTInfoError>> BadResult<T>(ASTInfoError error)
        {
            return new Result<T, List<ASTInfoError>>(new List<ASTInfoError> { error });
        }

        private static Result<T, List<ASTInfoError>> BadResult<T>(List<ASTInfoError> errors)
        {
            return new Result<T, List<ASTInfoError>>(errors);
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
