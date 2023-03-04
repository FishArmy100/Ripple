using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Transpiling.Source_Generation;

namespace Ripple.Transpiling.C_AST
{
	static class CPrimatives
	{
		public static readonly CBasicType Int =		new CBasicType(CKeywords.INT, false);
		public static readonly CBasicType Float =	new CBasicType(CKeywords.FLOAT, false);
		public static readonly CBasicType Char =	new CBasicType(CKeywords.CHAR, false);
		public static readonly CBasicType Bool =	new CBasicType(CKeywords.BOOL, false);
	}
}
