using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils.Extensions;

namespace Ripple.AST
{
    static class TypeNamePrinter
    {

        public static string PrintType(TypeName typeName)
        {
            TypeNamePrinterVisitor visitor = new TypeNamePrinterVisitor();
            return typeName.Accept(visitor);
        }

        private class TypeNamePrinterVisitor : ITypeNameVisitor<string>
        {
            public string VisitFuncPtr(FuncPtr funcPtr)
            {
                string str = "(";

                str += funcPtr.Parameters
                        .ConvertAll(p => p.Accept(this))
                        .Concat(", ");

                str += ") -> " + funcPtr.ReturnType.Accept(this);
                return str;
            }

            public string VisitGroupedType(GroupedType groupedType)
            {
                return "(" + groupedType.GroupedType.Accept(this) + ")";
            }

            public string VisitPointerType(PointerType pointerType)
            {
                return pointerType.BaseType.Accept(this) + "*";
            }

            public string VisitReferenceType(ReferenceType referenceType)
            {
                return referenceType.BaseType.Accept(this) + "&";
            }
        }

    }
}
