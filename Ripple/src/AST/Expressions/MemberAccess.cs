using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class MemberAccess : Expression
	{
		public readonly Expression Expression;
		public readonly Token Dot;
		public readonly Token MemberName;

		public MemberAccess(Expression expression, Token dot, Token memberName)
		{
			this.Expression = expression;
			this.Dot = dot;
			this.MemberName = memberName;
		}

		public override void Accept(IExpressionVisitor visitor)
		{
			visitor.VisitMemberAccess(this);
		}

		public override T Accept<T>(IExpressionVisitor<T> visitor)
		{
			return visitor.VisitMemberAccess(this);
		}

		public override TReturn Accept<TReturn, TArg>(IExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitMemberAccess(this, arg);
		}

		public override void Accept<TArg>(IExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitMemberAccess(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is MemberAccess memberAccess)
			{
				return Expression.Equals(memberAccess.Expression) && Dot.Equals(memberAccess.Dot) && MemberName.Equals(memberAccess.MemberName);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Expression);
			code.Add(Dot);
			code.Add(MemberName);
			return code.ToHashCode();
		}
	}
}
