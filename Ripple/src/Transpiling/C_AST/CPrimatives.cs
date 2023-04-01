using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Transpiling.SourceGeneration;

namespace Ripple.Transpiling.C_AST
{
	public static class CPrimatives
	{
		public static readonly CBasicType Int =		new CBasicType(CKeywords.INT, false, false);
		public static readonly CBasicType Float =	new CBasicType(CKeywords.FLOAT, false, false);
		public static readonly CBasicType Char =	new CBasicType(CKeywords.CHAR, false, false);
		public static readonly CBasicType Bool =	new CBasicType(CKeywords.BOOL, false, false);
		public static readonly CPointer   CString =	new CPointer(new CBasicType(CKeywords.CHAR, true, false), false);
	}
}
