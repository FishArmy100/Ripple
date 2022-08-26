using System;
using System.Collections.Generic;
using Ripple.Lexing;


namespace Ripple.AST
{
	class IfStmt : Statement
	{

		public readonly Token IfTok;
		public readonly Token OpenParen;
		public readonly Expression Expr;
		public readonly Token CloseParen;
		public readonly Statement Stmt;

		public IfStmt(Token ifTok, Token openParen, Expression expr, Token closeParen, Statement stmt)
		{
			this.IfTok = ifTok;
			this.OpenParen = openParen;
			this.Expr = expr;
			this.CloseParen = closeParen;
			this.Stmt = stmt;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitIfStmt(this);
		}
	}
}
