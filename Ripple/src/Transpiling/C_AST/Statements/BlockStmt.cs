using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class BlockStmt : CStatement
	{
		public readonly List<CStatement> Statements;

		public BlockStmt(List<CStatement> statements)
		{
			this.Statements = statements;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitBlockStmt(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitBlockStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitBlockStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is BlockStmt blockStmt)
			{
				return Statements.Equals(blockStmt.Statements);
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
