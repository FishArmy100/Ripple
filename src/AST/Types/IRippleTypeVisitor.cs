using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    interface IRippleTypeVisitor
    {
        public void VisitBasicType(BasicType basicType);
        public void VisitArrayType(ArrayType arrayType);
        public void VisitFuncRefType(FuncRefType funcRef);
    }

    interface IRippleTypeVisitor<T>
    {
        public T VisitBasicType(BasicType basicType);
        public T VisitArrayType(ArrayType arrayType);
        public T VisitFuncRefType(FuncRefType funcRef);
    }
}
