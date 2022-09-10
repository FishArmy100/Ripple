using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


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

	}
}
