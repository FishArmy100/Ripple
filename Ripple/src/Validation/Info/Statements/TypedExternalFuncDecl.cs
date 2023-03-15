using System;
using System.Collections.Generic;
using Ripple.Utils;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;


namespace Ripple.Validation.Info.Statements
{
	class TypedExternalFuncDecl : TypedStatement
	{
		public readonly FunctionInfo Info;

		public TypedExternalFuncDecl(FunctionInfo info)
		{
			this.Info = info;
		}

		public override void Accept(ITypedStatementVisitor visitor)
		{
			visitor.VisitTypedExternalFuncDecl(this);
		}

		public override T Accept<T>(ITypedStatementVisitor<T> visitor)
		{
			return visitor.VisitTypedExternalFuncDecl(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypedStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypedExternalFuncDecl(this, arg);
		}

		public override void Accept<TArg>(ITypedStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypedExternalFuncDecl(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypedExternalFuncDecl typedExternalFuncDecl)
			{
				return Info.Equals(typedExternalFuncDecl.Info);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Info);
			return code.ToHashCode();
		}
	}
}
