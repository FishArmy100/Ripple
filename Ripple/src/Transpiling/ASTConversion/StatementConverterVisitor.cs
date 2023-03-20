using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Validation.Info.Statements;
using Ripple.Transpiling.C_AST;
using Ripple.Utils;

namespace Ripple.Transpiling.ASTConversion
{
    class StatementConverterVisitor : ITypedStatementVisitor<List<CStatement>>
    {
        private readonly ExpressionConverterVisitor m_ExpressionConverter;
        private readonly TypeConverterVisitor m_TypeConverter;
        private readonly List<CIncludeStmt> m_Includes;

        public StatementConverterVisitor(CArrayRegistry registry, List<CIncludeStmt> includes)
        {
            m_ExpressionConverter = new ExpressionConverterVisitor(registry, "");
            m_TypeConverter = new TypeConverterVisitor(registry);
            m_Includes = includes;
        }

        public List<CStatement> VisitTypedBlockStmt(TypedBlockStmt typedBlockStmt)
        {
            return ToList(new CBlockStmt(typedBlockStmt.Accept(this)));
        }

        public List<CStatement> VisitTypedBreakStmt(TypedBreakStmt typedBreakStmt)
        {
            return ToList(new CBreakStmt());
        }

        public List<CStatement> VisitTypedContinueStmt(TypedContinueStmt typedContinueStmt)
        {
            return ToList(new CContinueStmt());
        }

        public List<CStatement> VisitTypedExprStmt(TypedExprStmt typedExprStmt)
        {
            ExpressionConversionResult result = typedExprStmt.Expression.Accept(m_ExpressionConverter);
            List<CStatement> statements = result.GeneratedVariables
                .Cast<CStatement>()
                .Append(new CExprStmt(result.Expression))
                .ToList();

            return statements;
        }

        public List<CStatement> VisitTypedExternalFuncDecl(TypedExternalFuncDecl typedExternalFuncDecl)
        {
            throw new NotImplementedException();
        }

        public List<CStatement> VisitTypedFileStmt(TypedFileStmt typedFileStmt)
        {
            var statements = typedFileStmt.Statements
                .Select(s => s.Accept(this))
                .SelectMany(s => s)
                .ToList();

            return ToList(new CFileStmt(m_Includes, statements, typedFileStmt.FilePath, CFileType.Source));
        }

        public List<CStatement> VisitTypedForStmt(TypedForStmt typedForStmt)
        {
            throw new NotImplementedException();
        }

        public List<CStatement> VisitTypedFuncDecl(TypedFuncDecl typedFuncDecl)
        {
            throw new NotImplementedException();
        }

        public List<CStatement> VisitTypedIfStmt(TypedIfStmt typedIfStmt)
        {
            throw new NotImplementedException();
        }

        public List<CStatement> VisitTypedProgramStmt(TypedProgramStmt typedProgramStmt)
        {
            throw new NotImplementedException();
        }

        public List<CStatement> VisitTypedReturnStmt(TypedReturnStmt typedReturnStmt)
        {
            throw new NotImplementedException();
        }

        public List<CStatement> VisitTypedUnsafeBlock(TypedUnsafeBlock typedUnsafeBlock)
        {
            throw new NotImplementedException();
        }

        public List<CStatement> VisitTypedVarDecl(TypedVarDecl typedVarDecl)
        {
            throw new NotImplementedException();
        }

        public List<CStatement> VisitTypedWhileStmt(TypedWhileStmt typedWhileStmt)
        {
            throw new NotImplementedException();
        }

        private static List<CStatement> ToList(params CStatement[] statements)
        {
            return new List<CStatement>(statements);
        }
    }
}
