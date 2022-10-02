using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


namespace Ripple.AST
{
	class BlockStmt : Statement
	{
		public readonly Token OpenBrace;
		public readonly List<Statement> Statements;
		public readonly Token CloseBrace;

		public BlockStmt(Token openBrace, List<Statement> statements, Token closeBrace)
		{
			this.OpenBrace = openBrace;
			this.Statements = statements;
			this.CloseBrace = closeBrace;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitBlockStmt(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitBlockStmt(this);
		}

		public override bool Equals(object other)
		{
			if(other is BlockStmt blockStmt)
			{
				return OpenBrace.Equals(blockStmt.OpenBrace) && Statements.Equals(blockStmt.Statements) && CloseBrace.Equals(blockStmt.CloseBrace);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(OpenBrace, Statements, CloseBrace);
		}
	}
}
