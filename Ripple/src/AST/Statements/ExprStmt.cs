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
	}
}
