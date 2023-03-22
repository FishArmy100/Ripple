using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Transpiling.C_AST;
using Ripple.AST;
using Ripple.Transpiling.SourceGeneration;
using Ripple.Utils;
using Ripple.Validation.Info.Types;

namespace Ripple.Transpiling.ASTConversion
{
	class CArrayRegistry
	{
		private readonly List<CStructDef> m_OrderedArrayStructs = new List<CStructDef>();
		private readonly Dictionary<TypeInfo, CStructDef> m_ArrayStructs = new Dictionary<TypeInfo, CStructDef>();

		public List<CStructDef> GetArrayAliasStructs() => m_OrderedArrayStructs;
		public CStructDef GetArrayAlias(ArrayInfo array)
		{
			if(m_ArrayStructs.TryGetValue(array, out CStructDef alias))
			{
				return alias;
			}
			else
			{
				alias = GenerateArrayAlias(array);
				return alias;
			}
		}

		private CStructDef GenerateArrayAlias(ArrayInfo array)
		{
			CStructDefData data = array.Contained.Accept(new CStructDefDataGeneratorVisitor(this));
			CStructDef def = new CStructDef($"{data.Name}_array_{array.Size}", new List<CStructMember>
			{
				new CStructMember(new CArray(data.Contained, array.Size), CKeywords.ARRAY_DATA_NAME)
			});

			m_ArrayStructs.Add(array, def);
			m_OrderedArrayStructs.Add(def);

			return def;
		}

		private class CStructDefDataGeneratorVisitor : ITypeInfoVisitor<CStructDefData>
		{
			private readonly CArrayRegistry m_Registry;

			public CStructDefDataGeneratorVisitor(CArrayRegistry registry)
			{
				m_Registry = registry;
			}

			public CStructDefData VisitArrayInfo(ArrayInfo sArray)
			{
				CStructDef def = m_Registry.GenerateArrayAlias(sArray);
				return new CStructDefData(def.Name, new CBasicType(def.Name, false, true));
			}

			public CStructDefData VisitBasicTypeInfo(BasicTypeInfo sBasicType)
			{
				return new CStructDefData(sBasicType.Name, new CBasicType(sBasicType.Name, !sBasicType.IsMutable, false));
			}

			public CStructDefData VisitFuncPtrInfo(FuncPtrInfo sFuncPtr)
			{
				IEnumerable<CStructDefData> parameterDatas = sFuncPtr.Parameters.Select(p => p.Accept(this));
				CStructDefData returnedData = sFuncPtr.Returned.Accept(this);

				string name = $"func_returned_{returnedData.Name}_params_{string.Join("_", parameterDatas.Select(p => p.Name))}";
				CFuncPtr ctype = new CFuncPtr(returnedData.Contained, parameterDatas.Select(p => p.Contained).ToList());
				return new CStructDefData(name, ctype);
			}

			public CStructDefData VisitPointerInfo(PointerInfo sPointer)
			{
				CStructDefData containedData = sPointer.Contained.Accept(this);
				string name = $"ptr_{containedData.Name}";
				CPointer cptr = new CPointer(containedData.Contained, false);
				return new CStructDefData(name, cptr);
			}

			// References are just pointers here
			public CStructDefData VisitReferenceInfo(ReferenceInfo sReference)
			{
				CStructDefData containedData = sReference.Contained.Accept(this);
				string name = $"ptr_{containedData.Name}";
				CPointer cptr = new CPointer(containedData.Contained, false);
				return new CStructDefData(name, cptr);
			}
		}

		private class CStructDefData
		{
			public readonly string Name;
			public readonly CType Contained;

			public CStructDefData(string name, CType contained)
			{
				Name = name;
				Contained = contained;
			}
		}
	}
}
