using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class Unary : CExpression
	{
		public readonly CExpression Expression;
		public readonly CUnaryOperator Op;

		public Unary(CExpression expression, CUnaryOperator op)
		{
			this.Expression = expression;
			this.Op = op;
		}

		public override void Accept(ICExpressionVisitor visitor)
		{
			visitor.VisitUnary(this);
		}

		public override T Accept<T>(ICExpressionVisitor<T> visitor)
		{
			return visitor.VisitUnary(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitUnary(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is Unary unary)
			{
				return Expression.Equals(unary.Expression) && Op.Equals(unary.Op);
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
