using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Core;
using Ripple.Lexing;

namespace Ripple.AST.Utils
{
    public static class TypeNameExtensions
    {
        public static SourceLocation GetLocation(this TypeName type)
        {
            return type.Accept(new LocationFinderVisitor()).Sum();
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
