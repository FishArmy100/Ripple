using System.Collections.Generic;
using Raucse;
using System;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	public class CBlockStmt : CStatement
	{
		public readonly List<CStatement> Statements;

		public CBlockStmt(List<CStatement> statements)
		{
			this.Statements = statements;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitCBlockStmt(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitCBlockStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCBlockStmt(this, arg);
		}

		public override void Accept<TArg>(ICStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCBlockStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CBlockStmt cBlockStmt)
			{
				return Statements.SequenceEqual(cBlockStmt.Statements);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Statements);
			return code.ToHashCode();
		}
	}
}
