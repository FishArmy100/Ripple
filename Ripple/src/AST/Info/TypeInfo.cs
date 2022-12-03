using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Utils.Extensions;

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
        public abstract TypeInfo ChangeMutable(bool isMutable);
        public abstract bool HasNonPointerVoid();

        public abstract string ToPrettyString();

        public static TypeInfo FromASTType(TypeName type)
        {
            TypeInfoHelperVisitor visitor = new TypeInfoHelperVisitor();
            return visitor.VisitTypeName(type);
        }

        protected string GetMutStrAfter() => (Mutable ? " mut" : "");
        protected string GetMutStrBefore() => (Mutable ? "mut " : "");

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

            public override TypeInfo ChangeMutable(bool isMutable)
            {
                return new Basic(isMutable, NameType);
            }

            public override string ToPrettyString()
            {
                return GetMutStrBefore() + Name;
            }

            public override bool HasNonPointerVoid()
            {
                return Name == Utils.RipplePrimitives.VoidName;
            }
        }

        public class Pointer : TypeInfo
        {
            public readonly TypeInfo Contained;

            public Pointer(bool mutable, TypeInfo contained) : base(mutable)
            {
                Contained = contained;
            }

            public override TypeInfo ChangeMutable(bool isMutable)
            {
                return new Pointer(isMutable, Contained);
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

            public override bool HasNonPointerVoid()
            {
                if (Contained is Basic)
                    return false;
                else
                    return Contained.HasNonPointerVoid();
            }

            public override bool IsUnsafe() => true;

            public override string ToPrettyString()
            {
                if(Contained is FunctionPointer)
                {
                    return "(" + Contained.ToPrettyString() + ")" + GetMutStrAfter() + "*";
                }
                else
                {
                    return Contained.ToPrettyString() + GetMutStrAfter() + "*";
                }
            }
        }

        public class Reference : TypeInfo
        {
            public readonly TypeInfo Contained;

            public Reference(bool mutable, TypeInfo contained) : base(mutable)
            {
                Contained = contained;
            }

            public override TypeInfo ChangeMutable(bool isMutable)
            {
                return new Reference(isMutable, Contained);
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
                return Contained.GetPrimaries();
            }

            public override string ToPrettyString()
            {
                if (Contained is FunctionPointer)
                {
                    return "(" + Contained.ToPrettyString() + ")" + GetMutStrAfter() + "&";
                }
                else
                {
                    return Contained.ToPrettyString() + GetMutStrAfter() + "&";
                }
            }

            public override bool IsUnsafe() => Contained.IsUnsafe();

            public override bool HasNonPointerVoid()
            {
                return Contained.HasNonPointerVoid();
            }
        }

        public class FunctionPointer : TypeInfo
        {
            public readonly List<TypeInfo> Parameters;
            public readonly TypeInfo Returned;

            public FunctionPointer(bool mutable, List<TypeInfo> parameters, TypeInfo returned) : base(mutable)
            {
                Parameters = parameters;
                Returned = returned;
            }

            public FunctionPointer(FunctionInfo info) : base(false)
            {
                Parameters = info.Parameters.ConvertAll(p => p.Type);
                Returned = info.ReturnType;
            }

            public override TypeInfo ChangeMutable(bool isMutable)
            {
                return new FunctionPointer(isMutable, Parameters, Returned);
            }

            public override bool Equals(object obj)
            {
                return obj is FunctionPointer pointer &&
                       Mutable == pointer.Mutable &&
                       Parameters.SequenceEqual(pointer.Parameters) &&
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

            public override string ToPrettyString()
            {
                return GetMutStrBefore() + "(" + Parameters.ToList().ConvertAll(p => p.ToPrettyString()).Concat(", ") + 
                    ")->" + Returned.ToPrettyString();
            }

            public override bool HasNonPointerVoid()
            {
                foreach(TypeInfo info in Parameters)
                {
                    if (info.HasNonPointerVoid())
                        return true;
                }

                return Returned.HasNonPointerVoid();
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

            public override bool IsUnsafe()
            {
                return Type.IsUnsafe();
            }

            public override List<PrimaryTypeInfo> GetPrimaries()
            {
                return Type.GetPrimaries();
            }

            public override TypeInfo ChangeMutable(bool isMutable)
            {
                return new Array(isMutable, Type, SizeToken);
            }

            public override string ToPrettyString()
            {
                return Type.ToPrettyString() + GetMutStrAfter() + "[" + Size.ToString() + "]";
            }

            public override bool HasNonPointerVoid()
            {
                return Type.HasNonPointerVoid();
            }
        }
    }
}
