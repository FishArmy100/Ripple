using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Utils;


namespace Ripple.AST
{
	class WhereClause : Statement
	{
		public readonly Token WhereToken;
		public readonly Expression Expression;

		public WhereClause(Token whereToken, Expression expression)
		{
			this.WhereToken = whereToken;
			this.Expression = expression;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitWhereClause(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitWhereClause(this);
		}

		public override bool Equals(object other)
		{
			if(other is WhereClause whereClause)
			{
				return WhereToken.Equals(whereClause.WhereToken) && Expression.Equals(whereClause.Expression);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(WhereToken);
			code.Add(Expression);
			return code.ToHashCode();
		}
	}
}
