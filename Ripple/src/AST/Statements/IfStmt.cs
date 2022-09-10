using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


namespace Ripple.AST
{
	class IfStmt : Statement
	{

		public readonly Token IfTok;
		public readonly Token OpenParen;
		public readonly Expression Expr;
		public readonly Token CloseParen;
		public readonly Statement Body;

		public IfStmt(Token ifTok, Token openParen, Expression expr, Token closeParen, Statement body)
		{
			this.IfTok = ifTok;
			this.OpenParen = openParen;
			this.Expr = expr;
			this.CloseParen = closeParen;
			this.Body = body;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitIfStmt(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitIfStmt(this);
		}

	}
}
