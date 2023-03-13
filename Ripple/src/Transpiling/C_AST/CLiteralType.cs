using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Transpiling.SourceGeneration;

namespace Ripple.Transpiling.C_AST
{
	enum CLiteralType
	{
		String,
		Intager,
		Charactor,
		Float,
		True,
		False,
	}

	static class CLiteralTypeExtensions
    {
		public static CType ToCType(this CLiteralType type)
        {
			return type switch
			{
				CLiteralType.String => new CPointer(new CBasicType(CKeywords.CHAR, false), false),
				CLiteralType.Intager => GenType(CKeywords.INT),
				CLiteralType.Charactor => GenType(CKeywords.CHAR),
				CLiteralType.Float => GenType(CKeywords.FLOAT),
				CLiteralType.True => GenType(CKeywords.BOOL),
				CLiteralType.False => GenType(CKeywords.BOOL),
				_ => throw new ArgumentException("Unknown literal type " + type),
			};
        }

		private static CBasicType GenType(string name)
        {
			return new CBasicType(name, false);
        }
    }
}
