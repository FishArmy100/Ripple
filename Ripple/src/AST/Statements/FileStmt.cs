using System;
using System.Collections.Generic;
using Ripple.Lexing;


namespace Ripple.AST
{
	class FileStmt : Statement
	{

		public readonly List<Statement> Statements;
		public readonly Token EOFTok;

		public FileStmt(List<Statement> statements, Token eOFTok)
		{
			this.Statements = statements;
			this.EOFTok = eOFTok;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitFileStmt(this);
		}
	}
}
