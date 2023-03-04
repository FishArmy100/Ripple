using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class CIfStmt : CStatement
	{
		public readonly CExpression Condition;
		public readonly CStatement Body;
		public readonly Option<CStatement> ElseBody;

		public CIfStmt(CExpression condition, CStatement body, Option<CStatement> elseBody)
		{
			this.Condition = condition;
			this.Body = body;
			this.ElseBody = elseBody;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitCIfStmt(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitCIfStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCIfStmt(this, arg);
		}

		public override void Accept<TArg>(ICStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCIfStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CIfStmt cIfStmt)
			{
				return Condition.Equals(cIfStmt.Condition) && Body.Equals(cIfStmt.Body) && ElseBody.Equals(cIfStmt.ElseBody);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Condition);
			code.Add(Body);
			code.Add(ElseBody);
			return code.ToHashCode();
		}
	}
}
