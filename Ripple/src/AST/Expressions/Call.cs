using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Utils;
using System.Linq;


namespace Ripple.AST
{
	class Call : Expression
	{
		public readonly Expression Callee;
		public readonly Token OpenParen;
		public readonly List<Expression> Args;
		public readonly Token CloseParen;

		public Call(Expression callee, Token openParen, List<Expression> args, Token closeParen)
		{
			this.Callee = callee;
			this.OpenParen = openParen;
			this.Args = args;
			this.CloseParen = closeParen;
		}

		public override void Accept(IExpressionVisitor visitor)
		{
			visitor.VisitCall(this);
		}

		public override T Accept<T>(IExpressionVisitor<T> visitor)
		{
			return visitor.VisitCall(this);
		}

		public override TReturn Accept<TReturn, TArg>(IExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCall(this, arg);
		}

		public override void Accept<TArg>(IExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCall(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is Call call)
			{
				return Callee.Equals(call.Callee) && OpenParen.Equals(call.OpenParen) && Args.SequenceEqual(call.Args) && CloseParen.Equals(call.CloseParen);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Callee);
			code.Add(OpenParen);
			code.Add(Args);
			code.Add(CloseParen);
			return code.ToHashCode();
		}
	}
}
