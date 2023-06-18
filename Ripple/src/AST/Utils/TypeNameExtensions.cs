using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Core;
using Ripple.Lexing;
using Raucse.Extensions.Nullables;

namespace Ripple.AST.Utils
{
    public static class TypeNameExtensions
    {
        public static SourceLocation GetLocation(this TypeName type)
        {
            return type.Accept(new LocationFinderVisitor()).Sum();
        }

        private static List<Token> GetAllLifetimesInternal(this TypeName type)
        {
            if (type is BasicType)
            {
                return new List<Token>();
            }
            else if (type is ReferenceType r)
            {
                var lifetimes = r.BaseType.GetAllLifetimesInternal();
                r.Lifetime.Match(ok => lifetimes.Add(ok));
                return lifetimes;
            }
            else if (type is PointerType p)
            {
                return p.BaseType.GetAllLifetimesInternal();
            }
            else if (type is ArrayType a)
            {
                return a.BaseType.GetAllLifetimesInternal();
            }
            else if (type is GroupedType g)
            {
                return g.Type.GetAllLifetimesInternal();
            }
            else if (type is FuncPtr fp)
            {

            }

            throw new NotImplementedException();
        }

        private class LocationFinderVisitor : ITypeNameVisitor<List<SourceLocation>>
        {
            public List<SourceLocation> VisitArrayType(ArrayType arrayType)
            {
                List<SourceLocation> locations = arrayType.BaseType.Accept(this);
                locations.Add(arrayType.OpenBracket.Location);
                locations.Add(arrayType.Size.Location);
                locations.Add(arrayType.CloseBracket.Location);
                return locations;
            }

            public List<SourceLocation> VisitBasicType(BasicType basicType)
            {
                return new List<SourceLocation> { basicType.Identifier.Location };
            }

            public List<SourceLocation> VisitFuncPtr(FuncPtr funcPtr)
            {
                List<SourceLocation> locations = new List<SourceLocation>();
                locations.Add(funcPtr.FuncToken.Location);
                locations.AddRange(funcPtr.ReturnType.Accept(this));
                return locations;
            }

            public List<SourceLocation> VisitGenericType(GenericType genericType)
            {
                return new List<SourceLocation>
                {
                    genericType.Identifier.Location,
                    genericType.GreaterThan.Location
                };
            }

            public List<SourceLocation> VisitGroupedType(GroupedType groupedType)
            {
                return new List<SourceLocation>
                {
                    groupedType.OpenParen.Location,
                    groupedType.CloseParen.Location
                };
            }

            public List<SourceLocation> VisitPointerType(PointerType pointerType)
            {
                List<SourceLocation> locations = pointerType.BaseType.Accept(this);
                if (pointerType.MutToken is Token t)
                    locations.Add(t.Location);
                locations.Add(pointerType.Star.Location);
                return locations;
            }

            public List<SourceLocation> VisitReferenceType(ReferenceType referenceType)
            {
                List<SourceLocation> locations = referenceType.BaseType.Accept(this);
                if (referenceType.MutToken is Token t)
                    locations.Add(t.Location);
                locations.Add(referenceType.Ampersand.Location);
                return locations;
            }
        }
    }
}
