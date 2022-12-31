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

        public abstract TypeInfo ChangeMutable(bool isMutable);
        public abstract bool HasNonPointerVoid();
        public abstract void Walk(Action<TypeInfo> walker);
        public abstract bool EqualsWithoutLifetimes(TypeInfo other);
        public abstract bool IsEquatableToTypeName(TypeName typeName);

        public bool IsUnsafe()
        {
            bool isUnsafe = false;
            this.Walk(t =>
            {
                if (t is Pointer)
                    isUnsafe = true;
            });

            return isUnsafe;
        }

        public List<PrimaryTypeInfo> GetPrimaries()
        {
            List<PrimaryTypeInfo> primaries = new List<PrimaryTypeInfo>();
            this.Walk((t) =>
            {
                if (t is Basic b)
                    primaries.Add(b.NameType);
            });

            return primaries;
        }

        public static TypeInfoCreationResult FromASTType(TypeName typeName, List<PrimaryTypeInfo> primaries, List<Token> lifetimes, Func<ReferenceType, AmbiguousTypeException> errorFunc = null)
        {
            List<ASTInfoError> typeErrors = new TypeNameValidityChecker(typeName, primaries, lifetimes).Errors;

            if (typeErrors.Count > 0)
                return new TypeInfoCreationResult(new Option<TypeInfo>(), new Option<ASTInfoError>(), typeErrors);

            try
            {
                TypeInfo info = new TypeInfoGeneratorVisitor(errorFunc == null ? DefaultErrorGenerator : errorFunc).VisitTypeName(typeName);
                return new TypeInfoCreationResult(info, new Option<ASTInfoError>(), new List<ASTInfoError>());
            }
            catch(AmbiguousTypeException e)
            {
                return new TypeInfoCreationResult(new Option<TypeInfo>(), new ASTInfoError(e.Message, e.ErrorToken), new List<ASTInfoError>());
            }
            catch(TypeOfExpressionExeption e)
            {
                return new TypeInfoCreationResult(new Option<TypeInfo>(), new ASTInfoError(e.Message, e.ErrorToken), new List<ASTInfoError>());
            }
        }

        private static AmbiguousTypeException DefaultErrorGenerator(ReferenceType referenceType)
        {
            return new AmbiguousTypeException("Expected a lifetime.", referenceType.Ampersand);
        }

        public bool EqualsWithoutFirstMutable(TypeInfo other)
        {
            return this.ChangeMutable(false).Equals(other.ChangeMutable(false));
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

            public override bool EqualsWithoutLifetimes(TypeInfo other) => Equals(other);

            public override bool IsEquatableToTypeName(TypeName typeName)
            {
                if(typeName is BasicType basic)
                {
                    return Name == basic.Identifier.Text &&
                           Mutable == basic.MutToken.HasValue;
                }

                return false;
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

            public override bool EqualsWithoutLifetimes(TypeInfo other) => Equals(other);

            public override int GetHashCode()
            {
                return HashCode.Combine(Mutable, Contained, GetType());
            }

            public override bool HasNonPointerVoid()
            {
                if (Contained is Basic)
                    return false;
                else
                    return Contained.HasNonPointerVoid();
            }

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

            public override bool IsEquatableToTypeName(TypeName typeName)
            {
                if (typeName is PointerType pointer)
                {
                    return Contained.IsEquatableToTypeName(pointer.BaseType) &&
                           Mutable == pointer.MutToken.HasValue;
                }

                return false;
            }
        }

        public class Reference : TypeInfo
        {
            public readonly TypeInfo Contained;
            public readonly LifetimeInfo Lifetime;

            public Reference(bool isMutable, TypeInfo contained, LifetimeInfo lifetime) : base(isMutable)
            {
                Contained = contained;
                Lifetime = lifetime;
            }

            public override TypeInfo ChangeMutable(bool isMutable)
            {
                return new Reference(isMutable, Contained, Lifetime);
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
                       EqualityComparer<LifetimeInfo>.Default.Equals(Lifetime, reference.Lifetime);
            }

            public override bool EqualsWithoutLifetimes(TypeInfo other)
            {
                return other is Reference reference &&
                       Mutable == reference.Mutable &&
                       EqualityComparer<TypeInfo>.Default.Equals(Contained, reference.Contained);
            }

            public override bool IsEquatableToTypeName(TypeName typeName)
            {
                if (typeName is ReferenceType reference)
                {
                    bool hasEquatableLifetimes = false;

                    if (!reference.Lifetime.HasValue)
                        hasEquatableLifetimes = true;
                    else if (Lifetime.LifetimeToken is Token t)
                        hasEquatableLifetimes = t.Equals(reference.Lifetime.Value);

                    return Contained.IsEquatableToTypeName(reference.BaseType) &&
                           Mutable == reference.MutToken.HasValue &&
                           hasEquatableLifetimes;
                }

                return false;
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

            public override bool EqualsWithoutLifetimes(TypeInfo other) => Equals(other);

            public override bool IsEquatableToTypeName(TypeName typeName)
            {
                if(typeName is FuncPtr funcPtr)
                {
                    if (funcPtr.Parameters.Count != Parameters.Count)
                        return false;

                    for(int i = 0; i < Parameters.Count; i++)
                    {
                        if (!Parameters[i].IsEquatableToTypeName(typeName))
                            return false;
                    }

                    return Returned.IsEquatableToTypeName(funcPtr.ReturnType) &&
                           Mutable == funcPtr.MutToken.HasValue;
                }

                return false;
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

            public override bool EqualsWithoutLifetimes(TypeInfo other) => Equals(other);

            public override bool IsEquatableToTypeName(TypeName typeName)
            {
                if(typeName is ArrayType array)
                {
                    return Type.IsEquatableToTypeName(array.BaseType) &&
                           SizeToken.Equals(array.Size);
                }

                return false;
            }
        }
    }
}
