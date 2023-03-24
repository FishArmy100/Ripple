using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class CIncludeStmt : CStatement
	{
		public readonly string File;

		public CIncludeStmt(string file)
		{
			this.File = file;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitCIncludeStmt(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitCIncludeStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCIncludeStmt(this, arg);
		}

		public override void Accept<TArg>(ICStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCIncludeStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CIncludeStmt cIncludeStmt)
			{
				return File.Equals(cIncludeStmt.File);
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
