using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class BlockStmt : Statement
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

		public override TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitBlockStmt(this, arg);
		}

		public override void Accept<TArg>(IStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitBlockStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is BlockStmt blockStmt)
			{
				return OpenBrace.Equals(blockStmt.OpenBrace) && Statements.SequenceEqual(blockStmt.Statements) && CloseBrace.Equals(blockStmt.CloseBrace);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(OpenBrace);
			code.Add(Statements);
			code.Add(CloseBrace);
			return code.ToHashCode();
		}
	}
}
