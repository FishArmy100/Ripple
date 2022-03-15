using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{

    interface IStatementVisitor
    {
        public void VisitExpressionStmt(ExpressionStmt expressionStmt);
        public void VisitBlock(BlockStmt block);
        public void VisitIfStmt(IfStmt ifStmt);
        public void VisitWhileLoop(WhileLoopStmt whileLoop);
        public void VisitForLoop(ForLoopStmt forLoop);
        public void VisitContinueStmt(ContinueStmt continueStmt);
        public void VisitBreakStmt(BreakStmt breakStmt);
        public void VisitReturnStmt(ReturnStmt returnStmt);
        // declarations
        public void VisitVarDeclaration(VariableDecl variableDecl);
        public void VisitFuncDeclaration(FunctionDecl funcDeclaration);
        public void VisitClassDeclaration(ClassDecl classDecl);
        // members
        public void VisitConstructorDecl(ConstructorDecl constructorDecl);
        public void VisitMemberDeclaration(MemberDeclarationStmt memberDeclaration);
    }

    interface IStatementVisitor<T>
    {
        public T VisitExpressionStmt(ExpressionStmt expressionStmt);
        public T VisitBlock(BlockStmt block);
        public T VisitIfStmt(IfStmt ifStmt);
        public T VisitWhileLoop(WhileLoopStmt whileLoop);
        public T VisitForLoop(ForLoopStmt forLoop);
        public T VisitContinueStmt(ContinueStmt continueStmt);
        public T VisitBreakStmt(BreakStmt breakStmt);
        public T VisitReturnStmt(ReturnStmt returnStmt);
        // declarations
        public T VisitVarDeclaration(VariableDecl variable);
        public T VisitFuncDeclaration(FunctionDecl funcDeclaration);
        public T VisitClassDeclaration(ClassDecl classDecl);
        // members
        public T VisitConstructorDecl(ConstructorDecl constructorDecl);
        public T VisitMemberDeclaration(MemberDeclarationStmt memberDeclaration);
    }
}
