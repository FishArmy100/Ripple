using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils.Extensions;
using Ripple.Utils;

namespace Ripple.AST.Info.Types
{
    class TypeInfoPrinterVisitor : ITypeInfoVisitor<string>
    {
        public string VisitArrayInfo(ArrayInfo arrayInfo)
        {
            return arrayInfo.Contained.Accept(this) + AppendMutablePostfix(arrayInfo.IsMutable, "[" + arrayInfo.Size + "]");
        }

        public string VisitBasicTypeInfo(BasicTypeInfo basicTypeInfo)
        {
            return AppendMutablePrefix(basicTypeInfo.IsMutable, basicTypeInfo.Name);
        }

        public string VisitFuncPtrInfo(FuncPtrInfo funcPtrInfo)
        {
            string signature = 
                "func(" + 
                funcPtrInfo.Parameters.ConvertAll(p => p.Accept(this)).Concat(", ") + 
                ") -> " + 
                funcPtrInfo.Returned.Accept(this);

            return AppendMutablePrefix(funcPtrInfo.IsMutable, signature);
        }

        public string VisitPointerInfo(PointerInfo pointerInfo)
        {
            return pointerInfo.Contained.Accept(this) + AppendMutablePostfix(pointerInfo.IsMutable, "*");
        }

        public string VisitReferenceInfo(ReferenceInfo referenceInfo)
        {
            string contained = referenceInfo.Contained.Accept(this);
            string lifetime = GetLifetimeString(referenceInfo.Lifetime);
            return contained + AppendMutablePostfix(referenceInfo.IsMutable, "&") + lifetime;
        }

        private string AppendMutablePostfix(bool isMutable, string subject)
        {
            return isMutable ? " mut" + subject : subject;
        }

        private string AppendMutablePrefix(bool isMutable, string subject)
        {
            return isMutable ? "mut " + subject : subject;
        }

        private string GetLifetimeString(Option<ReferenceLifetime> lifetime)
        {
            return lifetime.Match(ok => ok.ToString(), () => "");
        }
    }
}
