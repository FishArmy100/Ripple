using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class CMemberAccess : CExpression
	{
		public readonly CExpression Expression;
		public readonly string Identifier;

		public CMemberAccess(CExpression expression, string identifier)
		{
			this.Expression = expression;
			this.Identifier = identifier;
		}

		public override void Accept(ICExpressionVisitor visitor)
		{
			visitor.VisitCMemberAccess(this);
		}

		public override T Accept<T>(ICExpressionVisitor<T> visitor)
		{
			return visitor.VisitCMemberAccess(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCMemberAccess(this, arg);
		}

		public override void Accept<TArg>(ICExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCMemberAccess(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CMemberAccess cMemberAccess)
			{
				return Expression.Equals(cMemberAccess.Expression) && Identifier.Equals(cMemberAccess.Identifier);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Expression);
			code.Add(Identifier);
			return code.ToHashCode();
		}
	}
}
