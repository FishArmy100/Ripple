using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class Cast : Expression
	{
		public readonly Expression Castee;
		public readonly Token AsToken;
		public readonly TypeName TypeToCastTo;

		public Cast(Expression castee, Token asToken, TypeName typeToCastTo)
		{
			this.Castee = castee;
			this.AsToken = asToken;
			this.TypeToCastTo = typeToCastTo;
		}

		public override void Accept(IExpressionVisitor visitor)
		{
			visitor.VisitCast(this);
		}

		public override T Accept<T>(IExpressionVisitor<T> visitor)
		{
			return visitor.VisitCast(this);
		}

		public override TReturn Accept<TReturn, TArg>(IExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCast(this, arg);
		}

		public override void Accept<TArg>(IExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCast(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is Cast cast)
			{
				return Castee.Equals(cast.Castee) && AsToken.Equals(cast.AsToken) && TypeToCastTo.Equals(cast.TypeToCastTo);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Castee);
			code.Add(AsToken);
			code.Add(TypeToCastTo);
			return code.ToHashCode();
		}
	}
}
