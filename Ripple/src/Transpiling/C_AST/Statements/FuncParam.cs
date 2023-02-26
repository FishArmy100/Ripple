using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class FuncParam : CStatement
	{
		public readonly CType Type;
		public readonly string Name;

		public FuncParam(CType type, string name)
		{
			this.Type = type;
			this.Name = name;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitFuncParam(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitFuncParam(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitFuncParam(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is FuncParam funcParam)
			{
				return Type.Equals(funcParam.Type) && Name.Equals(funcParam.Name);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Type);
			code.Add(Name);
			return code.ToHashCode();
		}
	}
}
