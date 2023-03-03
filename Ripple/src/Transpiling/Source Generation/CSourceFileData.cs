using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.src.Transpiling.Source_Generation
{
	class CSourceFileData
	{
		public readonly string FileName;
		public readonly string Source;

		public CSourceFileData(string fileName, string source)
		{
			FileName = fileName;
			Source = source;
		}
	}
}
