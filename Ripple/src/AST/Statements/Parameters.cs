using System;
using System.Collections.Generic;
using Ripple.Lexing;


namespace Ripple.AST
{
	class Parameters : Statement
	{

		public readonly Token OpenParen;
		public readonly List<(Token,Token)> ParamList;
		public readonly Token CloseParen;

		public Parameters(Token openParen, List<(Token,Token)> paramList, Token closeParen)
		{
			this.OpenParen = openParen;
			this.ParamList = paramList;
			this.CloseParen = closeParen;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitParameters(this);
		}
	}
}
