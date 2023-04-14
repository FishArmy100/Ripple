using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class BasicType : TypeName
	{
		public readonly Token Identifier;

		public BasicType(Token identifier)
		{
			this.Identifier = identifier;
		}

		public override void Accept(ITypeNameVisitor visitor)
		{
			visitor.VisitBasicType(this);
		}

		public override T Accept<T>(ITypeNameVisitor<T> visitor)
		{
			return visitor.VisitBasicType(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypeNameVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitBasicType(this, arg);
		}

		public override void Accept<TArg>(ITypeNameVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitBasicType(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is BasicType basicType)
			{
				return Identifier.Equals(basicType.Identifier);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Identifier);
			return code.ToHashCode();
		}
	}
}
