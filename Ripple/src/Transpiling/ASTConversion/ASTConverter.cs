using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Transpiling.C_AST;
using Ripple.AST;
using Ripple.Utils;
using Ripple.Transpiling.SourceGeneration;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info.Expressions;
using Ripple.Validation.Info.Statements;

namespace Ripple.Transpiling.ASTConversion
{
	static class ASTConverter
	{
		public static string ConvertAST(TypedProgramStmt program)
		{
			TypedFileStmt file = program.Files[0];
			CArrayRegistry registry = new CArrayRegistry();

			StatementConverterVisitor visitor = new StatementConverterVisitor(registry, new List<CIncludeStmt>());

			CFileStmt cFile = (CFileStmt)file.Accept(visitor)[0];
			string source = CStatementSourceGenerator.GenerateSource(cFile);
			return source;
		}
	}
}
