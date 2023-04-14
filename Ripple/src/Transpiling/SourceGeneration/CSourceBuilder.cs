using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Transpiling.SourceGeneration
{
	class CSourceBuilder
	{
		private int m_IndentLevel = 0;
		private readonly StringBuilder m_Builder = new StringBuilder();

		public void AppendLine(string src = "")
		{
			m_Builder.AppendLine(new string('\t', m_IndentLevel) + src);
		}

		public void TabRight() { m_IndentLevel++; }
		public void TabLeft() { m_IndentLevel--; }

		public void BeginBlock()
		{
			AppendLine("{");
			m_IndentLevel++;
		}

		public void EndBlock()
		{
			m_IndentLevel--;
			AppendLine("}");
		}

		public void EndBlockWithSemicolon()
        {
			m_IndentLevel--;
			AppendLine("};");
        }

		public override string ToString()
		{
			return m_Builder.ToString();
		}
	}
}
