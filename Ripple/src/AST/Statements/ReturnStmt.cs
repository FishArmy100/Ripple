using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Utils;
using System.Linq;
using System.Linq;


namespace Ripple.AST
{
	class ReturnStmt : Statement
	{
		public readonly Token ReturnTok;
		public readonly Option<Expression> Expr;
		public readonly Token SemiColin;

		public ReturnStmt(Token returnTok, Option<Expression> expr, Token semiColin)
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

		public override TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitReturnStmt(this, arg);
		}

		public override void Accept<TArg>(IStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitReturnStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is ReturnStmt returnStmt)
			{
				return ReturnTok.Equals(returnStmt.ReturnTok) && Expr.Equals(returnStmt.Expr) && SemiColin.Equals(returnStmt.SemiColin);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(ReturnTok);
			code.Add(Expr);
			code.Add(SemiColin);
			return code.ToHashCode();
		}
	}
}
