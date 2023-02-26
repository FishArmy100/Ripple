using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Transpiling
{
	class CSourceBuilder
	{
		private int m_IndentLevel = 0;
		private StringBuilder m_Builder = new StringBuilder();

		public void Append(string s)
		{
			m_Builder.Append(s);
		} 

		public void BeginLine(string s = "")
		{
			m_Builder.AppendLine(new string('\t', m_IndentLevel));
			Append(s);
		}

		public void BeginBlock()
		{
			BeginLine("{");
			m_IndentLevel++;
		}

		public void EndBlock()
		{
			m_IndentLevel--;
			BeginLine("}");
		}

		public override string ToString()
		{
			return m_Builder.ToString();
		}
	}
}
