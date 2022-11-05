using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


namespace Ripple.AST
{
	class WhileStmt : Statement
	{
		public readonly Token WhileToken;
		public readonly Token OpenParen;
		public readonly Expression Condition;
		public readonly Token CloseParen;
		public readonly Statement Body;

		public WhileStmt(Token whileToken, Token openParen, Expression condition, Token closeParen, Statement body)
		{
			this.WhileToken = whileToken;
			this.OpenParen = openParen;
			this.Condition = condition;
			this.CloseParen = closeParen;
			this.Body = body;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitWhileStmt(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitWhileStmt(this);
		}

		public override bool Equals(object other)
		{
			if(other is WhileStmt whileStmt)
			{
				return WhileToken.Equals(whileStmt.WhileToken) && OpenParen.Equals(whileStmt.OpenParen) && Condition.Equals(whileStmt.Condition) && CloseParen.Equals(whileStmt.CloseParen) && Body.Equals(whileStmt.Body);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(WhileToken, OpenParen, Condition, CloseParen, Body);
		}
	}
}