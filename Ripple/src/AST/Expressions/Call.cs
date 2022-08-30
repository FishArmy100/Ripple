using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


namespace Ripple.AST
{
	class Call : Expression
	{

		public readonly Expression Expr;
		public readonly Token OpenParen;
		public readonly List<Expression> Args;
		public readonly Token CloseParen;

		public Call(Expression expr, Token openParen, List<Expression> args, Token closeParen)
		{
			this.Expr = expr;
			this.OpenParen = openParen;
			this.Args = args;
			this.CloseParen = closeParen;
		}

		public override void Accept(IExpressionVisitor visitor)
		{
			visitor.VisitCall(this);
		}
	}
}
