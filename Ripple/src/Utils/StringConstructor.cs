using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Utils
{
	class StringConstructor
	{
		private string m_Source = "";
		private string m_Seperator;
		public int IndentCount { get; private set; } = 0;

		public StringConstructor(string seperator)
		{
			m_Seperator = seperator;
		}

		public void PrintLine(string str = "")
		{
			m_Source += GetSeperator() + str + "\n";
		}

		public void TabIn() 
		{ 
			IndentCount++;
		}

		public void TabOut() 
		{ 
			IndentCount--;
		}

		public override string ToString()
		{
			return m_Source;
		}

		private string GetSeperator()
		{
			string seperator = "";
			for (int i = 0; i < IndentCount; i++)
				seperator += m_Seperator;
			return seperator;
		}
	}
}
