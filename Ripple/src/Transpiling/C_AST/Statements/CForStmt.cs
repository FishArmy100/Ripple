using System.Collections.Generic;
using Raucse;
using System;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	public class CForStmt : CStatement
	{
		public readonly Option<CVarDecl> Initalizer;
		public readonly Option<CExpression> Condition;
		public readonly Option<CExpression> Iterator;
		public readonly CStatement Body;

		public CForStmt(Option<CVarDecl> initalizer, Option<CExpression> condition, Option<CExpression> iterator, CStatement body)
		{
			this.Initalizer = initalizer;
			this.Condition = condition;
			this.Iterator = iterator;
			this.Body = body;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitCForStmt(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitCForStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCForStmt(this, arg);
		}

		public override void Accept<TArg>(ICStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCForStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CForStmt cForStmt)
			{
				return Initalizer.Equals(cForStmt.Initalizer) && Condition.Equals(cForStmt.Condition) && Iterator.Equals(cForStmt.Iterator) && Body.Equals(cForStmt.Body);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Initalizer);
			code.Add(Condition);
			code.Add(Iterator);
			code.Add(Body);
			return code.ToHashCode();
		}
	}
}
