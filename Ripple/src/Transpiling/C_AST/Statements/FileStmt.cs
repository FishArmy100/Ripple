using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class FileStmt : CStatement
	{
		public readonly List<IncludeStmt> Includes;
		public readonly List<CStatement> Statements;
		public readonly CFileType FileType;

		public FileStmt(List<IncludeStmt> includes, List<CStatement> statements, CFileType fileType)
		{
			this.Includes = includes;
			this.Statements = statements;
			this.FileType = fileType;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitFileStmt(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitFileStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitFileStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is FileStmt fileStmt)
			{
				return Includes.Equals(fileStmt.Includes) && Statements.Equals(fileStmt.Statements) && FileType.Equals(fileStmt.FileType);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Includes);
			code.Add(Statements);
			code.Add(FileType);
			return code.ToHashCode();
		}
	}
}
