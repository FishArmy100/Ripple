using System;
using System.Collections.Generic;
using Ripple.Lexing;


namespace Ripple.AST
{
	class ForStmt : Statement
	{

		public readonly Token ForTok;
		public readonly Token OpenParen;
		public readonly Statement Init;
		public readonly Expression Condition;
		public readonly Expression Iter;

		public ForStmt(Token forTok, Token openParen, Statement init, Expression condition, Expression iter)
		{
			this.ForTok = forTok;
			this.OpenParen = openParen;
			this.Init = init;
			this.Condition = condition;
			this.Iter = iter;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitForStmt(this);
		}
	}
}
