using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


namespace Ripple.AST
{
	class Parameters : Statement
	{
		public readonly Token OpenParen;
		public readonly List<(TypeName,Token)> ParamList;
		public readonly Token CloseParen;

		public Parameters(Token openParen, List<(TypeName,Token)> paramList, Token closeParen)
		{
			this.OpenParen = openParen;
			this.ParamList = paramList;
			this.CloseParen = closeParen;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitParameters(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitParameters(this);
		}

		public override bool Equals(object other)
		{
			if(other is Parameters parameters)
			{
				return OpenParen.Equals(parameters.OpenParen) && ParamList.Equals(parameters.ParamList) && CloseParen.Equals(parameters.CloseParen);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(OpenParen, ParamList, CloseParen);
		}
	}
}
