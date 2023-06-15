using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Validation.Info.Lifetimes
{
	public struct GenericLifetime
	{
		public readonly int LifetimeIndex;
		public readonly int FunctionIndex;

		public GenericLifetime(int lifetimeIndex, int functionIndex)
		{
			LifetimeIndex = lifetimeIndex;
			FunctionIndex = functionIndex;
		}

		public override bool Equals(object obj)
		{
			return obj is GenericLifetime lifetime &&
				   LifetimeIndex == lifetime.LifetimeIndex &&
				   FunctionIndex == lifetime.FunctionIndex;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(LifetimeIndex, FunctionIndex);
		}
	}
}
