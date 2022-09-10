using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


namespace Ripple.AST
{
	class ForStmt : Statement
	{

		public readonly Token ForTok;
		public readonly Token OpenParen;
		public readonly Statement Init;
		public readonly Expression Condition;
		public readonly Expression Iter;
		public readonly Token CloseParen;
		public readonly Statement Body;

		public ForStmt(Token forTok, Token openParen, Statement init, Expression condition, Expression iter, Token closeParen, Statement body)
		{
			this.ForTok = forTok;
			this.OpenParen = openParen;
			this.Init = init;
			this.Condition = condition;
			this.Iter = iter;
			this.CloseParen = closeParen;
			this.Body = body;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitForStmt(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitForStmt(this);
		}

	}
}
