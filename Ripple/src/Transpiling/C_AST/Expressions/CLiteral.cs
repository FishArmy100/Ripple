using System;
using System.Collections.Generic;
using Ripple.Utils;
using System.Linq;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	class CLiteral : CExpression
	{
		public readonly object Value;
		public readonly CLiteralType Type;

		public CLiteral(object value, CLiteralType type)
		{
			this.Value = value;
			this.Type = type;
		}

		public override void Accept(ICExpressionVisitor visitor)
		{
			visitor.VisitCLiteral(this);
		}

		public override T Accept<T>(ICExpressionVisitor<T> visitor)
		{
			return visitor.VisitCLiteral(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCLiteral(this, arg);
		}

		public override void Accept<TArg>(ICExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCLiteral(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CLiteral cLiteral)
			{
				return Value.Equals(cLiteral.Value) && Type.Equals(cLiteral.Type);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Value);
			code.Add(Type);
			return code.ToHashCode();
		}
	}
}
