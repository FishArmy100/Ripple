using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Utils;


namespace Ripple.AST
{
	class Unary : Expression
	{
		public readonly Token Op;
		public readonly Expression Expr;

		public Unary(Token op, Expression expr)
		{
			this.Op = op;
			this.Expr = expr;
		}

		public override void Accept(IExpressionVisitor visitor)
		{
			visitor.VisitUnary(this);
		}

		public override T Accept<T>(IExpressionVisitor<T> visitor)
		{
			return visitor.VisitUnary(this);
		}

		public override TReturn Accept<TReturn, TArg>(IExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitUnary(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is Unary unary)
			{
				return Op.Equals(unary.Op) && Expr.Equals(unary.Expr);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Op);
			code.Add(Expr);
			return code.ToHashCode();
		}
	}
}
