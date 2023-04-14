using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class Parameters : Statement
	{
		public readonly Token OpenParen;
		public readonly List<(TypeName,Token)> ParamList;
		public readonly Token CloseParen;

		public Parameters(Token openParen, List<(TypeName,Token)> paramList, Token closeParen)
		{
			this.OpenParen = openParen;
			this.ParamList = paramList;
			this.CloseParen = closeParen;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitParameters(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitParameters(this);
		}

		public override TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitParameters(this, arg);
		}

		public override void Accept<TArg>(IStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitParameters(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is Parameters parameters)
			{
				return OpenParen.Equals(parameters.OpenParen) && ParamList.SequenceEqual(parameters.ParamList) && CloseParen.Equals(parameters.CloseParen);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(OpenParen);
			code.Add(ParamList);
			code.Add(CloseParen);
			return code.ToHashCode();
		}
	}
}
