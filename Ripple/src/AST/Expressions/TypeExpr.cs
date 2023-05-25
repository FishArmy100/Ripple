using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class TypeExpr : Expression
	{
		public readonly TypeName Type;

		public TypeExpr(TypeName type)
		{
			this.Type = type;
		}

		public override void Accept(IExpressionVisitor visitor)
		{
			visitor.VisitTypeExpr(this);
		}

		public override T Accept<T>(IExpressionVisitor<T> visitor)
		{
			return visitor.VisitTypeExpr(this);
		}

		public override TReturn Accept<TReturn, TArg>(IExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypeExpr(this, arg);
		}

		public override void Accept<TArg>(IExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypeExpr(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypeExpr typeExpr)
			{
				return Type.Equals(typeExpr.Type);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Type);
			return code.ToHashCode();
		}
	}
}
