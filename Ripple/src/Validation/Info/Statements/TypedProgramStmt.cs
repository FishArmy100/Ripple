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
	class TypedProgramStmt : TypedStatement
	{
		public readonly List<TypedFileStmt> Files;
		public readonly string Path;

		public TypedProgramStmt(List<TypedFileStmt> files, string path)
		{
			this.Files = files;
			this.Path = path;
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
				return Files.SequenceEqual(typedProgramStmt.Files) && Path.Equals(typedProgramStmt.Path);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Files);
			code.Add(Path);
			return code.ToHashCode();
		}
	}
}
