using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils;
using Ripple.Lexing;
using Ripple.AST.Utils;

namespace Ripple.AST.Info.Types
{
    class TypeInfoGeneratorVisitor : ITypeNameVisitor<Result<TypeInfo, List<ASTInfoError>>>
    {
        private readonly List<string> m_PrimaryTypes;
        private readonly Stack<List<LifetimeInfo>> m_ActiveLifetimes = new Stack<List<LifetimeInfo>>();
        private readonly bool m_RequireLifetimes;
        private readonly SafetyContext m_SafetyContext;

        public TypeInfoGeneratorVisitor(List<string> primaryTypes, List<LifetimeInfo> activeLifetimes, bool requireLifetimes, SafetyContext safetyContext)
        {
            m_PrimaryTypes = primaryTypes;
            m_ActiveLifetimes.Push(activeLifetimes);
            m_RequireLifetimes = requireLifetimes;
            m_SafetyContext = safetyContext;
        }

        public Result<TypeInfo, List<ASTInfoError>> VisitArrayType(ArrayType arrayType)
        {
            return arrayType.BaseType.Accept(this).Match(ok =>
            {
                bool isMutable = arrayType.MutToken.HasValue;
                int size = int.Parse(arrayType.Size.Text);
                if (ok is BasicTypeInfo b && b.Name == RipplePrimitives.VoidName)
                    return BadResult(new ASTInfoError("Void can only be used in the context of a return type, or as a void*.", (arrayType.BaseType as BasicType).Identifier));
                else
                    return GoodResult(new ArrayInfo(isMutable, ok, size));
            },
            fail =>
            {
                return BadResult(fail);
            });
        }

        public Result<TypeInfo, List<ASTInfoError>> VisitBasicType(BasicType basicType)
        {
            string typeName = basicType.Identifier.Text;
            bool isMutable = basicType.MutToken.HasValue;

            if (m_PrimaryTypes.Contains(typeName))
                return GoodResult(new BasicTypeInfo(isMutable, typeName));
            return BadResult(new ASTInfoError("Type name '" + typeName + "' has not been defiend.", basicType.Identifier));
        }

        public Result<TypeInfo, List<ASTInfoError>> VisitFuncPtr(FuncPtr funcPtr)
        {
            bool isMutable = funcPtr.MutToken.HasValue;
            List<ASTInfoError> errors = new List<ASTInfoError>();
            List<TypeInfo> parameters = new List<TypeInfo>();
            List<LifetimeInfo> lifetimes = new List<LifetimeInfo>();

            funcPtr.Lifetimes.Match(ok => CheckLifetimes(ok, errors, lifetimes));
            m_ActiveLifetimes.Push(lifetimes);

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

            return new FuncPtrInfo(isMutable, lifetimes, parameters, returned.Value);
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

        private void CheckLifetimes(List<Token> lifetimeTokens, List<ASTInfoError> errors, List<LifetimeInfo> lifetimes)
        {
            foreach (Token token in lifetimeTokens)
            {
                LifetimeInfo lifetime = new LifetimeInfo(token);
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

        public Result<TypeInfo, List<ASTInfoError>> VisitGroupedType(GroupedType groupedType)
        {
            return groupedType.Type.Accept(this);
        }

        public Result<TypeInfo, List<ASTInfoError>> VisitPointerType(PointerType pointerType)
        {
            if (m_SafetyContext.IsSafe)
                return BadResult(new ASTInfoError("Pointer type cannot be used in a safe context.", pointerType.Star));

            bool isMutable = pointerType.MutToken.HasValue;
            return pointerType.BaseType.Accept(this).Match(
                ok => new PointerInfo(isMutable, ok),
                fail => BadResult(fail));
        }

        public Result<TypeInfo, List<ASTInfoError>> VisitReferenceType(ReferenceType referenceType)
        {
            Option<LifetimeInfo> lifetime = referenceType.Lifetime.HasValue ? new LifetimeInfo(referenceType.Lifetime.Value) : new Option<LifetimeInfo>();
            if (lifetime.HasValue() && !IsLifetimeDeclared(lifetime.Value))
                return BadResult(new ASTInfoError("Lifetime '" + lifetime.Value.ToString() + "' has not been defined", referenceType.Lifetime.Value));

            if (!lifetime.HasValue() && m_RequireLifetimes)
                return BadResult(new ASTInfoError("Lifetime is required.", referenceType.Ampersand));

            bool isMutable = referenceType.MutToken.HasValue;
            return referenceType.BaseType.Accept(this).Match(
                ok => 
                {
                    if (ok is BasicTypeInfo b && b.Name == RipplePrimitives.VoidName)
                        return BadResult(new ASTInfoError("Void can only be used in the context of a return type, or as a void*.", (referenceType.BaseType as BasicType).Identifier));
                    else
                        return GoodResult(new ReferenceInfo(isMutable, ok, lifetime));
                },
                fail => BadResult(fail));
        }

        private bool IsLifetimeDeclared(LifetimeInfo lifetime)
        {
            foreach(var list in m_ActiveLifetimes)
            {
                if (list.Contains(lifetime))
                    return true;
            }

            return false;
        }

        private static Result<TypeInfo, List<ASTInfoError>> GoodResult(TypeInfo info)
        {
            return new Result<TypeInfo, List<ASTInfoError>>(info);
        }

        private static Result<TypeInfo, List<ASTInfoError>> BadResult(ASTInfoError error)
        {
            return new Result<TypeInfo, List<ASTInfoError>>(new List<ASTInfoError> { error });
        }

        private static Result<TypeInfo, List<ASTInfoError>> BadResult(List<ASTInfoError> errors)
        {
            return new Result<TypeInfo, List<ASTInfoError>>(errors);
        }
    }
}
