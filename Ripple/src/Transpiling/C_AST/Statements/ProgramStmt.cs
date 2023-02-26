using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class ProgramStmt : CStatement
	{
		public readonly List<FileStmt> Files;

		public ProgramStmt(List<FileStmt> files)
		{
			this.Files = files;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitProgramStmt(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitProgramStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitProgramStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is ProgramStmt programStmt)
			{
				return Files.Equals(programStmt.Files);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Files);
			return code.ToHashCode();
		}
	}
}
