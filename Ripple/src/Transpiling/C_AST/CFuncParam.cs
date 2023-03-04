using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class CFuncParam
	{
		public readonly CType Type;
		public readonly string Name;

		public CFuncParam(CType type, string name)
		{
			this.Type = type;
			this.Name = name;
		}

		public override bool Equals(object other)
		{
			if(other is CFuncParam funcParam)
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
