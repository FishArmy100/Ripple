using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    abstract class AstWalkerBase : IStatementVisitor, IExpressionVisitor
    {
        public virtual void VisitBinary(Binary binary)
        {
            binary.Left.Accept(this);
            binary.Right.Accept(this);
        }

        public virtual void VisitBlockStmt(BlockStmt blockStmt)
        {
            foreach (Statement statement in blockStmt.Statements)
                statement.Accept(this);
        }

        public virtual void VisitBreakStmt(BreakStmt breakStmt) { }

        public virtual void VisitCall(Call call)
        {
            foreach (Expression expression in call.Args)
                expression.Accept(this);
        }

        public virtual void VisitCast(Cast cast)
        {
            cast.Castee.Accept(this);
        }

        public virtual void VisitContinueStmt(ContinueStmt continueStmt) { }

        public virtual void VisitExprStmt(ExprStmt exprStmt)
        {
            exprStmt.Expr.Accept(this);
        }

        public virtual void VisitExternalFuncDecl(ExternalFuncDecl externalFuncDecl) { }

        public virtual void VisitFileStmt(FileStmt fileStmt)
        {
            foreach (Statement statement in fileStmt.Statements)
                statement.Accept(this);
        }

        public virtual void VisitForStmt(ForStmt forStmt)
        {
            forStmt.Init.Match(init => init.Accept(this));
            forStmt.Condition.Match(con => con.Accept(this));
            forStmt.Iter.Match(iter => iter.Accept(this));
            forStmt.Body.Accept(this);
        }

        public virtual void VisitFuncDecl(FuncDecl funcDecl)
        {
            funcDecl.Param.Accept(this);
            funcDecl.Body.Accept(this);
        }

        public virtual void VisitGenericParameters(GenericParameters genericParameters) { }

        public virtual void VisitGrouping(Grouping grouping)
        {
            grouping.Expr.Accept(this);
        }

        public virtual void VisitIdentifier(Identifier identifier) { }

        public virtual void VisitIfStmt(IfStmt ifStmt)
        {
            ifStmt.Expr.Accept(this);
            ifStmt.Body.Accept(this);
        }

        public virtual void VisitIndex(Index index)
        {
            index.Indexed.Accept(this);
            index.Argument.Accept(this);
        }

        public virtual void VisitInitializerList(InitializerList initializerList)
        {
            foreach (Expression expr in initializerList.Expressions)
                expr.Accept(this);
        }

        public virtual void VisitLiteral(Literal literal) { }

        public virtual void VisitParameters(Parameters parameters) { }

        public virtual void VisitProgramStmt(ProgramStmt program)
        {
            foreach (FileStmt file in program.Files)
                file.Accept(this);
        }

        public virtual void VisitReturnStmt(ReturnStmt returnStmt)
        {
            if(returnStmt.Expr.HasValue())
                returnStmt.Expr.Value.Accept(this);
        }

        public void VisitSizeOf(SizeOf sizeOf) { }

        public virtual void VisitTypeExpression(TypeExpression typeExpression) { }

        public virtual void VisitUnary(Unary unary)
        {
            unary.Expr.Accept(this);
        }

        public virtual void VisitUnsafeBlock(UnsafeBlock unsafeBlock)
        {
            foreach (Statement statement in unsafeBlock.Statements)
                statement.Accept(this);
        }

        public virtual void VisitVarDecl(VarDecl varDecl)
        {
            varDecl.Expr.Accept(this);
        }

        public virtual void VisitWhereClause(WhereClause whereClause)
        {
            whereClause.Expression.Accept(this);
        }

        public virtual void VisitWhileStmt(WhileStmt whileStmt)
        {
            whileStmt.Condition.Accept(this);
            whileStmt.Body.Accept(this);
        }
    }
}
