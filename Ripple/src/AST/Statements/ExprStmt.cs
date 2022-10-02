using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


namespace Ripple.AST
{
	class ExprStmt : Statement
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
			return HashCode.Combine(Expr, SemiColin);
		}
	}
}
