using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class ArrayType : TypeName
	{
		public readonly TypeName BaseType;
		public readonly Token OpenBracket;
		public readonly Token Size;
		public readonly Token CloseBracket;

		public ArrayType(TypeName baseType, Token openBracket, Token size, Token closeBracket)
		{
			this.BaseType = baseType;
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

		public override TReturn Accept<TReturn, TArg>(ITypeNameVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitArrayType(this, arg);
		}

		public override void Accept<TArg>(ITypeNameVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitArrayType(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is ArrayType arrayType)
			{
				return BaseType.Equals(arrayType.BaseType) && OpenBracket.Equals(arrayType.OpenBracket) && Size.Equals(arrayType.Size) && CloseBracket.Equals(arrayType.CloseBracket);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(BaseType);
			code.Add(OpenBracket);
			code.Add(Size);
			code.Add(CloseBracket);
			return code.ToHashCode();
		}
	}
}
