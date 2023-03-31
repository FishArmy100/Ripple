using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils;
using Raucse;

namespace Ripple.Validation.Info
{
	class ReferenceLifetime
	{
		private readonly Either<GenericLifetime, LifetimeInfo> m_LifetimeInfo;

		public ReferenceLifetime(GenericLifetime lifetime)
		{
			m_LifetimeInfo = new Either<GenericLifetime, LifetimeInfo>(lifetime);
		}

		public ReferenceLifetime(LifetimeInfo lifetime)
		{
			m_LifetimeInfo = new Either<GenericLifetime, LifetimeInfo>(lifetime);
		}

		public bool IsGeneric => m_LifetimeInfo.IsOptionA;
		public bool IsLifetimeInfo => m_LifetimeInfo.IsOptionB;

		public override bool Equals(object obj)
		{
			return obj is ReferenceLifetime lifetime &&
				   EqualityComparer<Either<GenericLifetime, LifetimeInfo>>.Default.Equals(m_LifetimeInfo, lifetime.m_LifetimeInfo);
		}

		public override string ToString()
		{
			return m_LifetimeInfo.Match(a => "", b => b.ToString());
		}

		public Option<GenericLifetime> GetGenericLifetime() => m_LifetimeInfo.IsOptionA ? new Option<GenericLifetime>(m_LifetimeInfo.AValue) : new Option<GenericLifetime>();

		public override int GetHashCode()
		{
			return HashCode.Combine(m_LifetimeInfo);
		}

		public Option<LifetimeInfo> GetLifetimeInfo() => m_LifetimeInfo.IsOptionB ? new Option<LifetimeInfo>(m_LifetimeInfo.BValue) : new Option<LifetimeInfo>();
	}
}
