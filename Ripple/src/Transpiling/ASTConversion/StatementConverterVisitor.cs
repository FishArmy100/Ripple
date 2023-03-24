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
using Ripple.Validation.Info.Types;

namespace Ripple.Transpiling.ASTConversion
{
    class StatementConverterVisitor : ITypedStatementVisitor<List<CStatement>>
    {
        private readonly ExpressionConverterVisitor m_ExpressionConverter;
        private readonly TypeConverterVisitor m_TypeConverter;
        private readonly List<CIncludeStmt> m_Includes;

        private Stack<int> m_VariableCount = new Stack<int>();

        private bool m_IsGlobal = true;

        public StatementConverterVisitor(CArrayRegistry registry, List<CIncludeStmt> includes)
        {
            m_ExpressionConverter = new ExpressionConverterVisitor(registry, "");
            m_TypeConverter = new TypeConverterVisitor(registry);
            m_Includes = includes;
        }

        public List<CStatement> VisitTypedBlockStmt(TypedBlockStmt typedBlockStmt)
        {
            m_VariableCount.Push(0);
            var statements = typedBlockStmt.Statements
                .Select(s => s.Accept(this))
                .SelectMany(s => s)
                .ToList();

            var returned = ToList(new CBlockStmt(statements));
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
            ExpressionConversionResult result = ConvertExpression(typedExprStmt.Expression);
            List<CStatement> statements = result.GeneratedVariables
                .Cast<CStatement>()
                .Append(new CExprStmt(result.Expression))
                .ToList();

            return statements;
        }

        public List<CStatement> VisitTypedExternalFuncDecl(TypedExternalFuncDecl typedExternalFuncDecl)
        {
            return new List<CStatement>();
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
                    var result = ConvertExpression(ok);
                    return new Pair<CExpression, List<CStatement>>(result.Expression, result.GeneratedVariables.Cast<CStatement>().ToList());
                },
                () => new Option<Pair<CExpression, List<CStatement>>>());

            conditionResult.Match(ok => extraStatements.AddRange(ok.Second));

            Option<Pair<CExpression, List<CStatement>>> iteratorResult = typedForStmt.Condition.Match(
                ok =>
                {
                    var result = ConvertExpression(ok);
                    return new Pair<CExpression, List<CStatement>>(result.Expression, result.GeneratedVariables.Cast<CStatement>().ToList());
                },
                () => new Option<Pair<CExpression, List<CStatement>>>());

            iteratorResult.Match(ok => extraStatements.AddRange(ok.Second));
            CStatement body = GenerateBody(typedForStmt.Body.Accept(this));

            Option<CVarDecl> initalizer = initalizerResult.MatchOrConstruct<Option<CVarDecl>>(ok => ok.First);
            Option<CExpression> condition = conditionResult.MatchOrConstruct<Option<CExpression>>(ok => ok.First);
            Option<CExpression> iterator = iteratorResult.MatchOrConstruct<Option<CExpression>>(ok => ok.First);

            CForStmt cFor = new CForStmt(initalizer, condition, iterator, body);

            m_VariableCount.Pop();

            return extraStatements.Append(cFor).ToList();
        }

        public List<CStatement> VisitTypedFuncDecl(TypedFuncDecl typedFuncDecl)
        {
            m_IsGlobal = false;
            m_VariableCount.Push(0);

            CType returned = ConvertType(typedFuncDecl.Info.ReturnType);
            string name = typedFuncDecl.Info.Name;

            List<CFuncParam> parameters = typedFuncDecl.Info.Parameters
                .Select(p => new CFuncParam(ConvertType(p.Type), p.Name))
                .ToList();

            CBlockStmt body = (CBlockStmt)typedFuncDecl.Body.Accept(this)[0]; // should always be fine


            CFuncDef funcDecl = new CFuncDef(returned, name, parameters, body);

            m_IsGlobal = true;
            m_VariableCount.Pop();
            return ToList(funcDecl);
        }

        public List<CStatement> VisitTypedIfStmt(TypedIfStmt typedIfStmt)
        {
            m_VariableCount.Push(0);
            ExpressionConversionResult conditionResult = ConvertExpression(typedIfStmt.Condition);
            CStatement body = GenerateBody(typedIfStmt.Body.Accept(this));
            Option<CStatement> elseBody = typedIfStmt.ElseBody.MatchOrConstruct<Option<CStatement>>(ok => GenerateBody(ok.Accept(this)));

            CIfStmt cIf = new CIfStmt(conditionResult.Expression, body, elseBody);

            m_VariableCount.Pop();

            return conditionResult.GeneratedVariables
                .Cast<CStatement>()
                .Append(cIf)
                .ToList();
        }

        public List<CStatement> VisitTypedProgramStmt(TypedProgramStmt typedProgramStmt)
        {
            throw new NotImplementedException(); // should never be called
        }

        public List<CStatement> VisitTypedReturnStmt(TypedReturnStmt typedReturnStmt)
        {
            var result = typedReturnStmt.Expression.Match(
                ok => ConvertExpression(ok), 
                () => new Option<ExpressionConversionResult>());

            CReturnStmt returnStmt = new CReturnStmt(result.Match(ok => ok.Expression, () => new Option<CExpression>()));
            return result.Match(
                ok =>
                {
                    return ok.GeneratedVariables
                        .Cast<CStatement>()
                        .Append(returnStmt)
                        .ToList();
                },
                () =>
                {
                    return ToList(returnStmt);
                });
        }

        public List<CStatement> VisitTypedUnsafeBlock(TypedUnsafeBlock typedUnsafeBlock)
        {
            m_VariableCount.Push(0);
            var statements = typedUnsafeBlock.Statements
                .Select(s => s.Accept(this))
                .SelectMany(s => s)
                .ToList();

            var returned = ToList(new CBlockStmt(statements));
            m_VariableCount.Pop();
            return returned;
        }

        public List<CStatement> VisitTypedVarDecl(TypedVarDecl typedVarDecl)
        {
            CType type = ConvertType(typedVarDecl.Type);
            var initalizerResult = ConvertExpression(typedVarDecl.Initalizer);
            List<CVarDecl> generatedVars = new List<CVarDecl>();

            for(int i = 0; i < typedVarDecl.VariableNames.Count; i++)
            {
                string name = typedVarDecl.VariableNames[i];
                if (i == 0)
                {
                    generatedVars.Add(new CVarDecl(type, name, initalizerResult.Expression));
                }
                else
                {
                    generatedVars.Add(new CVarDecl(type, name, new CIdentifier(generatedVars[0].Name)));
                }
            }

            List<CStatement> statements = initalizerResult.GeneratedVariables.Cast<CStatement>().ToList();
            statements.AddRange(generatedVars);
            return statements;
        }

        public List<CStatement> VisitTypedWhileStmt(TypedWhileStmt typedWhileStmt)
        {
            m_VariableCount.Push(0);
            ExpressionConversionResult conditionResult = ConvertExpression(typedWhileStmt.Condition);
            CStatement body = GenerateBody(typedWhileStmt.Body.Accept(this));

            CWhileStmt cWhile = new CWhileStmt(conditionResult.Expression, body);

            m_VariableCount.Pop();

            return conditionResult.GeneratedVariables
                .Cast<CStatement>()
                .Append(cWhile)
                .ToList();
        }

        private static List<CStatement> ToList(params CStatement[] statements)
        {
            return new List<CStatement>(statements);
        }

        private CType ConvertType(TypeInfo type)
        {
            return type.Accept(m_TypeConverter);
        }

        private ExpressionConversionResult ConvertExpression(TypedExpression expression)
		{
            int count = m_VariableCount.Pop();
            m_VariableCount.Push(++count);
            m_ExpressionConverter.VariableSufix = GetTempararyVarPostfix();
            ExpressionConversionResult result = expression.Accept(m_ExpressionConverter);
            return result;
		}

        private static CStatement GenerateBody(List<CStatement> statements)
        {
            return statements.Count switch
            {
                1 => statements[0],
                _ => new CBlockStmt(statements)
            };
        }


        private string GetTempararyVarPostfix()
		{
            return m_VariableCount.Select(c => c.ToString()).Concat("_");
		}
    }
}
