using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST.Utils
{
    public class TypeWalkerBase : ITypeNameVisitor
    {
        public virtual void VisitArrayType(ArrayType arrayType)
        {
            arrayType.BaseType.Accept(this);
        }

        public virtual void VisitBasicType(BasicType basicType) { }

        public virtual void VisitFuncPtr(FuncPtr funcPtr)
        {
            foreach (TypeName parameter in funcPtr.Parameters)
                parameter.Accept(this);
            funcPtr.ReturnType.Accept(this);
        }

        public void VisitGenericType(GenericType genericType) { }

        public virtual void VisitGroupedType(GroupedType groupedType)
        {
            groupedType.Type.Accept(this);
        }

        public virtual void VisitPointerType(PointerType pointerType)
        {
            pointerType.BaseType.Accept(this);
        }

        public virtual void VisitReferenceType(ReferenceType referenceType)
        {
            referenceType.BaseType.Accept(this);
        }
    }
}
