using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class SizeOf : CExpression
	{
		public readonly CType Type;

		public SizeOf(CType type)
		{
			this.Type = type;
		}

		public override void Accept(ICExpressionVisitor visitor)
		{
			visitor.VisitSizeOf(this);
		}

		public override T Accept<T>(ICExpressionVisitor<T> visitor)
		{
			return visitor.VisitSizeOf(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitSizeOf(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is SizeOf sizeOf)
			{
				return Type.Equals(sizeOf.Type);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Type);
			return code.ToHashCode();
		}
	}
}
