using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class CArray : CType
	{
		public readonly CType BaseType;
		public readonly Option<int> Size;

		public CArray(CType baseType, Option<int> size)
		{
			this.BaseType = baseType;
			this.Size = size;
		}

		public override void Accept(ICTypeVisitor visitor)
		{
			visitor.VisitCArray(this);
		}

		public override T Accept<T>(ICTypeVisitor<T> visitor)
		{
			return visitor.VisitCArray(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICTypeVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCArray(this, arg);
		}

		public override void Accept<TArg>(ICTypeVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCArray(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CArray cArray)
			{
				return BaseType.Equals(cArray.BaseType) && Size.Equals(cArray.Size);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(BaseType);
			code.Add(Size);
			return code.ToHashCode();
		}
	}
}
