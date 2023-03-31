using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System.Linq;
using System.Linq;


namespace Ripple.AST
{
	class GenericParameters : Statement
	{
		public readonly Token LessThan;
		public readonly List<Token> Lifetimes;
		public readonly Token GreaterThan;

		public GenericParameters(Token lessThan, List<Token> lifetimes, Token greaterThan)
		{
			this.LessThan = lessThan;
			this.Lifetimes = lifetimes;
			this.GreaterThan = greaterThan;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitGenericParameters(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitGenericParameters(this);
		}

		public override TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitGenericParameters(this, arg);
		}

		public override void Accept<TArg>(IStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitGenericParameters(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is GenericParameters genericParameters)
			{
				return LessThan.Equals(genericParameters.LessThan) && Lifetimes.SequenceEqual(genericParameters.Lifetimes) && GreaterThan.Equals(genericParameters.GreaterThan);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(LessThan);
			code.Add(Lifetimes);
			code.Add(GreaterThan);
			return code.ToHashCode();
		}
	}
}
