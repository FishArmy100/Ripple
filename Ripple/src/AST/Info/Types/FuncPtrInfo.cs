using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Utils;


namespace Ripple.AST.Info.Types
{
	class FuncPtrInfo : TypeInfo
	{
		public readonly bool IsMutable;
		public readonly List<LifetimeInfo> Lifetimes;
		public readonly List<TypeInfo> Parameters;
		public readonly TypeInfo Returned;

		public FuncPtrInfo(bool isMutable, List<LifetimeInfo> lifetimes, List<TypeInfo> parameters, TypeInfo returned)
		{
			this.IsMutable = isMutable;
			this.Lifetimes = lifetimes;
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

		public override bool Equals(object other)
		{
			if(other is FuncPtrInfo funcPtrInfo)
			{
				return IsMutable.Equals(funcPtrInfo.IsMutable) && Lifetimes.Equals(funcPtrInfo.Lifetimes) && Parameters.Equals(funcPtrInfo.Parameters) && Returned.Equals(funcPtrInfo.Returned);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(IsMutable);
			code.Add(Lifetimes);
			code.Add(Parameters);
			code.Add(Returned);
			return code.ToHashCode();
		}
	}
}
