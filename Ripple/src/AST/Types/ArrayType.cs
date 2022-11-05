using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


namespace Ripple.AST
{
	class ArrayType : TypeName
	{
		public readonly TypeName BaseType;
		public readonly Token? MutToken;
		public readonly Token OpenBracket;
		public readonly Token Size;
		public readonly Token CloseBracket;

		public ArrayType(TypeName baseType, Token? mutToken, Token openBracket, Token size, Token closeBracket)
		{
			this.BaseType = baseType;
			this.MutToken = mutToken;
			this.OpenBracket = openBracket;
			this.Size = size;
			this.CloseBracket = closeBracket;
		}

		public override void Accept(ITypeNameVisitor visitor)
		{
			visitor.VisitArrayType(this);
		}

		public override T Accept<T>(ITypeNameVisitor<T> visitor)
		{
			return visitor.VisitArrayType(this);
		}

		public override bool Equals(object other)
		{
			if(other is ArrayType arrayType)
			{
				return BaseType.Equals(arrayType.BaseType) && MutToken.Equals(arrayType.MutToken) && OpenBracket.Equals(arrayType.OpenBracket) && Size.Equals(arrayType.Size) && CloseBracket.Equals(arrayType.CloseBracket);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(BaseType, MutToken, OpenBracket, Size, CloseBracket);
		}
	}
}