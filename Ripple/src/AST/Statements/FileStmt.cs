using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


namespace Ripple.AST
{
	class FileStmt : Statement
	{
		public readonly List<Statement> Statements;
		public readonly string FilePath;
		public readonly Token EOFTok;

		public FileStmt(List<Statement> statements, string filePath, Token eOFTok)
		{
			this.Statements = statements;
			this.FilePath = filePath;
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

		public override bool Equals(object other)
		{
			if(other is FileStmt fileStmt)
			{
				return Statements.Equals(fileStmt.Statements) && FilePath.Equals(fileStmt.FilePath) && EOFTok.Equals(fileStmt.EOFTok);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Statements);
			code.Add(FilePath);
			code.Add(EOFTok);
			return code.ToHashCode();
		}
	}
}
