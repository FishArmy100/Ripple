using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST.Utils;
using Ripple.Utils;
using Ripple.Lexing;

namespace Ripple.AST.Info
{
    class TypeInfoGeneratorVisitor : ITypeNameVisitor<TypeInfo>
    {
        public TypeInfo VisitTypeName(TypeName type)
        {
            return type.Accept(this);
        }

        public TypeInfo VisitArrayType(ArrayType arrayType)
        {
            TypeInfo baseType = arrayType.BaseType.Accept(this);
            bool mutable = arrayType.MutToken.HasValue;
            return new TypeInfo.Array(mutable, baseType, arrayType.Size);
        }

        public TypeInfo VisitBasicType(BasicType basicType)
        {
            bool mutable = basicType.MutToken.HasValue;
            return new TypeInfo.Basic(mutable, new PrimaryTypeInfo(basicType.Identifier));
        }

        public TypeInfo VisitFuncPtr(FuncPtr funcPtr)
        {
            bool mutable = funcPtr.MutToken.HasValue;
            List<TypeInfo> parameters = funcPtr.Parameters.ConvertAll(p => p.Accept(this));
            TypeInfo returned = funcPtr.ReturnType.Accept(this);
            return new TypeInfo.FunctionPointer(mutable, parameters, returned);
        }

        public TypeInfo VisitGroupedType(GroupedType groupedType)
        {
            return groupedType.Type.Accept(this);
        }

        public TypeInfo VisitPointerType(PointerType pointerType)
        {
            TypeInfo baseType = pointerType.BaseType.Accept(this);
            bool mutable = pointerType.MutToken.HasValue;
            return new TypeInfo.Pointer(mutable, baseType);
        }

        public TypeInfo VisitReferenceType(ReferenceType referenceType)
        {
            TypeInfo baseType = referenceType.BaseType.Accept(this);
            bool mutable = referenceType.MutToken.HasValue;
            Option<Token> lifetime = referenceType.Lifetime.HasValue ? referenceType.Lifetime.Value : new Option<Token>();
            
            return new TypeInfo.Reference(mutable, baseType, lifetime);
        }
    }
}
