using System;
using System.Collections.Generic;
using Raucse;
using System.Linq;
using System.Linq;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	class CFuncDecl : CStatement
	{
		public readonly CType Returned;
		public readonly string Name;
		public readonly List<CFuncParam> Parameters;

		public CFuncDecl(CType returned, string name, List<CFuncParam> parameters)
		{
			this.Returned = returned;
			this.Name = name;
			this.Parameters = parameters;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitCFuncDecl(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitCFuncDecl(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCFuncDecl(this, arg);
		}

		public override void Accept<TArg>(ICStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCFuncDecl(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CFuncDecl cFuncDecl)
			{
				return Returned.Equals(cFuncDecl.Returned) && Name.Equals(cFuncDecl.Name) && Parameters.SequenceEqual(cFuncDecl.Parameters);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Returned);
			code.Add(Name);
			code.Add(Parameters);
			return code.ToHashCode();
		}
	}
}
