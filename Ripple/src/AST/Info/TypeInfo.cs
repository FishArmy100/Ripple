using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;

namespace Ripple.AST.Info
{
    abstract class TypeInfo
    {
        public readonly bool Mutable;
        public TypeInfo(bool mutable)
        {
            Mutable = mutable;
        }

        public static TypeInfo FromASTType(TypeName type)
        {
            TypeInfoHelperVisitor visitor = new TypeInfoHelperVisitor();
            return visitor.VisitTypeName(type);
        }

        public List<PrimaryTypeInfo> GetPrimaryTypes()
        {
            List<PrimaryTypeInfo> primaries = new List<PrimaryTypeInfo>();
            switch (this)
            {
                case Basic b:
                    primaries.Add(b.NameType);
                    break;
                case Pointer p:
                    primaries.AddRange(p.Contained.GetPrimaryTypes());
                    break;
                case Reference r:
                    primaries.AddRange(r.Contained.GetPrimaryTypes());
                    break;
                case Array a:
                    primaries.AddRange(a.Type.GetPrimaryTypes());
                    break;
                case FunctionPointer fp:
                    foreach (TypeInfo info in fp.Parameters)
                        primaries.AddRange(info.GetPrimaryTypes());
                    primaries.AddRange(fp.Returned.GetPrimaryTypes());
                    break;
                default:
                    throw new Exception("Unknown type used");
            }

            return primaries;
        }

        public class Basic : TypeInfo
        {
            public readonly PrimaryTypeInfo NameType;
            public string Name => NameType.Name.Text;

            public Basic(bool mutable, PrimaryTypeInfo name) : base(mutable)
            {
                NameType = name;
            }

            public override bool Equals(object obj)
            {
                return obj is Basic basic &&
                       Mutable == basic.Mutable &&
                       EqualityComparer<PrimaryTypeInfo>.Default.Equals(NameType, basic.NameType);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Mutable, NameType);
            }
        }

        public class Pointer : TypeInfo
        {
            public readonly TypeInfo Contained;

            public Pointer(bool mutable, TypeInfo contained) : base(mutable)
            {
                Contained = contained;
            }

            public override bool Equals(object obj)
            {
                return obj is Pointer pointer &&
                       Mutable == pointer.Mutable &&
                       EqualityComparer<TypeInfo>.Default.Equals(Contained, pointer.Contained);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Mutable, Contained, GetType());
            }
        }

        public class Reference : TypeInfo
        {
            public readonly TypeInfo Contained;

            public Reference(bool mutable, TypeInfo contained) : base(mutable)
            {
                Contained = contained;
            }

            public override bool Equals(object obj)
            {
                return obj is Reference reference &&
                       Mutable == reference.Mutable &&
                       EqualityComparer<TypeInfo>.Default.Equals(Contained, reference.Contained);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Mutable, Contained, GetType());
            }
        }

        public class FunctionPointer : TypeInfo
        {
            public readonly IReadOnlyList<TypeInfo> Parameters;
            public readonly TypeInfo Returned;

            public FunctionPointer(bool mutable, IReadOnlyList<TypeInfo> parameters, TypeInfo returned) : base(mutable)
            {
                Parameters = parameters;
                Returned = returned;
            }

            public override bool Equals(object obj)
            {
                return obj is FunctionPointer pointer &&
                       Mutable == pointer.Mutable &&
                       EqualityComparer<IReadOnlyList<TypeInfo>>.Default.Equals(Parameters, pointer.Parameters) &&
                       EqualityComparer<TypeInfo>.Default.Equals(Returned, pointer.Returned);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Mutable, Parameters, Returned, GetType());
            }
        }

        public class Array : TypeInfo
        {
            public readonly TypeInfo Type;
            public readonly Token SizeToken;
            public int Size => int.Parse(SizeToken.Text);

            public Array(bool mutable, TypeInfo type, Token size) : base(mutable)
            {
                Type = type;
                SizeToken = size;
            }

            public override bool Equals(object obj)
            {
                return obj is Array array &&
                       Mutable == array.Mutable &&
                       EqualityComparer<TypeInfo>.Default.Equals(Type, array.Type) &&
                       EqualityComparer<Token>.Default.Equals(SizeToken, array.SizeToken);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Mutable, Type, SizeToken, GetType());
            }
        }
    }
}
