using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class Pointer : CType
	{
		public readonly CType BaseType;
		public readonly bool IsConst;

		public Pointer(CType baseType, bool isConst)
		{
			this.BaseType = baseType;
			this.IsConst = isConst;
		}

		public override void Accept(ICTypeVisitor visitor)
		{
			visitor.VisitPointer(this);
		}

		public override T Accept<T>(ICTypeVisitor<T> visitor)
		{
			return visitor.VisitPointer(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICTypeVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitPointer(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is Pointer pointer)
			{
				return BaseType.Equals(pointer.BaseType) && IsConst.Equals(pointer.IsConst);
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
