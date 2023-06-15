using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils;
using Raucse;
using Raucse.Extensions;
using Ripple.Validation.Info.Lifetimes;

namespace Ripple.Validation.Info.Types
{
	class GenericLifetimeValueEvaluator : ITypeInfoVisitor<Dictionary<int, List<LifetimeInfo>>, TypeInfo>
	{
		public readonly int FunctionIndexToGetLifetimesFor;

		public GenericLifetimeValueEvaluator(int functionIndexToGetLifetimesFor)
		{
			FunctionIndexToGetLifetimesFor = functionIndexToGetLifetimesFor;
		}

		/// <summary>
		/// Takes a function parameter type, and a function argument type, with a function index, to finds all the lifetime infos for the given generic lifetimes in the parameter type
		/// </summary>
		/// <param name="functionParameter"></param>
		/// <param name="functionArgument"></param>
		/// <param name="functionIndexToGetLifetimesFor"></param>
		/// <returns></returns>
		public static Option<Dictionary<int, List<LifetimeInfo>>> GetGenericLifetimeValues(TypeInfo functionParameter, TypeInfo functionArgument, int functionIndexToGetLifetimesFor)
		{
			try
			{
				return functionParameter.Accept(new GenericLifetimeValueEvaluator(functionIndexToGetLifetimesFor), functionArgument);
			}
			catch(GenericLifetimeValueEvaluatorExeption)
			{
				return new Option<Dictionary<int, List<LifetimeInfo>>>();
			}
		}

		public Dictionary<int, List<LifetimeInfo>> VisitArrayInfo(ArrayInfo arrayInfo, TypeInfo arg)
		{
			if(arg is ArrayInfo array)
				return arrayInfo.Contained.Accept(this, array.Contained);

			throw new GenericLifetimeValueEvaluatorExeption();
		}

		public Dictionary<int, List<LifetimeInfo>> VisitBasicTypeInfo(BasicTypeInfo basicTypeInfo, TypeInfo arg)
		{
			if (arg is BasicTypeInfo)
				return new Dictionary<int, List<LifetimeInfo>>();

			throw new GenericLifetimeValueEvaluatorExeption();
		}

		public Dictionary<int, List<LifetimeInfo>> VisitFuncPtrInfo(FuncPtrInfo funcPtrInfo, TypeInfo arg)
		{
			Dictionary<int, List<LifetimeInfo>> lifetimeValues = new Dictionary<int, List<LifetimeInfo>>();
			if(arg is FuncPtrInfo other)
			{
				var returnedValues = funcPtrInfo.Returned.Accept(this, other.Returned);
				foreach((int key, List<LifetimeInfo> lifetimes) in returnedValues)
				{
					lifetimeValues.GetOrCreate(key).AddRange(lifetimes);
				}

				var parameterLifetimeValuesList = funcPtrInfo.Parameters.Zip(other.Parameters).Select((t) =>
				{
					(var p, var a) = t;
					return p.Accept(this, a);
				});

				foreach(var paramLifetimesValues in parameterLifetimeValuesList)
				{
					foreach ((int key, List<LifetimeInfo> lifetimes) in paramLifetimesValues)
					{
						lifetimeValues.GetOrCreate(key).AddRange(lifetimes);
					}
				}

				return lifetimeValues;
			}

			throw new GenericLifetimeValueEvaluatorExeption();
		}

		public Dictionary<int, List<LifetimeInfo>> VisitPointerInfo(PointerInfo pointerInfo, TypeInfo arg)
		{
			if (arg is PointerInfo ptr)
				return pointerInfo.Contained.Accept(this, ptr.Contained);

			throw new GenericLifetimeValueEvaluatorExeption();
		}

		public Dictionary<int, List<LifetimeInfo>> VisitReferenceInfo(ReferenceInfo referenceInfo, TypeInfo arg)
		{
			if(arg is ReferenceInfo other)
			{
				var containedLifetimeValues = referenceInfo.Contained.Accept(this, other.Contained);

				return referenceInfo.Lifetime.Match(ok =>
				{
					if(ok.IsGeneric && 
					   ok.GetGenericLifetime().Value.FunctionIndex == FunctionIndexToGetLifetimesFor &&
					   other.Lifetime.HasValue() &&
					   other.Lifetime.Value.IsLifetimeInfo)
					{
						containedLifetimeValues.GetOrCreate(ok.GetGenericLifetime().Value.LifetimeIndex)
							.Add(other.Lifetime.Value.GetLifetimeInfo().Value);
					}

					return containedLifetimeValues;
				},
				() =>
				{
					return containedLifetimeValues;
				});
			}

			throw new GenericLifetimeValueEvaluatorExeption();
		}

		private  class GenericLifetimeValueEvaluatorExeption : Exception { }
	}
}
