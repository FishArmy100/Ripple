using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Validation.Info.Types;
using Ripple.Transpiling.C_AST;

namespace Ripple.Transpiling.ASTConversion
{
	class TypeConverterVisitor : ITypeInfoVisitor<CType>
	{
		private readonly CArrayRegistry m_Registry;

		public TypeConverterVisitor(CArrayRegistry registry)
		{
			m_Registry = registry;
		}

		public CType VisitArrayInfo(ArrayInfo sArray)
		{
			return new CBasicType(m_Registry.GetArrayAlias(sArray).Name, false);
		}

		public CType VisitBasicTypeInfo(BasicTypeInfo sBasicType)
		{
			return new CBasicType(sBasicType.Name, false);
		}

		public CType VisitFuncPtrInfo(FuncPtrInfo sFuncPtr)
		{
			List<CType> parameters = sFuncPtr.Parameters.Select(p => p.Accept(this)).ToList();
			CType returned = sFuncPtr.Returned.Accept(this);
			return new CFuncPtr(returned, parameters);
		}

		public CType VisitPointerInfo(PointerInfo sPointer)
		{
			return new CPointer(sPointer.Contained.Accept(this), false);
		}

		public CType VisitReferenceInfo(ReferenceInfo sReference)
		{
			return new CPointer(sReference.Contained.Accept(this), false);
		}
	}
}
