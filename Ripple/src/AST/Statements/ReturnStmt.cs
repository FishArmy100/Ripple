using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


namespace Ripple.AST
{
	class ReturnStmt : Statement
	{

		public readonly Token ReturnTok;
		public readonly Expression Expr;
		public readonly Token SemiColin;

		public ReturnStmt(Token returnTok, Expression expr, Token semiColin)
		{
			this.ReturnTok = returnTok;
			this.Expr = expr;
			this.SemiColin = semiColin;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitReturnStmt(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitReturnStmt(this);
		}

	}
}
