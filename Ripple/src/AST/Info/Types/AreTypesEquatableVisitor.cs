using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST.Info.Types
{
    class AreTypesEquatableVisitor : ITypeInfoVisitor<bool, TypeInfo>
    {
        public bool VisitArrayInfo(ArrayInfo arrayInfo, TypeInfo arg)
        {
            if(arg is ArrayInfo a2)
            {
                return arrayInfo.IsMutable == a2.IsMutable && 
                       arrayInfo.Size == a2.Size && 
                       arrayInfo.Contained.Accept(this, a2.Contained);
            }

            return false;
        }

        public bool VisitBasicTypeInfo(BasicTypeInfo basicTypeInfo, TypeInfo arg)
        {
            if(arg is BasicTypeInfo b2)
            {
                return basicTypeInfo.IsMutable == b2.IsMutable && basicTypeInfo.Name == b2.Name;
            }

            return false;
        }

        public bool VisitFuncPtrInfo(FuncPtrInfo funcPtrInfo, TypeInfo arg)
        {
            if(arg is FuncPtrInfo other)
            {
                if (funcPtrInfo.LifetimeCount != other.LifetimeCount)
                    return false;

                if (funcPtrInfo.Parameters.Count != other.Parameters.Count)
                    return false;

                for(int i = 0; i < other.Parameters.Count; i++)
                {
                    TypeInfo a = funcPtrInfo.Parameters[i];
                    TypeInfo b = other.Parameters[i];

                    if (!a.Accept(this, b))
                        return false;
                }

                return funcPtrInfo.Returned.Accept(this, other.Returned);
            }

            return false;
        }

        public bool VisitPointerInfo(PointerInfo pointerInfo, TypeInfo arg)
        {
            if(arg is PointerInfo other)
            {
                return pointerInfo.Contained.Accept(this, other.Contained);
            }

            return false;
        }

        public bool VisitReferenceInfo(ReferenceInfo referenceInfo, TypeInfo arg)
        {
            if(arg is ReferenceInfo other)
            {
                if (referenceInfo.Lifetime.HasValue() && other.Lifetime.HasValue())
                {
                    if (!referenceInfo.Lifetime.Value.Equals(other.Lifetime.Value))
                        return false;
                }

                return referenceInfo.IsMutable == other.IsMutable && referenceInfo.Contained.Accept(this, other.Contained);
            }

            return false;
        }
    }
}
