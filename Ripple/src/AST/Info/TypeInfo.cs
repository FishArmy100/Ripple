using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Utils.Extensions;
using Ripple.Utils;
using Ripple.AST.Utils;

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
        public abstract void Walk(Action<TypeInfo> walker);

        public bool EqualsWithoutFirstMutable(TypeInfo other)
        {
            return this.ChangeMutable(false).Equals(other.ChangeMutable(false));
        }

        public static TypeInfo FromASTType(TypeName type)
        {
            TypeInfoGeneratorVisitor visitor = new TypeInfoGeneratorVisitor();
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

            public override string ToString()
            {
                return GetMutStrBefore() + Name;
            }

            public override bool HasNonPointerVoid()
            {
                return Name == Utils.RipplePrimitives.VoidName;
            }

            public override void Walk(Action<TypeInfo> walker)
            {
                walker(this);
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

            public override string ToString()
            {
                if(Contained is FunctionPointer)
                {
                    return "(" + Contained + ")" + GetMutStrAfter() + "*";
                }
                else
                {
                    return Contained + GetMutStrAfter() + "*";
                }
            }

            public override void Walk(Action<TypeInfo> walker)
            {
                walker(this);
                Contained.Walk(walker);
            }
        }

        public class Reference : TypeInfo
        {
            public readonly TypeInfo Contained;
            public readonly Option<LifetimeInfo> Lifetime;

            public Reference(bool isMutable, TypeInfo contained, Option<LifetimeInfo> lifetime) : base(isMutable)
            {
                Contained = contained;
                Lifetime = lifetime;
            }

            public override TypeInfo ChangeMutable(bool isMutable)
            {
                return new Reference(isMutable, Contained, Lifetime);
            }

            public override List<PrimaryTypeInfo> GetPrimaries()
            {
                return Contained.GetPrimaries();
            }

            public override string ToString()
            {
                if (Contained is FunctionPointer)
                {
                    return "(" + Contained + ")" + GetMutStrAfter() + "&";
                }
                else
                {
                    return Contained + GetMutStrAfter() + "&";
                }
            }

            public override bool IsUnsafe() => Contained.IsUnsafe();

            public override bool HasNonPointerVoid()
            {
                return Contained.HasNonPointerVoid();
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Mutable, Contained, Lifetime);
            }

            public override void Walk(Action<TypeInfo> walker)
            {
                walker(this);
                Contained.Walk(walker);
            }

            public override bool Equals(object obj)
            {
                return obj is Reference reference &&
                       Mutable == reference.Mutable &&
                       EqualityComparer<TypeInfo>.Default.Equals(Contained, reference.Contained) &&
                       EqualityComparer<Option<LifetimeInfo>>.Default.Equals(Lifetime, reference.Lifetime);
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

            public override string ToString()
            {
                return GetMutStrBefore() + "(" + Parameters.ToList().ConvertAll(p => p.ToString()).Concat(", ") + 
                    ")->" + Returned;
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

            public override void Walk(Action<TypeInfo> walker)
            {
                walker(this);
                foreach (TypeInfo type in Parameters)
                    type.Walk(walker);

                Returned.Walk(walker);
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

            public override string ToString()
            {
                return Type + GetMutStrAfter() + "[" + Size + "]";
            }

            public override bool HasNonPointerVoid()
            {
                return Type.HasNonPointerVoid();
            }

            public override void Walk(Action<TypeInfo> walker)
            {
                walker(this);
                Type.Walk(walker);
            }
        }
    }
}
