using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.Transpiling.ASTConversion.SimplifiedTypes;

namespace Ripple.Transpiling.ASTConversion
{
	static class SimplifiedTypeGenerator
	{
		public static SimplifiedType Generate(TypeName type)
		{
			return type.Accept(new SimplifiedTypeGeneratorVisitor());
		}

		private class SimplifiedTypeGeneratorVisitor : ITypeNameVisitor<SimplifiedType>
		{
			public SimplifiedType VisitArrayType(ArrayType arrayType)
			{
				bool isMutable = arrayType.MutToken.HasValue;
				SimplifiedType contained = arrayType.BaseType.Accept(this);
				int size = int.Parse(arrayType.Size.Text);
				return new SArray(isMutable, contained, size);
			}

			public SimplifiedType VisitBasicType(BasicType basicType)
			{
				string name = basicType.Identifier.Text;
				bool isMutable = basicType.MutToken.HasValue;
				return new SBasicType(isMutable, name);
			}

			public SimplifiedType VisitFuncPtr(FuncPtr funcPtr)
			{
				bool isMutable = funcPtr.MutToken.HasValue;
				List<SimplifiedType> parameters = funcPtr.Parameters.Select(p => p.Accept(this)).ToList();
				SimplifiedType returned = funcPtr.ReturnType.Accept(this);
				return new SFuncPtr(isMutable, parameters, returned);
			}

			public SimplifiedType VisitGroupedType(GroupedType groupedType)
			{
				return groupedType.Type.Accept(this);
			}

			public SimplifiedType VisitPointerType(PointerType pointerType)
			{
				bool isMutable = pointerType.MutToken.HasValue;
				SimplifiedType contained = pointerType.BaseType.Accept(this);
				return new SPointer(isMutable, contained);
			}

			public SimplifiedType VisitReferenceType(ReferenceType referenceType)
			{
				bool isMutable = referenceType.MutToken.HasValue;
				SimplifiedType contained = referenceType.BaseType.Accept(this);
				return new SReference(isMutable, contained);
			}
		}
	}
}
