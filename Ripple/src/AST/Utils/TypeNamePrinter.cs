using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raucse.Extensions;
using Ripple.Lexing;

namespace Ripple.AST.Utils
{
    public static class TypeNamePrinter
    {
        public static string PrintType(TypeName typeName)
        {
            TypeNamePrinterVisitor visitor = new TypeNamePrinterVisitor();
            return typeName.Accept(visitor);
        }

        private class TypeNamePrinterVisitor : ITypeNameVisitor<string>
        {
            public string VisitArrayType(ArrayType arrayType)
            {
                string str = arrayType.BaseType.Accept(this);
                if (arrayType.MutToken is Token t)
                    str += " " + t.Text;
                str += "[" + arrayType.Size.Text + "]";
                return str;
            }

            public string VisitBasicType(BasicType basicType)
            {
                string str = "";
                if (basicType.MutToken is Token)
                    str += "mut ";
                return str += basicType.Identifier.Text;
            }

            public string VisitFuncPtr(FuncPtr funcPtr)
            {
                string str = "func";

                funcPtr.Lifetimes.Match(ok =>
                {
                    str += "<" + ok.ConvertAll(t => t.Text).Concat(", ") + ">";
                });

                str += "(";

                str += funcPtr.Parameters
                        .ConvertAll(p => p.Accept(this))
                        .Concat(", ");

                str += ") -> " + funcPtr.ReturnType.Accept(this);
                return str;
            }

            public string VisitGroupedType(GroupedType groupedType)
            {
                return "(" + groupedType.Type.Accept(this) + ")";
            }

            public string VisitPointerType(PointerType pointerType)
            {
                string str = pointerType.BaseType.Accept(this);
                if (pointerType.MutToken is Token t)
                    str += " " + t.Text;

                str += "*";
                return str;
            }

            public string VisitReferenceType(ReferenceType referenceType)
            {
                string str = referenceType.BaseType.Accept(this);
                if (referenceType.MutToken is Token t)
                    str += " " + t.Text;

                str += "&";
                if (referenceType.Lifetime is Token lifetime)
                    str += lifetime.Text;

                return str;
            }
        }

    }
}
