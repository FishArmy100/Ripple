using System;
using System.Collections.Generic;
using Raucse;
using System.Linq;
using System.Linq;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	class CFileStmt : CStatement
	{
		public readonly List<CIncludeStmt> Includes;
		public readonly List<CStatement> Statements;
		public readonly string RelativePath;
		public readonly CFileType FileType;

		public CFileStmt(List<CIncludeStmt> includes, List<CStatement> statements, string relativePath, CFileType fileType)
		{
			this.Includes = includes;
			this.Statements = statements;
			this.RelativePath = relativePath;
			this.FileType = fileType;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitCFileStmt(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitCFileStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCFileStmt(this, arg);
		}

		public override void Accept<TArg>(ICStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCFileStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CFileStmt cFileStmt)
			{
				return Includes.SequenceEqual(cFileStmt.Includes) && Statements.SequenceEqual(cFileStmt.Statements) && RelativePath.Equals(cFileStmt.RelativePath) && FileType.Equals(cFileStmt.FileType);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Includes);
			code.Add(Statements);
			code.Add(RelativePath);
			code.Add(FileType);
			return code.ToHashCode();
		}
	}
}
