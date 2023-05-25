using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class ThisFunctionParameter : Statement
	{
		public readonly Token ThisToken;
		public readonly Token? MutToken;
		public readonly Token? RefToken;
		public readonly Token? LifetimeToken;

		public ThisFunctionParameter(Token thisToken, Token? mutToken, Token? refToken, Token? lifetimeToken)
		{
			this.ThisToken = thisToken;
			this.MutToken = mutToken;
			this.RefToken = refToken;
			this.LifetimeToken = lifetimeToken;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitThisFunctionParameter(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitThisFunctionParameter(this);
		}

		public override TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitThisFunctionParameter(this, arg);
		}

		public override void Accept<TArg>(IStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitThisFunctionParameter(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is ThisFunctionParameter thisFunctionParameter)
			{
				return ThisToken.Equals(thisFunctionParameter.ThisToken) && MutToken.Equals(thisFunctionParameter.MutToken) && RefToken.Equals(thisFunctionParameter.RefToken) && LifetimeToken.Equals(thisFunctionParameter.LifetimeToken);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(ThisToken);
			code.Add(MutToken);
			code.Add(RefToken);
			code.Add(LifetimeToken);
			return code.ToHashCode();
		}
	}
}
