using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    abstract class Statement
    {
        public abstract void Accept(IStatementVisitor visitor);
        public abstract T Accept<T>(IStatementVisitor<T> visitor);

        public class ExpressionStmt : Statement
        {
            public readonly Expression Expr;

            public ExpressionStmt(Expression expr)
            {
                Expr = expr;
            }

            public override void Accept(IStatementVisitor visitor)
            {
                visitor.VisitExpressionStmt(this);
            }

            public override T Accept<T>(IStatementVisitor<T> visitor)
            {
                return visitor.VisitExpressionStmt(this);
            }
        }

        public class Block : Statement
        {
            public readonly List<Statement> Statements;

            public Block(List<Statement> statements)
            {
                Statements = statements;
            }

            public override void Accept(IStatementVisitor visitor)
            {
                visitor.VisitBlock(this);
            }

            public override T Accept<T>(IStatementVisitor<T> visitor)
            {
                return visitor.VisitBlock(this);
            }
        }

        public class IfStmt : Statement
        {
            public readonly Expression Condition;
            public readonly Statement ThenBranch;
            public readonly Statement ElseBranch;

            public IfStmt(Expression condition, Statement thenBranch, Statement elseBranch)
            {
                Condition = condition;
                ThenBranch = thenBranch;
                ElseBranch = elseBranch;
            }

            public override void Accept(IStatementVisitor visitor)
            {
                visitor.VisitIfStmt(this);
            }

            public override T Accept<T>(IStatementVisitor<T> visitor)
            {
                return visitor.VisitIfStmt(this);
            }
        }

        public class WhileLoop : Statement
        {
            public readonly Expression Condition;
            public readonly Statement Body;

            public WhileLoop(Expression condition, Statement body)
            {
                Condition = condition;
                Body = body;
            }

            public override void Accept(IStatementVisitor visitor)
            {
                visitor.VisitWhileLoop(this);
            }

            public override T Accept<T>(IStatementVisitor<T> visitor)
            {
                return visitor.VisitWhileLoop(this);
            }
        }

        public class ForLoop : Statement
        {
            public readonly Statement Initializer;
            public readonly Expression Condition;
            public readonly Expression Incrementer;
            public readonly Statement Body;

            public ForLoop(Statement initializer, Expression condition, Expression incrementer, Statement body)
            {
                Initializer = initializer;
                Condition = condition;
                Incrementer = incrementer;
                Body = body;
            }

            public override void Accept(IStatementVisitor visitor)
            {
                visitor.VisitForLoop(this);
            }

            public override T Accept<T>(IStatementVisitor<T> visitor)
            {
                return visitor.VisitForLoop(this);
            }
        }

        public class ContinueStmt : Statement
        {
            public readonly Token ContinueToken;

            public ContinueStmt(Token continueToken)
            {
                ContinueToken = continueToken;
            }

            public override void Accept(IStatementVisitor visitor)
            {
                visitor.VisitContinueStmt(this);
            }

            public override T Accept<T>(IStatementVisitor<T> visitor)
            {
                return visitor.VisitContinueStmt(this);
            }
        }

        public class BreakStmt : Statement
        {
            public readonly Token BreakToken;

            public BreakStmt(Token breakToken)
            {
                BreakToken = breakToken;
            }

            public override void Accept(IStatementVisitor visitor)
            {
                visitor.VisitBreakStmt(this);
            }

            public override T Accept<T>(IStatementVisitor<T> visitor)
            {
                return visitor.VisitBreakStmt(this);
            }
        }

        public class ReturnStmt : Statement
        {
            public readonly Token ReturnToken;
            public readonly Expression ReturnExpression;

            public ReturnStmt(Token returnToken, Expression returnExpression)
            {
                ReturnToken = returnToken;
                ReturnExpression = returnExpression;
            }

            public override void Accept(IStatementVisitor visitor)
            {
                visitor.VisitReturnStmt(this);
            }

            public override T Accept<T>(IStatementVisitor<T> visitor)
            {
                return visitor.VisitReturnStmt(this);
            }
        }

        public abstract class Declaration : Statement { }

        public class VarDeclaration : Declaration
        {
            public readonly Token TypeName;
            public readonly Token Name;
            public readonly Expression Initializer;

            public VarDeclaration(Token typeName, Token name, Expression initializer)
            {
                TypeName = typeName;
                Name = name;
                Initializer = initializer;
            }

            public override void Accept(IStatementVisitor visitor)
            {
                visitor.VisitVarDeclaration(this);
            }

            public override T Accept<T>(IStatementVisitor<T> visitor)
            {
                return visitor.VisitVarDeclaration(this);
            }
        }

        public class FuncDeclaration : Declaration
        {
            public readonly Token ReturnType;
            public readonly Token Name;
            public readonly List<Tuple<Token, Token>> Parameters;
            public readonly Block Body;

            public FuncDeclaration(Token returnType, Token name, List<Tuple<Token, Token>> parameters, Block body)
            {
                ReturnType = returnType;
                Name = name;
                Parameters = parameters;
                Body = body;
            }

            public override void Accept(IStatementVisitor visitor)
            {
                visitor.VisitFuncDeclaration(this);
            }

            public override T Accept<T>(IStatementVisitor<T> visitor)
            {
                return visitor.VisitFuncDeclaration(this);
            }
        }


        public interface IStatementVisitor
        {
            public void VisitExpressionStmt(ExpressionStmt expressionStmt);
            public void VisitVarDeclaration(VarDeclaration variable);
            public void VisitFuncDeclaration(FuncDeclaration funcDeclaration);
            public void VisitBlock(Block block);
            public void VisitIfStmt(IfStmt ifStmt);
            public void VisitWhileLoop(WhileLoop whileLoop);
            public void VisitForLoop(ForLoop forLoop);
            public void VisitContinueStmt(ContinueStmt continueStmt);
            public void VisitBreakStmt(BreakStmt breakStmt);
            public void VisitReturnStmt(ReturnStmt returnStmt);
        }

        public interface IStatementVisitor<T>
        {
            public T VisitExpressionStmt(ExpressionStmt expressionStmt);
            public T VisitVarDeclaration(VarDeclaration variable);
            public T VisitFuncDeclaration(FuncDeclaration funcDeclaration);
            public T VisitBlock(Block block);
            public T VisitIfStmt(IfStmt ifStmt);
            public T VisitWhileLoop(WhileLoop whileLoop);
            public T VisitForLoop(ForLoop forLoop);
            public T VisitContinueStmt(ContinueStmt continueStmt);
            public T VisitBreakStmt(BreakStmt breakStmt);
            public T VisitReturnStmt(ReturnStmt returnStmt);
        }
    }
}
