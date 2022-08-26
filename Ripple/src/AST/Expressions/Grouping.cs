using System;
using System.Collections.Generic;
using Ripple.Lexing;


namespace Ripple.AST
{
	class Grouping : Expression
	{

		public readonly Token LeftParen;
		public readonly Expression Expr;
		public readonly Token RightParen;

		public Grouping(Token leftParen, Expression expr, Token rightParen)
		{
			this.LeftParen = leftParen;
			this.Expr = expr;
			this.RightParen = rightParen;
		}

		public override void Accept(IExpressionVisitor visitor)
		{
			visitor.VisitGrouping(this);
		}
	}
}
