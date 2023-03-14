using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Transpiling.C_AST;
using Ripple.AST;
using Ripple.Transpiling.ASTConversion.SimplifiedTypes;
using Ripple.Utils;
using Ripple.Transpiling.SourceGeneration;

namespace Ripple.Transpiling.ASTConversion
{
	static class ASTConverter
	{
		public static CProgram ConvertAST(ProgramStmt program)
		{
			return null;
		}

		public static CType ConvertToCType(TypeName name, ref CArrayRegistry registry)
		{
			SimplifiedType simplified = SimplifiedTypeGenerator.Generate(name);
			return simplified.Accept(new SimplifiedTypeToCTypeConverterVisitor(registry));
		}

		public static CType ConvertToCType(SimplifiedType type, ref CArrayRegistry registry)
        {
			return type.Accept(new SimplifiedTypeToCTypeConverterVisitor(registry));
        }

		private class SimplifiedTypeToCTypeConverterVisitor : ISimplifiedTypeVisitor<CType>
		{
			private readonly CArrayRegistry m_Registry;

			public SimplifiedTypeToCTypeConverterVisitor(CArrayRegistry registry)
			{
				m_Registry = registry;
			}

			public CType VisitSArray(SArray sArray)
			{
				return new CBasicType(m_Registry.GetArrayAlias(sArray).Name, false); 
			}

			public CType VisitSBasicType(SBasicType sBasicType)
			{
				return new CBasicType(sBasicType.Name, false);
			}

			public CType VisitSFuncPtr(SFuncPtr sFuncPtr)
			{
				List<CType> parameters = sFuncPtr.Parameters.Select(p => p.Accept(this)).ToList();
				CType returned = sFuncPtr.Returned.Accept(this);
				return new CFuncPtr(returned, parameters);
			}

			public CType VisitSPointer(SPointer sPointer)
			{
				return new CPointer(sPointer.Contained.Accept(this), false);
			}

			public CType VisitSReference(SReference sReference)
			{
				return new CPointer(sReference.Contained.Accept(this), false);
			}
		}
	}
}
