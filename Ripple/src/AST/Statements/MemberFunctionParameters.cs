using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class MemberFunctionParameters : Statement
	{
		public readonly Token OpenParen;
		public readonly Option<ThisFunctionParameter> ThisParameter;
		public readonly List<Pair<TypeName,Token>> ParamList;
		public readonly Token CloseParen;

		public MemberFunctionParameters(Token openParen, Option<ThisFunctionParameter> thisParameter, List<Pair<TypeName,Token>> paramList, Token closeParen)
		{
			this.OpenParen = openParen;
			this.ThisParameter = thisParameter;
			this.ParamList = paramList;
			this.CloseParen = closeParen;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitMemberFunctionParameters(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitMemberFunctionParameters(this);
		}

		public override TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitMemberFunctionParameters(this, arg);
		}

		public override void Accept<TArg>(IStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitMemberFunctionParameters(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is MemberFunctionParameters memberFunctionParameters)
			{
				return OpenParen.Equals(memberFunctionParameters.OpenParen) && ThisParameter.Equals(memberFunctionParameters.ThisParameter) && ParamList.SequenceEqual(memberFunctionParameters.ParamList) && CloseParen.Equals(memberFunctionParameters.CloseParen);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(OpenParen);
			code.Add(ThisParameter);
			code.Add(ParamList);
			code.Add(CloseParen);
			return code.ToHashCode();
		}
	}
}
