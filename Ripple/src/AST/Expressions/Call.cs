using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


namespace Ripple.AST
{
	class Call : Expression
	{
		public readonly Token Identifier;
		public readonly Token OpenParen;
		public readonly List<Expression> Args;
		public readonly Token CloseParen;

		public Call(Token identifier, Token openParen, List<Expression> args, Token closeParen)
		{
			this.Identifier = identifier;
			this.OpenParen = openParen;
			this.Args = args;
			this.CloseParen = closeParen;
		}

		public override void Accept(IExpressionVisitor visitor)
		{
			visitor.VisitCall(this);
		}

		public override T Accept<T>(IExpressionVisitor<T> visitor)
		{
			return visitor.VisitCall(this);
		}

		public override bool Equals(object other)
		{
			if(other is Call call)
			{
				return Identifier.Equals(call.Identifier) && OpenParen.Equals(call.OpenParen) && Args.Equals(call.Args) && CloseParen.Equals(call.CloseParen);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Identifier, OpenParen, Args, CloseParen);
		}
	}
}
