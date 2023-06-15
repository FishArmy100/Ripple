using System.Collections.Generic;
using Raucse;
using Ripple.Validation;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;
using Ripple.Validation.Info.Lifetimes;
using Ripple.Validation.Info.Functions;
using Ripple.Validation.Info.Values;
using Ripple.Lexing;
using System;
using System.Linq;


namespace Ripple.Validation.Info.Statements
{
	public class TypedUnsafeBlock : TypedStatement
	{
		public readonly List<TypedStatement> Statements;

		public TypedUnsafeBlock(List<TypedStatement> statements)
		{
			this.Statements = statements;
		}

		public override void Accept(ITypedStatementVisitor visitor)
		{
			visitor.VisitTypedUnsafeBlock(this);
		}

		public override T Accept<T>(ITypedStatementVisitor<T> visitor)
		{
			return visitor.VisitTypedUnsafeBlock(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypedStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypedUnsafeBlock(this, arg);
		}

		public override void Accept<TArg>(ITypedStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypedUnsafeBlock(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypedUnsafeBlock typedUnsafeBlock)
			{
				return Statements.SequenceEqual(typedUnsafeBlock.Statements);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Statements);
			return code.ToHashCode();
		}
	}
}
