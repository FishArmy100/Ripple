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

		public override bool Equals(object other)
		{
			if(other is ForStmt forStmt)
			{
				return ForTok.Equals(forStmt.ForTok) && OpenParen.Equals(forStmt.OpenParen) && Init.Equals(forStmt.Init) && Condition.Equals(forStmt.Condition) && Iter.Equals(forStmt.Iter) && CloseParen.Equals(forStmt.CloseParen) && Body.Equals(forStmt.Body);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(ForTok, OpenParen, Init, Condition, Iter, CloseParen, Body);
		}
	}
}
