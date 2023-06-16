using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class GenericType : TypeName
	{
		public readonly Token Identifier;
		public readonly Token LessThan;
		public readonly List<Token> Lifetimes;
		public readonly Token GreaterThan;

		public GenericType(Token identifier, Token lessThan, List<Token> lifetimes, Token greaterThan)
		{
			this.Identifier = identifier;
			this.LessThan = lessThan;
			this.Lifetimes = lifetimes;
			this.GreaterThan = greaterThan;
		}

		public override void Accept(ITypeNameVisitor visitor)
		{
			visitor.VisitGenericType(this);
		}

		public override T Accept<T>(ITypeNameVisitor<T> visitor)
		{
			return visitor.VisitGenericType(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypeNameVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitGenericType(this, arg);
		}

		public override void Accept<TArg>(ITypeNameVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitGenericType(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is GenericType genericType)
			{
				return Identifier.Equals(genericType.Identifier) && LessThan.Equals(genericType.LessThan) && Lifetimes.SequenceEqual(genericType.Lifetimes) && GreaterThan.Equals(genericType.GreaterThan);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Identifier);
			code.Add(LessThan);
			code.Add(Lifetimes);
			code.Add(GreaterThan);
			return code.ToHashCode();
		}
	}
}
