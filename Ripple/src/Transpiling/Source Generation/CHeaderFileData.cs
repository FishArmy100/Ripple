using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.src.Transpiling.Source_Generation
{
	class CHeaderFileData
	{
		public readonly string FileName;
		public readonly string Source;

		public CHeaderFileData(string fileName, string source)
		{
			FileName = fileName;
			Source = source;
		}
	}
}
