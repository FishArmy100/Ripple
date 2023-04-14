using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class SizeOf : Expression
	{
		public readonly Token SizeofToken;
		public readonly Token LessThan;
		public readonly TypeName Type;
		public readonly Token GreaterThan;
		public readonly Token OpenParen;
		public readonly Token CloseParen;

		public SizeOf(Token sizeofToken, Token lessThan, TypeName type, Token greaterThan, Token openParen, Token closeParen)
		{
			this.SizeofToken = sizeofToken;
			this.LessThan = lessThan;
			this.Type = type;
			this.GreaterThan = greaterThan;
			this.OpenParen = openParen;
			this.CloseParen = closeParen;
		}

		public override void Accept(IExpressionVisitor visitor)
		{
			visitor.VisitSizeOf(this);
		}

		public override T Accept<T>(IExpressionVisitor<T> visitor)
		{
			return visitor.VisitSizeOf(this);
		}

		public override TReturn Accept<TReturn, TArg>(IExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitSizeOf(this, arg);
		}

		public override void Accept<TArg>(IExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitSizeOf(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is SizeOf sizeOf)
			{
				return SizeofToken.Equals(sizeOf.SizeofToken) && LessThan.Equals(sizeOf.LessThan) && Type.Equals(sizeOf.Type) && GreaterThan.Equals(sizeOf.GreaterThan) && OpenParen.Equals(sizeOf.OpenParen) && CloseParen.Equals(sizeOf.CloseParen);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(SizeofToken);
			code.Add(LessThan);
			code.Add(Type);
			code.Add(GreaterThan);
			code.Add(OpenParen);
			code.Add(CloseParen);
			return code.ToHashCode();
		}
	}
}
