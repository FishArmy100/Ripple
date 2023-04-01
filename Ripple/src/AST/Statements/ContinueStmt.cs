using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class ContinueStmt : Statement
	{
		public readonly Token ContinueToken;
		public readonly Token SemiColon;

		public ContinueStmt(Token continueToken, Token semiColon)
		{
			this.ContinueToken = continueToken;
			this.SemiColon = semiColon;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitContinueStmt(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitContinueStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitContinueStmt(this, arg);
		}

		public override void Accept<TArg>(IStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitContinueStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is ContinueStmt continueStmt)
			{
				return ContinueToken.Equals(continueStmt.ContinueToken) && SemiColon.Equals(continueStmt.SemiColon);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(ContinueToken);
			code.Add(SemiColon);
			return code.ToHashCode();
		}
	}
}
