using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils;

namespace Ripple.Transpiling.C_AST
{
	static class C_ASTPrinter
	{
		public static string PrintCType(CType type)
		{
			CTypePrinterVisitor visitor = new CTypePrinterVisitor();
			type.Accept(visitor);
			return visitor.ToString();
		}

		private class CTypePrinterVisitor : ICTypeVisitor
		{
			private readonly StringConstructor m_StringConstructor = new StringConstructor("   ");
			public void VisitCArray(CArray cArray)
			{
				m_StringConstructor.PrintLine("Array:");
				m_StringConstructor.TabIn();
				cArray.Size.Match(ok => m_StringConstructor.PrintLine($"Size: {ok}"));

				m_StringConstructor.PrintLine("Type:");
				m_StringConstructor.TabIn();
				cArray.BaseType.Accept(this);
				m_StringConstructor.TabOut();

				m_StringConstructor.TabOut();
			}

			public void VisitCBasicType(CBasicType cBasicType)
			{
				m_StringConstructor.PrintLine(cBasicType.Name);
				m_StringConstructor.TabIn();
				m_StringConstructor.PrintLine($"Is Const: {cBasicType.IsConst}");
				m_StringConstructor.TabOut();
			}

			public void VisitCFuncPtr(CFuncPtr cFuncPtr)
			{
				m_StringConstructor.PrintLine("Function Pointer:");
				m_StringConstructor.TabIn();

				m_StringConstructor.PrintLine("Parameters:");
				m_StringConstructor.TabIn();
				foreach (CType param in cFuncPtr.Parameters)
					param.Accept(this);
				m_StringConstructor.TabOut();

				m_StringConstructor.PrintLine("Returned:");
				m_StringConstructor.TabIn();
				cFuncPtr.Returned.Accept(this);
				m_StringConstructor.TabOut();

				m_StringConstructor.TabOut();
			}

			public void VisitCPointer(CPointer cPointer)
			{
				m_StringConstructor.PrintLine("Pointer:");
				m_StringConstructor.TabIn();
				cPointer.BaseType.Accept(this);
				m_StringConstructor.PrintLine($"Is Const: {cPointer.IsConst}");
				m_StringConstructor.TabOut();
			}

			public override string ToString()
			{
				return m_StringConstructor.ToString();
			}
		}
	}
}
