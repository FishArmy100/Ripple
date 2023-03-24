using System;
using System.Collections.Generic;
using Ripple.Utils;
using Ripple.Validation;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;
using Ripple.Lexing;
using System.Linq;


namespace Ripple.Validation.Info.Statements
{
	class TypedProgramStmt : TypedStatement
	{
		public readonly List<TypedFileStmt> Files;

		public TypedProgramStmt(List<TypedFileStmt> files)
		{
			this.Files = files;
		}

		public override void Accept(ITypedStatementVisitor visitor)
		{
			visitor.VisitTypedProgramStmt(this);
		}

		public override T Accept<T>(ITypedStatementVisitor<T> visitor)
		{
			return visitor.VisitTypedProgramStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypedStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypedProgramStmt(this, arg);
		}

		public override void Accept<TArg>(ITypedStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypedProgramStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypedProgramStmt typedProgramStmt)
			{
				return Files.SequenceEqual(typedProgramStmt.Files);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Files);
			return code.ToHashCode();
		}
	}
}
