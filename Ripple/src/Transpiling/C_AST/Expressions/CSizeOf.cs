using System.Collections.Generic;
using Raucse;
using System;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	public class CSizeOf : CExpression
	{
		public readonly CType Type;

		public CSizeOf(CType type)
		{
			this.Type = type;
		}

		public override void Accept(ICExpressionVisitor visitor)
		{
			visitor.VisitCSizeOf(this);
		}

		public override T Accept<T>(ICExpressionVisitor<T> visitor)
		{
			return visitor.VisitCSizeOf(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCSizeOf(this, arg);
		}

		public override void Accept<TArg>(ICExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCSizeOf(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CSizeOf cSizeOf)
			{
				return Type.Equals(cSizeOf.Type);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Type);
			return code.ToHashCode();
		}
	}
}
