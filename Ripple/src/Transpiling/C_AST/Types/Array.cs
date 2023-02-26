using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class Array : CType
	{
		public readonly CType BaseType;
		public readonly Option<int> Size;

		public Array(CType baseType, Option<int> size)
		{
			this.BaseType = baseType;
			this.Size = size;
		}

		public override void Accept(ICTypeVisitor visitor)
		{
			visitor.VisitArray(this);
		}

		public override T Accept<T>(ICTypeVisitor<T> visitor)
		{
			return visitor.VisitArray(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICTypeVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitArray(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is Array array)
			{
				return BaseType.Equals(array.BaseType) && Size.Equals(array.Size);
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
