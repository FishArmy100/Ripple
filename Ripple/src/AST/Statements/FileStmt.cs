using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System.Linq;
using System.Linq;


namespace Ripple.AST
{
	class FileStmt : Statement
	{
		public readonly List<Statement> Statements;
		public readonly string RelativePath;
		public readonly Token EOFTok;

		public FileStmt(List<Statement> statements, string relativePath, Token eOFTok)
		{
			this.Statements = statements;
			this.RelativePath = relativePath;
			this.EOFTok = eOFTok;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitFileStmt(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitFileStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitFileStmt(this, arg);
		}

		public override void Accept<TArg>(IStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitFileStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is FileStmt fileStmt)
			{
				return Statements.SequenceEqual(fileStmt.Statements) && RelativePath.Equals(fileStmt.RelativePath) && EOFTok.Equals(fileStmt.EOFTok);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Statements);
			code.Add(RelativePath);
			code.Add(EOFTok);
			return code.ToHashCode();
		}
	}
}
