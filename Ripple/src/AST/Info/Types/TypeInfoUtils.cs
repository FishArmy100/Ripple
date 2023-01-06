using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils;

namespace Ripple.AST.Info.Types
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
                FuncPtrInfo     f => new FuncPtrInfo(isMutable, f.Lifetimes, f.Parameters, f.Returned),
                _ => throw new ArgumentException("No case for type: " + typeInfo.GetType())
            };
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

        public static FuncPtrInfo FromFunctionInfo(FunctionInfo info)
        {
            List<LifetimeInfo> lifetimes = info.Lifetimes.ConvertAll(l => new LifetimeInfo(l));
            List<TypeInfo> parameters = info.Parameters.ConvertAll(p => p.Type);
            return new FuncPtrInfo(false, lifetimes, parameters, info.ReturnType);
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
                    r.Walk(func);
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

        public static List<LifetimeInfo> GetDeclaredLifetimes(this TypeInfo info)
        {
            List<LifetimeInfo> lifetimes = new List<LifetimeInfo>();
            info.Walk((t) =>
            {
                if (t is ReferenceInfo r)
                    r.Lifetime.Match(ok => lifetimes.Add(ok));
            });

            return lifetimes;
        }

        public static Result<TypeInfo, List<ASTInfoError>> FromASTType(TypeName typeName, List<string> primaryTypes, List<LifetimeInfo> activeLifetimes, SafetyContext safetyContext, bool requireLifetimes = false)
        {
            TypeInfoGeneratorVisitor visitor = new TypeInfoGeneratorVisitor(primaryTypes, activeLifetimes, requireLifetimes, safetyContext);
            return typeName.Accept(visitor);
        }

        public static bool IsEquatableTo(this TypeInfo self, TypeInfo other)
        {

        }
    }
}
