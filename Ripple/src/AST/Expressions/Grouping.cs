using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Utils;
using System.Linq;


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

		public override T Accept<T>(IExpressionVisitor<T> visitor)
		{
			return visitor.VisitGrouping(this);
		}

		public override TReturn Accept<TReturn, TArg>(IExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitGrouping(this, arg);
		}

		public override void Accept<TArg>(IExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitGrouping(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is Grouping grouping)
			{
				return LeftParen.Equals(grouping.LeftParen) && Expr.Equals(grouping.Expr) && RightParen.Equals(grouping.RightParen);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(LeftParen);
			code.Add(Expr);
			code.Add(RightParen);
			return code.ToHashCode();
		}
	}
}
