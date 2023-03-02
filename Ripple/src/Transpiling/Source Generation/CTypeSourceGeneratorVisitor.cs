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
		public string VisitCArray(CArray cArray)
		{
			throw new NotImplementedException();
		}

		public string VisitCBasicType(CBasicType cBasicType)
		{
			throw new NotImplementedException();
		}

		public string VisitCFuncPtr(CFuncPtr cFuncPtr)
		{
			throw new NotImplementedException();
		}

		public string VisitCPointer(CPointer cPointer)
		{
			throw new NotImplementedException();
		}
	}
}
