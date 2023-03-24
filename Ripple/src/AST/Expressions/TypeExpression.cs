using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Utils;


namespace Ripple.AST
{
	class TypeExpression : Expression
	{
		public readonly Token Name;
		public readonly Token GreaterThan;
		public readonly List<Token> Lifetimes;
		public readonly Token LessThan;

		public TypeExpression(Token name, Token greaterThan, List<Token> lifetimes, Token lessThan)
		{
			this.Name = name;
			this.GreaterThan = greaterThan;
			this.Lifetimes = lifetimes;
			this.LessThan = lessThan;
		}

		public override void Accept(IExpressionVisitor visitor)
		{
			visitor.VisitTypeExpression(this);
		}

		public override T Accept<T>(IExpressionVisitor<T> visitor)
		{
			return visitor.VisitTypeExpression(this);
		}

		public override TReturn Accept<TReturn, TArg>(IExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypeExpression(this, arg);
		}

		public override void Accept<TArg>(IExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypeExpression(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypeExpression typeExpression)
			{
				return Name.Equals(typeExpression.Name) && GreaterThan.Equals(typeExpression.GreaterThan) && Lifetimes.Equals(typeExpression.Lifetimes) && LessThan.Equals(typeExpression.LessThan);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Name);
			code.Add(GreaterThan);
			code.Add(Lifetimes);
			code.Add(LessThan);
			return code.ToHashCode();
		}
	}
}
