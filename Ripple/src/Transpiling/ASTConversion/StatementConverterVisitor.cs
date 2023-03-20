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
    class StatementConverterVisitor : ITypedStatementVisitor<CStatement>
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

        public CStatement VisitTypedBlockStmt(TypedBlockStmt typedBlockStmt)
        {
            List<CStatement> statements = typedBlockStmt.Statements.Select(s => s.Accept(this)).ToList();
            return new CBlockStmt(statements);
        }

        public CStatement VisitTypedBreakStmt(TypedBreakStmt typedBreakStmt)
        {
            return ToList(new CBreakStmt());
        }

        public CStatement VisitTypedContinueStmt(TypedContinueStmt typedContinueStmt)
        {
            return new CContinueStmt();
        }

        public CStatement VisitTypedExprStmt(TypedExprStmt typedExprStmt)
        {
            CExpression expression = typedExprStmt.Expression.Accept(m_ExpressionConverter).Expression;
            return new CExprStmt(expression);
        }

        public CStatement VisitTypedExternalFuncDecl(TypedExternalFuncDecl typedExternalFuncDecl)
        {
            throw new NotImplementedException();
        }

        public CStatement VisitTypedFileStmt(TypedFileStmt typedFileStmt)
        {
            var statements = typedFileStmt.Statements
                .Select(s => s.Accept(this))
                .ToList();

            return new CFileStmt(m_Includes, statements, typedFileStmt.FilePath, CFileType.Source);
        }

        public CStatement VisitTypedForStmt(TypedForStmt typedForStmt)
        {
            throw new NotImplementedException();
        }

        public CStatement VisitTypedFuncDecl(TypedFuncDecl typedFuncDecl)
        {
            throw new NotImplementedException();
        }

        public CStatement VisitTypedIfStmt(TypedIfStmt typedIfStmt)
        {
            throw new NotImplementedException();
        }

        public CStatement VisitTypedProgramStmt(TypedProgramStmt typedProgramStmt)
        {
            throw new NotImplementedException(); // Should never be called
        }

        public CStatement VisitTypedReturnStmt(TypedReturnStmt typedReturnStmt)
        {
            throw new NotImplementedException();
        }

        public CStatement VisitTypedUnsafeBlock(TypedUnsafeBlock typedUnsafeBlock)
        {
            throw new NotImplementedException();
        }

        public CStatement VisitTypedVarDecl(TypedVarDecl typedVarDecl)
        {
            throw new NotImplementedException();
        }

        public CStatement VisitTypedWhileStmt(TypedWhileStmt typedWhileStmt)
        {
            throw new NotImplementedException();
        }

        private static List<CStatement> ToList(params CStatement[] statements)
        {
            return new List<CStatement>(statements);
        }
    }
}
