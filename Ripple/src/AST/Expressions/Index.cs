using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Utils;


namespace Ripple.AST
{
	class Index : Expression
	{
		public readonly Expression Indexed;
		public readonly Token OpenBracket;
		public readonly Expression Argument;
		public readonly Token CloseBracket;

		public Index(Expression indexed, Token openBracket, Expression argument, Token closeBracket)
		{
			this.Indexed = indexed;
			this.OpenBracket = openBracket;
			this.Argument = argument;
			this.CloseBracket = closeBracket;
		}

		public override void Accept(IExpressionVisitor visitor)
		{
			visitor.VisitIndex(this);
		}

		public override T Accept<T>(IExpressionVisitor<T> visitor)
		{
			return visitor.VisitIndex(this);
		}

		public override TReturn Accept<TReturn, TArg>(IExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitIndex(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is Index index)
			{
				return Indexed.Equals(index.Indexed) && OpenBracket.Equals(index.OpenBracket) && Argument.Equals(index.Argument) && CloseBracket.Equals(index.CloseBracket);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Indexed);
			code.Add(OpenBracket);
			code.Add(Argument);
			code.Add(CloseBracket);
			return code.ToHashCode();
		}
	}
}
