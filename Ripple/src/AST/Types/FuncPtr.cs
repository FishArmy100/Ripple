using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class FuncPtr : TypeName
	{
		public readonly Token FuncToken;
		public readonly Option<List<Token>> Lifetimes;
		public readonly Token OpenParen;
		public readonly List<TypeName> Parameters;
		public readonly Token CloseParen;
		public readonly Token Arrow;
		public readonly TypeName ReturnType;

		public FuncPtr(Token funcToken, Option<List<Token>> lifetimes, Token openParen, List<TypeName> parameters, Token closeParen, Token arrow, TypeName returnType)
		{
			this.FuncToken = funcToken;
			this.Lifetimes = lifetimes;
			this.OpenParen = openParen;
			this.Parameters = parameters;
			this.CloseParen = closeParen;
			this.Arrow = arrow;
			this.ReturnType = returnType;
		}

		public override void Accept(ITypeNameVisitor visitor)
		{
			visitor.VisitFuncPtr(this);
		}

		public override T Accept<T>(ITypeNameVisitor<T> visitor)
		{
			return visitor.VisitFuncPtr(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypeNameVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitFuncPtr(this, arg);
		}

		public override void Accept<TArg>(ITypeNameVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitFuncPtr(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is FuncPtr funcPtr)
			{
				return FuncToken.Equals(funcPtr.FuncToken) && Lifetimes.Equals(funcPtr.Lifetimes) && OpenParen.Equals(funcPtr.OpenParen) && Parameters.SequenceEqual(funcPtr.Parameters) && CloseParen.Equals(funcPtr.CloseParen) && Arrow.Equals(funcPtr.Arrow) && ReturnType.Equals(funcPtr.ReturnType);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(FuncToken);
			code.Add(Lifetimes);
			code.Add(OpenParen);
			code.Add(Parameters);
			code.Add(CloseParen);
			code.Add(Arrow);
			code.Add(ReturnType);
			return code.ToHashCode();
		}
	}
}
