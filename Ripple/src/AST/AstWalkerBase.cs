using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    abstract class AstWalkerBase : IAstVisitor
    {
        public void VisitArrayType(ArrayType arrayType)
        {
            throw new NotImplementedException();
        }

        public void VisitBasicType(BasicType basicType)
        {
            throw new NotImplementedException();
        }

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

        public void VisitBreakStmt(BreakStmt breakStmt)
        {
            throw new NotImplementedException();
        }

        public virtual void VisitCall(Call call)
        {
            foreach (Expression expression in call.Args)
                expression.Accept(this);
        }

        public void VisitCast(Cast cast)
        {
            throw new NotImplementedException();
        }

        public void VisitContinueStmt(ContinueStmt continueStmt)
        {
            throw new NotImplementedException();
        }

        public virtual void VisitExprStmt(ExprStmt exprStmt)
        {
            exprStmt.Expr.Accept(this);
        }

        public void VisitExternalFuncDecl(ExternalFuncDecl externalFuncDecl)
        {
            throw new NotImplementedException();
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

        public void VisitFuncPtr(FuncPtr funcPtr)
        {
            throw new NotImplementedException();
        }

        public void VisitGroupedType(GroupedType groupedType)
        {
            throw new NotImplementedException();
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

        public void VisitIndex(Index index)
        {
            throw new NotImplementedException();
        }

        public void VisitInitializerList(InitializerList initializerList)
        {
            throw new NotImplementedException();
        }

        public virtual void VisitLiteral(Literal literal) { }

        public virtual void VisitParameters(Parameters parameters) { }

        public void VisitPointerType(PointerType pointerType)
        {
            throw new NotImplementedException();
        }

        public void VisitProgram(Program program)
        {
            throw new NotImplementedException();
        }

        public void VisitReferenceType(ReferenceType referenceType)
        {
            throw new NotImplementedException();
        }

        public virtual void VisitReturnStmt(ReturnStmt returnStmt)
        {
            if(returnStmt.Expr != null)
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

        public void VisitWhileStmt(WhileStmt whileStmt)
        {
            throw new NotImplementedException();
        }
    }
}
