using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System.Linq;
using System.Linq;


namespace Ripple.AST
{
	class BreakStmt : Statement
	{
		public readonly Token BreakToken;
		public readonly Token SemiColon;

		public BreakStmt(Token breakToken, Token semiColon)
		{
			this.BreakToken = breakToken;
			this.SemiColon = semiColon;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitBreakStmt(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitBreakStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitBreakStmt(this, arg);
		}

		public override void Accept<TArg>(IStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitBreakStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is BreakStmt breakStmt)
			{
				return BreakToken.Equals(breakStmt.BreakToken) && SemiColon.Equals(breakStmt.SemiColon);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(BreakToken);
			code.Add(SemiColon);
			return code.ToHashCode();
		}
	}
}
