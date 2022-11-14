using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.Lexing;

namespace Ripple.Transpiling
{
    class TranspilerVisitor : IStatementVisitor<CStatement>
    {
        private readonly string m_FileName;

        public TranspilerVisitor(string fileName)
        {
            m_FileName = fileName;
        }

        public string Transpile(FileStmt fileStmt)
        {
            CStatement.Package package = fileStmt.Accept(this) as CStatement.Package;
            return package.ConvertToCCode(0);
        }

        public CStatement VisitExprStmt(ExprStmt exprStmt)
        {
            CExpression expression = TranspilerExpressionVisitor.Visit(exprStmt.Expr);
            return new CStatement.ExprStmt(expression);
        }

        public CStatement VisitBlockStmt(BlockStmt blockStmt)
        {
            List<CStatement> statements = new List<CStatement>();
            foreach (Statement statement in blockStmt.Statements)
                statements.Add(statement.Accept(this));

            return new CStatement.Block(statements);
        }

        public CStatement VisitIfStmt(IfStmt ifStmt)
        {
            CExpression condition = TranspilerExpressionVisitor.Visit(ifStmt.Expr);
            CStatement body = ifStmt.Body.Accept(this);
            return new CStatement.If(condition, body);
        }

        public CStatement VisitForStmt(ForStmt forStmt)
        {
            CStatement.Var initalizer = null;
            if (forStmt.Init.HasValue() && forStmt.Init.Value is VarDecl v)
                initalizer = v.Accept(this) as CStatement.Var;


            CExpression condition = null;
            if(forStmt.Condition.HasValue())
                condition = TranspilerExpressionVisitor.Visit(forStmt.Condition.Value);

            CExpression iterator = null;
            if (forStmt.Iter.HasValue())
                iterator = TranspilerExpressionVisitor.Visit(forStmt.Iter.Value);

            CStatement body = forStmt.Body.Accept(this);
            
            return new CStatement.For(initalizer, condition, iterator, body);
        }

        public CStatement VisitVarDecl(VarDecl varDecl)
        {
            string typeName = TypeNamePrinter.PrintType(varDecl.Type);
            List<string> varNames = varDecl.VarNames.ConvertAll(t => t.Text);
            CExpression initalizer = TranspilerExpressionVisitor.Visit(varDecl.Expr);

            return new CStatement.Var(typeName, varNames, initalizer);
        }

        public CStatement VisitReturnStmt(ReturnStmt returnStmt)
        {
            return new CStatement.Return(TranspilerExpressionVisitor.Visit(returnStmt.Expr.Value));
        }

        public CStatement VisitParameters(Parameters parameters) // Unused
        {
            throw new NotImplementedException();
        }

        public CStatement VisitFuncDecl(FuncDecl funcDecl)
        {
            string funcName = funcDecl.Name.Text;
            string returnType = TypeNamePrinter.PrintType(funcDecl.ReturnType);
            List<(string, string)> parameters = funcDecl.Param.ParamList.ConvertAll(p => (TypeNamePrinter.PrintType(p.Item1), p.Item2.Text));
            CStatement.Block body = funcDecl.Body.Accept(this) as CStatement.Block;

            return new CStatement.Func(returnType, funcName, parameters, body);
        }

        public CStatement VisitFileStmt(FileStmt fileStmt)
        {
            List<CStatement.Func> functions = new List<CStatement.Func>();
            List<CStatement.Var> variables = new List<CStatement.Var>();

            foreach(Statement statement in fileStmt.Statements)
            {
                CStatement cStatement = statement.Accept(this);
                switch (cStatement)
                {
                    case CStatement.Func f:
                        functions.Add(f);
                        break;
                    case CStatement.Var v:
                        variables.Add(v);
                        break;
                    default:
                        throw new InvalidCastException("Translated statement must be a declaration");
                }
            }

            return new CStatement.Package(m_FileName, functions, variables, new List<string>());
        }

        public CStatement VisitWhileStmt(WhileStmt whileStmt)
        {
            throw new NotImplementedException();
        }

        public CStatement VisitContinueStmt(ContinueStmt continueStmt)
        {
            throw new NotImplementedException();
        }

        public CStatement VisitBreakStmt(BreakStmt breakStmt)
        {
            throw new NotImplementedException();
        }

        public CStatement VisitExternalFuncDecl(ExternalFuncDecl externalFuncDecl)
        {
            throw new NotImplementedException();
        }

        public CStatement VisitProgramStmt(ProgramStmt program)
        {
            throw new NotImplementedException();
        }

        public CStatement VisitGenericParameters(GenericParameters genericParameters)
        {
            throw new NotImplementedException();
        }

        public CStatement VisitWhereClause(WhereClause whereClause)
        {
            throw new NotImplementedException();
        }

        public CStatement VisitUnsafeBlock(UnsafeBlock unsafeBlock)
        {
            throw new NotImplementedException();
        }
    }
}
