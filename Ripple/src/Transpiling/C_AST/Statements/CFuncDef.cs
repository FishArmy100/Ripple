using System;
using System.Collections.Generic;
using Raucse;
using System.Linq;
using System.Linq;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	class CFuncDef : CStatement
	{
		public readonly CType Returned;
		public readonly string Name;
		public readonly List<CFuncParam> Parameters;
		public readonly CBlockStmt Body;

		public CFuncDef(CType returned, string name, List<CFuncParam> parameters, CBlockStmt body)
		{
			this.Returned = returned;
			this.Name = name;
			this.Parameters = parameters;
			this.Body = body;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitCFuncDef(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitCFuncDef(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCFuncDef(this, arg);
		}

		public override void Accept<TArg>(ICStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCFuncDef(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CFuncDef cFuncDef)
			{
				return Returned.Equals(cFuncDef.Returned) && Name.Equals(cFuncDef.Name) && Parameters.SequenceEqual(cFuncDef.Parameters) && Body.Equals(cFuncDef.Body);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Returned);
			code.Add(Name);
			code.Add(Parameters);
			code.Add(Body);
			return code.ToHashCode();
		}
	}
}
