using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils;
using Ripple.AST;
using Raucse;
using Raucse.Extensions;
using Ripple.Validation.Errors;

namespace Ripple.Validation.Info.Types
{
    static class TypeInfoUtils
    {
        public static TypeInfo SetFirstMutable(this TypeInfo typeInfo, bool isMutable)
        {
            return typeInfo switch
            {
                BasicTypeInfo   b => new BasicTypeInfo(isMutable, b.Name),
                ReferenceInfo   r => new ReferenceInfo(isMutable, r.Contained, r.Lifetime),
                PointerInfo     p => new PointerInfo(isMutable, p.Contained),
                ArrayInfo       a => new ArrayInfo(isMutable, a.Contained, a.Size),
                FuncPtrInfo     f => new FuncPtrInfo(isMutable, f.FunctionIndex, f.LifetimeCount, f.Parameters, f.Returned),
                _ => throw new ArgumentException("No case for type: " + typeInfo.GetType())
            };
        }

        public static string ToPrettyString(this TypeInfo typeInfo)
		{
            string str = typeInfo switch
            {
                BasicTypeInfo b => $"{ReturnMutIfTrue(b.IsMutable)} {b.Name}",
                ReferenceInfo r => WrapIfTrue(r.Contained is FuncPtrInfo, r.Contained.ToPrettyString()) + $"{ReturnMutIfTrue(r.IsMutable)}&{r.Lifetime.Match(ok => ok.ToString(), () => "")}",
                PointerInfo p => WrapIfTrue(p.Contained is FuncPtrInfo, p.Contained.ToPrettyString()) + $"{ReturnMutIfTrue(p.IsMutable)}*",
                ArrayInfo a => $"{a.Contained.ToPrettyString()} {ReturnMutIfTrue(a.IsMutable)}[{a.Size}]",
                FuncPtrInfo f => $"{ReturnMutIfTrue(f.IsMutable)} func({f.Parameters.Select(p => p.ToPrettyString()).Concat(", ")})",
                _ => throw new ArgumentException("No case for type: " + typeInfo.GetType())
            };

            return str;
        }

        public static bool IsMutable(this TypeInfo typeInfo)
        {
            return typeInfo switch
            {
                BasicTypeInfo b => b.IsMutable,
                ReferenceInfo r => r.IsMutable,
                PointerInfo p => p.IsMutable,
                ArrayInfo a => a.IsMutable,
                FuncPtrInfo f => f.IsMutable,
                _ => throw new ArgumentException("No case for type: " + typeInfo.GetType())
            };
        }

        public static bool EqualsWithoutFirstMutable(this TypeInfo self, TypeInfo other)
        {
            return self.SetFirstMutable(false).Equals(other.SetFirstMutable(false));
        }

        public static void Walk(this TypeInfo typeInfo, Action<TypeInfo> func)
        {
            switch (typeInfo)
            {
                case BasicTypeInfo b:
                    func(b);
                    break;
                case ReferenceInfo r:
                    func(r);
                    r.Contained.Walk(func);
                    break;
                case PointerInfo p:
                    func(p);
                    p.Contained.Walk(func);
                    break;
                case ArrayInfo a:
                    func(a);
                    a.Contained.Walk(func);
                    break;
                case FuncPtrInfo fp:
                    func(fp);
                    foreach (TypeInfo parameter in fp.Parameters)
                        parameter.Walk(func);
                    fp.Returned.Walk(func);
                    break;
                default:
                    throw new ArgumentException("No case for type: " + typeInfo.GetType());
            }
        }

        public static Result<TypeInfo, List<ValidationError>> FromASTType(TypeName typeName, IReadOnlyList<string> primaryTypes, List<string> activeLifetimes, SafetyContext safetyContext, bool requireLifetimes = false)
        {
            TypeInfoGeneratorVisitor visitor = new TypeInfoGeneratorVisitor(primaryTypes, activeLifetimes, requireLifetimes, safetyContext);
            return typeName.Accept(visitor);
        }

        /// <summary>
        /// If one or both of the types does not have a lifetime, they will count references as equal
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool IsEquatableTo(this TypeInfo self, TypeInfo other)
        {
            AreTypesEquatableVisitor visitor = new AreTypesEquatableVisitor();
            return self.Accept(visitor, other);
        }

        public static bool EqualsWithoutLifetimes(this TypeInfo self, TypeInfo other)
        {
            return self switch
            {
                BasicTypeInfo b1 => other is BasicTypeInfo b2 && b1.Name == b2.Name && b1.IsMutable == b2.IsMutable,
                PointerInfo p1 => other is PointerInfo p2 && p1.Contained.EqualsWithoutLifetimes(p2.Contained) && p1.IsMutable == p2.IsMutable,
                ReferenceInfo r1 => other is ReferenceInfo r2 && r1.Contained.EqualsWithoutLifetimes(r2.Contained) && r1.IsMutable == r2.IsMutable,
                ArrayInfo a1 => other is ArrayInfo a2 && a1.Size == a2.Size && a1.Contained.EqualsWithoutLifetimes(a2.Contained) && a1.IsMutable == a2.IsMutable,
                FuncPtrInfo fp1 => other is FuncPtrInfo fp2 && FunctionTypesEqualWithoutLifetimes(fp1, fp2),
                _ => throw new ArgumentException()
            };
        }

        private static bool FunctionTypesEqualWithoutLifetimes(FuncPtrInfo a, FuncPtrInfo b)
        {
            if (!(a.IsMutable == b.IsMutable) || !(a.Parameters.Count == b.Parameters.Count))
                return false;

            for(int i = 0; i < a.Parameters.Count; i++)
            {
                if (!a.Parameters[i].EqualsWithoutLifetimes(b.Parameters[i]))
                    return false;
            }

            return a.Returned.EqualsWithoutLifetimes(b.Returned);
        }

        private static string ReturnMutIfTrue(bool condition)
		{
            if (condition)
                return "mut";

            return "";
		}

        private static string WrapIfTrue(bool condition, string str)
		{
            if (condition)
                return $"({str})";

            return str;
		}
    }
}
