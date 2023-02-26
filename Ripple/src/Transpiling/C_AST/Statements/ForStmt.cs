using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class ForStmt : CStatement
	{
		public readonly VarDecl Initalizer;
		public readonly CExpression Condition;
		public readonly CExpression Iterator;

		public ForStmt(VarDecl initalizer, CExpression condition, CExpression iterator)
		{
			this.Initalizer = initalizer;
			this.Condition = condition;
			this.Iterator = iterator;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitForStmt(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitForStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitForStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is ForStmt forStmt)
			{
				return Initalizer.Equals(forStmt.Initalizer) && Condition.Equals(forStmt.Condition) && Iterator.Equals(forStmt.Iterator);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Initalizer);
			code.Add(Condition);
			code.Add(Iterator);
			return code.ToHashCode();
		}
	}
}
