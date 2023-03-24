using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Utils;
using System.Linq;
using System.Linq;


namespace Ripple.AST
{
	class ProgramStmt : Statement
	{
		public readonly List<FileStmt> Files;

		public ProgramStmt(List<FileStmt> files)
		{
			this.Files = files;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitProgramStmt(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitProgramStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitProgramStmt(this, arg);
		}

		public override void Accept<TArg>(IStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitProgramStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is ProgramStmt programStmt)
			{
				return Files.SequenceEqual(programStmt.Files);
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
