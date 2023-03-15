using System;
using System.Collections.Generic;
using Ripple.Utils;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;


namespace Ripple.Validation.Info.Statements
{
	class TypedBlockStmt : TypedStatement
	{
		public readonly List<TypedStatement> Statements;

		public TypedBlockStmt(List<TypedStatement> statements)
		{
			this.Statements = statements;
		}

		public override void Accept(ITypedStatementVisitor visitor)
		{
			visitor.VisitTypedBlockStmt(this);
		}

		public override T Accept<T>(ITypedStatementVisitor<T> visitor)
		{
			return visitor.VisitTypedBlockStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypedStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypedBlockStmt(this, arg);
		}

		public override void Accept<TArg>(ITypedStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypedBlockStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypedBlockStmt typedBlockStmt)
			{
				return Statements.Equals(typedBlockStmt.Statements);
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
