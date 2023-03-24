using System;
using System.Collections.Generic;
using Ripple.Utils;
using Ripple.Validation;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;
using Ripple.Lexing;


namespace Ripple.Validation.Info.Types
{
	class FuncPtrInfo : TypeInfo
	{
		public readonly bool IsMutable;
		public readonly int FunctionIndex;
		public readonly int LifetimeCount;
		public readonly List<TypeInfo> Parameters;
		public readonly TypeInfo Returned;

		public FuncPtrInfo(bool isMutable, int functionIndex, int lifetimeCount, List<TypeInfo> parameters, TypeInfo returned)
		{
			this.IsMutable = isMutable;
			this.FunctionIndex = functionIndex;
			this.LifetimeCount = lifetimeCount;
			this.Parameters = parameters;
			this.Returned = returned;
		}

		public override void Accept(ITypeInfoVisitor visitor)
		{
			visitor.VisitFuncPtrInfo(this);
		}

		public override T Accept<T>(ITypeInfoVisitor<T> visitor)
		{
			return visitor.VisitFuncPtrInfo(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypeInfoVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitFuncPtrInfo(this, arg);
		}

		public override void Accept<TArg>(ITypeInfoVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitFuncPtrInfo(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is FuncPtrInfo funcPtrInfo)
			{
				return IsMutable.Equals(funcPtrInfo.IsMutable) && FunctionIndex.Equals(funcPtrInfo.FunctionIndex) && LifetimeCount.Equals(funcPtrInfo.LifetimeCount) && Parameters.Equals(funcPtrInfo.Parameters) && Returned.Equals(funcPtrInfo.Returned);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(IsMutable);
			code.Add(FunctionIndex);
			code.Add(LifetimeCount);
			code.Add(Parameters);
			code.Add(Returned);
			return code.ToHashCode();
		}
	}
}
