using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Validation.Info.Statements;
using Ripple.Validation.Info.Expressions;
using Ripple.Transpiling.C_AST;
using Ripple.Utils;
using Ripple.Utils.Extensions;

namespace Ripple.Transpiling.ASTConversion
{
    class StatementConverterVisitor : ITypedStatementVisitor<List<CStatement>>
    {
        private readonly ExpressionConverterVisitor m_ExpressionConverter;
        private readonly TypeConverterVisitor m_TypeConverter;
        private readonly List<CIncludeStmt> m_Includes;

        private Stack<int> m_VariableCount = new Stack<int>();

        public StatementConverterVisitor(CArrayRegistry registry, List<CIncludeStmt> includes)
        {
            m_ExpressionConverter = new ExpressionConverterVisitor(registry, "");
            m_TypeConverter = new TypeConverterVisitor(registry);
            m_Includes = includes;
        }

        public List<CStatement> VisitTypedBlockStmt(TypedBlockStmt typedBlockStmt)
        {
            m_VariableCount.Push(0);
            var returned = ToList(new CBlockStmt(typedBlockStmt.Accept(this)));
            m_VariableCount.Pop();
            return returned;
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
            m_ExpressionConverter.VariableSufix = $"var_{m_VariableCount.Peek()}";
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
            m_VariableCount.Push(0);
            var statements = typedFileStmt.Statements
                .Select(s => s.Accept(this))
                .SelectMany(s => s)
                .ToList();

            m_VariableCount.Pop();

            return ToList(new CFileStmt(m_Includes, statements, typedFileStmt.FilePath, CFileType.Source));
        }

        public List<CStatement> VisitTypedForStmt(TypedForStmt typedForStmt)
        {
            m_VariableCount.Push(0);

            Option<Pair<CVarDecl, List<CStatement>>> initalizerResult = typedForStmt.Initalizer.Match(
                ok =>
                {
                    List<CStatement> statements = ok.Accept(this);
                    CVarDecl var = (CVarDecl)statements[statements.Count - 1];
                    statements.RemoveAt(statements.Count - 1);
                    return new Pair<CVarDecl, List<CStatement>>(var, statements);
                }, 
                () => new Option<Pair<CVarDecl, List<CStatement>>>());

            List<CStatement> extraStatements = initalizerResult.Match(ok => ok.Second, () => new List<CStatement>());

            Option<Pair<CExpression, List<CStatement>>> conditionResult = typedForStmt.Condition.Match(
                ok =>
                {

                    var result = ok.Accept(m_ExpressionConverter);
                    return new Pair<CExpression, List<CStatement>>(result.Expression, result.GeneratedVariables.Cast<CStatement>().ToList());
                },
                () => new Option<Pair<CExpression, List<CStatement>>>());
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

        private ExpressionConversionResult VisitExpression(TypedExpression expression)
		{
            ExpressionConversionResult result = expression.Accept(m_ExpressionConverter);
            int count = m_VariableCount.Pop();
            m_VariableCount.Push(++count);
            m_ExpressionConverter.VariableSufix = GetPostfix();
            return result;
		}

        private string GetPostfix()
		{
            return m_VariableCount.Select(c => c.ToString()).Concat("_");
		}
    }
}
