using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Utils;


namespace Ripple.AST
{
	class FuncPtr : TypeName
	{
		public readonly Token? MutToken;
		public readonly Token OpenParen;
		public readonly List<TypeName> Parameters;
		public readonly Token CloseParen;
		public readonly Token Arrow;
		public readonly TypeName ReturnType;

		public FuncPtr(Token? mutToken, Token openParen, List<TypeName> parameters, Token closeParen, Token arrow, TypeName returnType)
		{
			this.MutToken = mutToken;
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

		public override bool Equals(object other)
		{
			if(other is FuncPtr funcPtr)
			{
				return MutToken.Equals(funcPtr.MutToken) && OpenParen.Equals(funcPtr.OpenParen) && Parameters.Equals(funcPtr.Parameters) && CloseParen.Equals(funcPtr.CloseParen) && Arrow.Equals(funcPtr.Arrow) && ReturnType.Equals(funcPtr.ReturnType);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(MutToken);
			code.Add(OpenParen);
			code.Add(Parameters);
			code.Add(CloseParen);
			code.Add(Arrow);
			code.Add(ReturnType);
			return code.ToHashCode();
		}
	}
}
