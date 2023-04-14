using System.Collections.Generic;
using Raucse;
using System;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	public class CPointer : CType
	{
		public readonly CType BaseType;
		public readonly bool IsConst;

		public CPointer(CType baseType, bool isConst)
		{
			this.BaseType = baseType;
			this.IsConst = isConst;
		}

		public override void Accept(ICTypeVisitor visitor)
		{
			visitor.VisitCPointer(this);
		}

		public override T Accept<T>(ICTypeVisitor<T> visitor)
		{
			return visitor.VisitCPointer(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICTypeVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCPointer(this, arg);
		}

		public override void Accept<TArg>(ICTypeVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCPointer(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CPointer cPointer)
			{
				return BaseType.Equals(cPointer.BaseType) && IsConst.Equals(cPointer.IsConst);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(BaseType);
			code.Add(IsConst);
			return code.ToHashCode();
		}
	}
}
