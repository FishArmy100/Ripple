using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class ExprStmt : Statement
	{
		public readonly Expression Expr;
		public readonly Token SemiColin;

		public ExprStmt(Expression expr, Token semiColin)
		{
			this.Expr = expr;
			this.SemiColin = semiColin;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitExprStmt(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitExprStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitExprStmt(this, arg);
		}

		public override void Accept<TArg>(IStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitExprStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is ExprStmt exprStmt)
			{
				return Expr.Equals(exprStmt.Expr) && SemiColin.Equals(exprStmt.SemiColin);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Expr);
			code.Add(SemiColin);
			return code.ToHashCode();
		}
	}
}
