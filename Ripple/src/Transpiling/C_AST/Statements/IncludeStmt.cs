using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class IncludeStmt : CStatement
	{
		public readonly string File;

		public IncludeStmt(string file)
		{
			this.File = file;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitIncludeStmt(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitIncludeStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitIncludeStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is IncludeStmt includeStmt)
			{
				return File.Equals(includeStmt.File);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(File);
			return code.ToHashCode();
		}
	}
}
