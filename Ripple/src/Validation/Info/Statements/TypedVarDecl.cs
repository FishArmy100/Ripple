using System;
using System.Collections.Generic;
using Raucse;
using Ripple.Validation;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;
using Ripple.Lexing;
using System.Linq;


namespace Ripple.Validation.Info.Statements
{
	class TypedVarDecl : TypedStatement
	{
		public readonly bool IsUnsafe;
		public readonly TypeInfo Type;
		public readonly List<string> VariableNames;
		public readonly TypedExpression Initalizer;

		public TypedVarDecl(bool isUnsafe, TypeInfo type, List<string> variableNames, TypedExpression initalizer)
		{
			this.IsUnsafe = isUnsafe;
			this.Type = type;
			this.VariableNames = variableNames;
			this.Initalizer = initalizer;
		}

		public override void Accept(ITypedStatementVisitor visitor)
		{
			visitor.VisitTypedVarDecl(this);
		}

		public override T Accept<T>(ITypedStatementVisitor<T> visitor)
		{
			return visitor.VisitTypedVarDecl(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypedStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypedVarDecl(this, arg);
		}

		public override void Accept<TArg>(ITypedStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypedVarDecl(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypedVarDecl typedVarDecl)
			{
				return IsUnsafe.Equals(typedVarDecl.IsUnsafe) && Type.Equals(typedVarDecl.Type) && VariableNames.SequenceEqual(typedVarDecl.VariableNames) && Initalizer.Equals(typedVarDecl.Initalizer);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(IsUnsafe);
			code.Add(Type);
			code.Add(VariableNames);
			code.Add(Initalizer);
			return code.ToHashCode();
		}
	}
}
