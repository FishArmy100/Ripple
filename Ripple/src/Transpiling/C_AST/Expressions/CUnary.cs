using System.Collections.Generic;
using Raucse;
using System;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	public class CUnary : CExpression
	{
		public readonly CExpression Expression;
		public readonly CUnaryOperator Op;

		public CUnary(CExpression expression, CUnaryOperator op)
		{
			this.Expression = expression;
			this.Op = op;
		}

		public override void Accept(ICExpressionVisitor visitor)
		{
			visitor.VisitCUnary(this);
		}

		public override T Accept<T>(ICExpressionVisitor<T> visitor)
		{
			return visitor.VisitCUnary(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCUnary(this, arg);
		}

		public override void Accept<TArg>(ICExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCUnary(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CUnary cUnary)
			{
				return Expression.Equals(cUnary.Expression) && Op.Equals(cUnary.Op);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Expression);
			code.Add(Op);
			return code.ToHashCode();
		}
	}
}
