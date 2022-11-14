using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Utils;


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

		public override bool Equals(object other)
		{
			if(other is GenericParameters genericParameters)
			{
				return LessThan.Equals(genericParameters.LessThan) && Lifetimes.Equals(genericParameters.Lifetimes) && GreaterThan.Equals(genericParameters.GreaterThan);
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
