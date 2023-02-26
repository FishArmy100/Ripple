using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Transpiling.C_AST;

namespace Ripple.Transpiling.Source_Generation
{
	class CTypeSourceGeneratorVisitor : ICTypeVisitor<string>
	{
		public string VisitArray(C_AST.Array array)
		{
			throw new NotImplementedException();
		}

		public string VisitBasic(Basic basic)
		{
			return basic.Name;
		}

		public string VisitFuncPtr(FuncPtr funcPtr)
		{
			throw new NotImplementedException();
		}

		public string VisitPointer(Pointer pointer)
		{
			return $"{pointer.BaseType.Accept(this)}*";
		}
	}
}
