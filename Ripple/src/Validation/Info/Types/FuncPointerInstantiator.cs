using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils;
using Raucse;

namespace Ripple.Validation.Info.Types
{
	static class FuncPointerInstantiator
	{
		public static TypeInfo InstantiateFunctionPointer(List<LifetimeInfo> replacerLifetimes, FuncPtrInfo funcPtrInfo)
		{
			if (replacerLifetimes.Count != funcPtrInfo.LifetimeCount)
				throw new ArgumentException("Replacer lifetimes count, and the lifetime argument count of the function pointer do not match.");

			return new Visitor(replacerLifetimes, funcPtrInfo.FunctionIndex).VisitFuncPtrInfo(funcPtrInfo);
		}

		private class Visitor : ITypeInfoVisitor<TypeInfo>
		{
			private readonly List<LifetimeInfo> m_ReplacerLifetimes;
			private readonly int m_FunctionIndex;

			public Visitor(List<LifetimeInfo> replacerLifetimes, int functionIndex)
			{
				m_ReplacerLifetimes = replacerLifetimes;
				m_FunctionIndex = functionIndex;
			}

			public TypeInfo VisitArrayInfo(ArrayInfo arrayInfo)
			{
				return new ArrayInfo(arrayInfo.Contained.Accept(this), arrayInfo.Size);
			}

			public TypeInfo VisitBasicTypeInfo(BasicTypeInfo basicTypeInfo)
			{
				return new BasicTypeInfo(basicTypeInfo.Name);
			}

			public TypeInfo VisitFuncPtrInfo(FuncPtrInfo funcPtrInfo)
			{
				if(funcPtrInfo.FunctionIndex == m_FunctionIndex)
				{
					List<TypeInfo> parameters = funcPtrInfo.Parameters
						.Select(p => p.Accept(this))
						.ToList();

					TypeInfo returned = funcPtrInfo.Returned.Accept(this);
					return new FuncPtrInfo(-1, -1, parameters, returned); // shhhhh 
				}
				else
				{
					List<TypeInfo> parameters = funcPtrInfo.Parameters
						.Select(p => p.Accept(this))
						.ToList();

					TypeInfo returned = funcPtrInfo.Returned.Accept(this);
					return new FuncPtrInfo(funcPtrInfo.FunctionIndex, funcPtrInfo.LifetimeCount, parameters, returned);
				}
			}

			public TypeInfo VisitPointerInfo(PointerInfo pointerInfo)
			{
				return new PointerInfo(pointerInfo.IsMutable, pointerInfo.Contained.Accept(this));
			}

			public TypeInfo VisitReferenceInfo(ReferenceInfo referenceInfo)
			{
				TypeInfo contained = referenceInfo.Contained.Accept(this);
				Option<ReferenceLifetime> lifetime = referenceInfo.Lifetime.Match(
					ok =>
					{
						if(ok.IsGeneric)
						{
							GenericLifetime generic = ok.GetGenericLifetime().Value;
							if(generic.FunctionIndex == m_FunctionIndex)
							{
								return new Option<ReferenceLifetime>(new ReferenceLifetime(m_ReplacerLifetimes[generic.LifetimeIndex]));
							}
						}

						return ok;
					},
					() =>
					{
						return new Option<ReferenceLifetime>();
					});

				return new ReferenceInfo(referenceInfo.IsMutable, contained, lifetime);
			}
		}
	}
}
