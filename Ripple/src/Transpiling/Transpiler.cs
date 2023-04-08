using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils;
using Ripple.Validation.Info.Statements;
using Ripple.Transpiling.C_AST;
using Ripple.Transpiling.ASTConversion;
using Ripple.Transpiling.SourceGeneration;

namespace Ripple.Transpiling
{
	public static class Transpiler
	{
		public const string CORE_TYPE_PREDEFS_FILE = "CORE_TYPE_PREDEFS.h";
		public const string CORE_PREDEFS_FILE = "CORE_PREDEFS.h";

		public static List<CFileInfo> Transpile(TypedProgramStmt programStmt)
        {
			List<CFileStmt> files = ASTConverter.ConvertAST(programStmt);

			var cFiles = files
				.Select(f => new CFileInfo(f.RelativePath, CStatementSourceGenerator.GenerateSource(f), f.FileType))
				.ToList();

			return cFiles;
        }
	}
}
