using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


namespace Ripple.AST
{
	class Program : Statement
	{
		public readonly List<FileStmt> Files;

		public Program(List<FileStmt> files)
		{
			this.Files = files;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitProgram(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitProgram(this);
		}

		public override bool Equals(object other)
		{
			if(other is Program program)
			{
				return Files.Equals(program.Files);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Files);
		}
	}
}
