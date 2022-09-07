using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    abstract class AstWalkerBase : IAstVisitor
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


        public virtual void VisitCall(Call call)
        {
            call.Expr.Accept(this);
            foreach (Expression expression in call.Args)
                expression.Accept(this);
        }

        public virtual void VisitExprStmt(ExprStmt exprStmt)
        {
            exprStmt.Expr.Accept(this);
        }

        public virtual void VisitFileStmt(FileStmt fileStmt)
        {
            foreach (Statement statement in fileStmt.Statements)
                statement.Accept(this);
        }

        public virtual void VisitForStmt(ForStmt forStmt)
        {
            forStmt.Init.Accept(this);
            forStmt.Condition.Accept(this);
            forStmt.Iter.Accept(this);
            forStmt.Body.Accept(this);
        }

        public virtual void VisitFuncDecl(FuncDecl funcDecl)
        {
            funcDecl.Param.Accept(this);
            funcDecl.Body.Accept(this);
        }


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

        public virtual void VisitLiteral(Literal literal) { }

        public virtual void VisitParameters(Parameters parameters) { }

        public virtual void VisitReturnStmt(ReturnStmt returnStmt)
        {
            returnStmt.Expr.Accept(this);
        }

        public virtual void VisitUnary(Unary unary)
        {
            unary.Expr.Accept(this);
        }

        public virtual void VisitVarDecl(VarDecl varDecl)
        {
            varDecl.Expr.Accept(this);
        }
    }
}
