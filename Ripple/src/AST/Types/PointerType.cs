using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System.Linq;
using System.Linq;
using System.Linq;


namespace Ripple.AST
{
	class PointerType : TypeName
	{
		public readonly TypeName BaseType;
		public readonly Token? MutToken;
		public readonly Token Star;

		public PointerType(TypeName baseType, Token? mutToken, Token star)
		{
			this.BaseType = baseType;
			this.MutToken = mutToken;
			this.Star = star;
		}

		public override void Accept(ITypeNameVisitor visitor)
		{
			visitor.VisitPointerType(this);
		}

		public override T Accept<T>(ITypeNameVisitor<T> visitor)
		{
			return visitor.VisitPointerType(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypeNameVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitPointerType(this, arg);
		}

		public override void Accept<TArg>(ITypeNameVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitPointerType(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is PointerType pointerType)
			{
				return BaseType.Equals(pointerType.BaseType) && MutToken.Equals(pointerType.MutToken) && Star.Equals(pointerType.Star);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(BaseType);
			code.Add(MutToken);
			code.Add(Star);
			return code.ToHashCode();
		}
	}
}
