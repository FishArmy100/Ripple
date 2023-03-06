using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Transpiling.C_AST
{
	class CProgram
	{
		public readonly List<CFileStmt> Files;

		public IEnumerable<CFileStmt> Headers => Files.Where(f => f.FileType == CFileType.Header);
		public IEnumerable<CFileStmt> Sources => Files.Where(f => f.FileType == CFileType.Source);

		public CProgram(List<CFileStmt> files)
		{
			Files = files;
		}
	}
}
