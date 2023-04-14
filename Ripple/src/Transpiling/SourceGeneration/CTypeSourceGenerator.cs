using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Transpiling.C_AST;
using Raucse;

namespace Ripple.Transpiling.SourceGeneration
{
	static class CTypeSourceGenerator
	{
		public static string GenerateSource(CType type, Option<string> baseName)
		{
			return type.Accept(new CTypeNameGeneratorVisitor(), baseName);
		}

		private class CTypeNameGeneratorVisitor : ICTypeVisitor<string, Option<string>>
		{
			public string VisitCArray(CArray cArray, Option<string> arg)
			{
				string size = cArray.Size.Match(ok => ok.ToString(), () => "");
				return arg.Match(ok =>
				{
					return cArray.BaseType.Accept(this, ok + $"[{size}]");
				},
				() =>
				{
					return cArray.BaseType.Accept(this, $"[{size}]");
				});
			}

			public string VisitCBasicType(CBasicType cBasicType, Option<string> arg)
			{
				if(cBasicType.IsStruct)
				{
					return CKeywords.STRUCT + " " + cBasicType.Name + " " + arg.Match(ok => ok, () => "");
				}

				return cBasicType.Name + " " + arg.Match(ok => ok, () => "");
			}

			public string VisitCFuncPtr(CFuncPtr cFuncPtr, Option<string> arg)
			{
				string returned = cFuncPtr.Returned.Accept(this, new Option<string>());
				string parameters = string.Join(", ", cFuncPtr.Parameters.Select(p => p.Accept(this, new Option<string>())));
				string b = arg.MatchOrEmpty();
				return $"{returned}(*{b})({parameters})";
			}

			public string VisitCPointer(CPointer cPointer, Option<string> arg)
			{
				if (cPointer.BaseType is CArray)
					throw new TranspilingException("Cannot have a pointer to an array.");

				string pointer = "*" + (cPointer.IsConst ? "const " : "");
				return cPointer.BaseType.Accept(this, pointer + arg.MatchOrEmpty());
			}
		}
	}
}
