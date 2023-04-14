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
using Raucse;

namespace Ripple.Transpiling.ASTConversion
{
	static class ASTConverter
	{
		public static List<CFileStmt> ConvertAST(TypedProgramStmt program)
        {
            CArrayRegistry registry = new CArrayRegistry();
            TypeConverterVisitor typeConverter = new TypeConverterVisitor(registry);
            StatementConverterVisitor statementConverter = new StatementConverterVisitor(registry, new List<CIncludeStmt>());

            List<CFileStmt> files = program.Files
                .Select(f => f.Accept(statementConverter))
                .SelectMany(f => f)
                .Cast<CFileStmt>()
                .ToList();

            CFileStmt predefs = program.Accept(new PreDeclarationGeneratorVisitor()).GeneratePredefFile(statementConverter, typeConverter, Transpiler.CORE_PREDEFS_FILE);
            CFileStmt typePredefs = GenTypePredefs(registry);
            typePredefs.Includes.Add(new CIncludeStmt("stdbool.h"));
            predefs.Includes.Add(new CIncludeStmt(typePredefs.RelativePath));

            foreach (CFileStmt file in files)
            {
                file.Includes.Add(new CIncludeStmt(predefs.RelativePath));
                file.Includes.Add(new CIncludeStmt(typePredefs.RelativePath));
            }

            files.Add(predefs);
            files.Add(typePredefs);
            return files;
        }

        private static CFileStmt GenTypePredefs(CArrayRegistry registry)
        {
            List<CStatement> statements = registry.GetArrayAliasStructs()
                            .Cast<CStatement>()
                            .ToList();

            CFileStmt typePredefs = new CFileStmt(new List<CIncludeStmt>(), statements, Transpiler.CORE_TYPE_PREDEFS_FILE, CFileType.Header);
            return typePredefs;
        }
    }
}
