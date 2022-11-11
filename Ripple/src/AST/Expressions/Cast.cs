using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


namespace Ripple.AST
{
	class Cast : Expression
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
