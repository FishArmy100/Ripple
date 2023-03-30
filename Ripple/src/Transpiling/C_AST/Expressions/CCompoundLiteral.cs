using System;
using System.Collections.Generic;
using Ripple.Utils;
using System.Linq;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	class CCompoundLiteral : CExpression
	{
		public readonly CType Type;
		public readonly CInitalizerList Initalizer;

		public CCompoundLiteral(CType type, CInitalizerList initalizer)
		{
			this.Type = type;
			this.Initalizer = initalizer;
		}

		public override void Accept(ICExpressionVisitor visitor)
		{
			visitor.VisitCCompoundLiteral(this);
		}

		public override T Accept<T>(ICExpressionVisitor<T> visitor)
		{
			return visitor.VisitCCompoundLiteral(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCCompoundLiteral(this, arg);
		}

		public override void Accept<TArg>(ICExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCCompoundLiteral(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CCompoundLiteral cCompoundLiteral)
			{
				return Type.Equals(cCompoundLiteral.Type) && Initalizer.Equals(cCompoundLiteral.Initalizer);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Type);
			code.Add(Initalizer);
			return code.ToHashCode();
		}
	}
}
