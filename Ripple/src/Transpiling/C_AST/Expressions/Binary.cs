using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class Binary : CExpression
	{
		public readonly CExpression Left;
		public readonly CBinaryOperator Op;
		public readonly CExpression Right;

		public Binary(CExpression left, CBinaryOperator op, CExpression right)
		{
			this.Left = left;
			this.Op = op;
			this.Right = right;
		}

		public override void Accept(ICExpressionVisitor visitor)
		{
			visitor.VisitBinary(this);
		}

		public override T Accept<T>(ICExpressionVisitor<T> visitor)
		{
			return visitor.VisitBinary(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitBinary(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is Binary binary)
			{
				return Left.Equals(binary.Left) && Op.Equals(binary.Op) && Right.Equals(binary.Right);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Left);
			code.Add(Op);
			code.Add(Right);
			return code.ToHashCode();
		}
	}
}
