using System.Collections.Generic;
using Raucse;
using System;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	public class CBinary : CExpression
	{
		public readonly CExpression Left;
		public readonly CBinaryOperator Op;
		public readonly CExpression Right;

		public CBinary(CExpression left, CBinaryOperator op, CExpression right)
		{
			this.Left = left;
			this.Op = op;
			this.Right = right;
		}

		public override void Accept(ICExpressionVisitor visitor)
		{
			visitor.VisitCBinary(this);
		}

		public override T Accept<T>(ICExpressionVisitor<T> visitor)
		{
			return visitor.VisitCBinary(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCBinary(this, arg);
		}

		public override void Accept<TArg>(ICExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCBinary(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CBinary cBinary)
			{
				return Left.Equals(cBinary.Left) && Op.Equals(cBinary.Op) && Right.Equals(cBinary.Right);
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
