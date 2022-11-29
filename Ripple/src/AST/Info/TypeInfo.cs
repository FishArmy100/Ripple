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

        public abstract bool IsUnsafe();
        public abstract List<PrimaryTypeInfo> GetPrimaries();

        public static TypeInfo FromASTType(TypeName type)
        {
            TypeInfoHelperVisitor visitor = new TypeInfoHelperVisitor();
            return visitor.VisitTypeName(type);
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

            public override bool IsUnsafe()
            {
                return false;
            }

            public override List<PrimaryTypeInfo> GetPrimaries()
            {
                return new List<PrimaryTypeInfo> { NameType };
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

            public override List<PrimaryTypeInfo> GetPrimaries()
            {
                return Contained.GetPrimaries();
            }

            public override bool IsUnsafe() => true; 
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

            public override List<PrimaryTypeInfo> GetPrimaries()
            {
                throw new NotImplementedException();
            }

            public override bool IsUnsafe() => Contained.IsUnsafe();
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

            public override List<PrimaryTypeInfo> GetPrimaries()
            {
                List<PrimaryTypeInfo> primaries = Parameters.SelectMany(p => p.GetPrimaries()).ToList();
                primaries.AddRange(Returned.GetPrimaries());
                return primaries;
            }

            public override bool IsUnsafe() => Parameters.Any(t => t.IsUnsafe()) || Returned.IsUnsafe();
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

            public override bool IsUnsafe()
            {
                return Type.IsUnsafe();
            }

            public override List<PrimaryTypeInfo> GetPrimaries()
            {
                return Type.GetPrimaries();
            }
        }
    }
}
