using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    class AstPrinter : IStatementVisitor, IExpressionVisitor
    {
        private int m_Index = 0;
        private readonly string m_Seperator;

        public AstPrinter(string seperator)
        {
            m_Seperator = seperator;
        }

        public void PrintAst(Expression expr)
        {
            expr.Accept(this);
            m_Index = 0;
        }

        public void PrintAst(Statement statement)
        {
            statement.Accept(this);
            m_Index = 0;
        }

        public void VisitExprStmt(ExprStmt exprStmt)
        {
            Print("Expression Statement: ");
            TabRight();
            exprStmt.Expr.Accept(this);
            TabLeft();
        }

        public void VisitBlockStmt(BlockStmt blockStmt)
        {
            Print("Block Statement:");
            TabRight();
            foreach (Statement statement in blockStmt.Statements)
                statement.Accept(this);
            TabLeft();
        }

        public void VisitIfStmt(IfStmt ifStmt)
        {
            Print("If Statement");
            TabRight();

            Print("Condition:");
            TabRight();
            ifStmt.Expr.Accept(this);
            TabLeft();

            Print("Main Branch:");
            TabRight();
            ifStmt.Body.Accept(this);
            TabLeft();

            if(ifStmt.ElseToken.HasValue)
            {
                Print("Else Branch");
                TabRight();
                ifStmt.ElseBody.Accept(this);
                TabLeft();
            }

            TabLeft();
        }

        public void VisitForStmt(ForStmt forStmt)
        {
            Print("For Statement");
            TabRight();

            if(forStmt.Init != null)
            {
                Print("Initalization:");
                TabRight();
                forStmt.Init.Accept(this);
                TabLeft();
            }

            if(forStmt.Condition != null)
            {
                Print("Condition:");
                TabRight();
                forStmt.Condition.Accept(this);
                TabLeft();
            }

            if(forStmt.Iter != null)
            {
                Print("Iterator:");
                TabRight();
                forStmt.Iter.Accept(this);
                TabLeft();
            }

            Print("Body:");
            TabRight();
            forStmt.Body.Accept(this);
            TabLeft();

            TabLeft();
        }

        public void VisitVarDecl(VarDecl varDecl)
        {
            Print("Variable Declaration:");
            TabRight();
            Print("Type: " + TypeNamePrinter.PrintType(varDecl.Type));

            string names = "";
            for(int i = 0; i < varDecl.VarNames.Count; i++)
            {
                if (i != 0)
                    names += ", ";
                names += varDecl.VarNames[i];
            }

            Print("Var Names: " + names);

            Print("Initializer: ");
            TabRight();
            varDecl.Expr.Accept(this);
            TabLeft();

            TabLeft();
        }

        public void VisitReturnStmt(ReturnStmt returnStmt)
        {
            Print("Return Statement:");
            if(returnStmt.Expr != null)
            {
                TabRight();
                returnStmt.Expr.Accept(this);
                TabLeft();
            }
        }

        public void VisitParameters(Parameters parameters)
        {
            Print("Parameters: ");
            TabRight();
            foreach (var pair in parameters.ParamList)
                Print(TypeNamePrinter.PrintType(pair.Item1) + " " + pair.Item2.Text);
            TabLeft();
        }

        public void VisitFuncDecl(FuncDecl funcDecl)
        {
            Print("Function Declaration:");
            TabRight();
            funcDecl.Param.Accept(this);
            Print("Return type: " + TypeNamePrinter.PrintType(funcDecl.ReturnType));
            funcDecl.Body.Accept(this);
            TabLeft();
        }

        public void VisitFileStmt(FileStmt fileStmt)
        {
            Print("File:");
            TabRight();
            foreach (Statement statement in fileStmt.Statements)
                statement.Accept(this);
            TabLeft();
        }

        public void VisitWhileStmt(WhileStmt whileStmt)
        {
            Print("While Loop:");
            TabRight();

            Print("Condition:");
            TabRight();
            whileStmt.Condition.Accept(this);
            TabLeft();

            Print("Body:");
            TabRight();
            whileStmt.Body.Accept(this);
            TabLeft();

            TabLeft();
        }

        public void VisitContinueStmt(ContinueStmt continueStmt)
        {
            Print("Continue Statement");
        }

        public void VisitBreakStmt(BreakStmt breakStmt)
        {
            Print("Break Statement");
        }

        public void VisitExternalFuncDecl(ExternalFuncDecl externalFuncDecl)
        {
            Print("External Function Declaration:");
            TabRight();
            externalFuncDecl.Parameters.Accept(this);
            Print("Return type: " + TypeNamePrinter.PrintType(externalFuncDecl.ReturnType));
            TabLeft();
        }

        public void VisitProgramStmt(ProgramStmt program)
        {
            Print("Program:");
            TabRight();
            foreach (FileStmt file in program.Files)
                file.Accept(this);
            TabLeft();
        }

        public void VisitBinary(Binary binary)
        {
            Print("Binary: " + binary.Op.Text);
            TabRight();
            binary.Left.Accept(this);
            binary.Right.Accept(this);
            TabLeft();
        }

        public void VisitCall(Call call)
        {
            Print("Call:");
            TabRight();
            Print("Callee:");

            TabRight();
            call.Callee.Accept(this);
            TabLeft();

            Print("Arguments:");
            TabRight();
            foreach (Expression expr in call.Args)
                expr.Accept(this);
            TabLeft();
            TabLeft();
        }

        public void VisitGrouping(Grouping grouping)
        {
            Print("Grouping");
            TabRight();
            grouping.Expr.Accept(this);
            TabLeft();
        }

        public void VisitLiteral(Literal literal)
        {
            Print("Literal: " + literal.Val.Text);
        }

        public void VisitUnary(Unary unary)
        {
            Print("Unary: " + unary.Op.Text);
            TabRight();
            unary.Expr.Accept(this);
            TabLeft();
        }

        public void VisitIdentifier(Identifier variable)
        {
            Print("Identifier: " + variable.Name.Text);
        }

        public void VisitIndex(Index index)
        {
            Print("Index:");
            TabRight();
            Print("Indexed:");

            TabRight();
            index.Indexed.Accept(this);
            TabLeft();

            Print("Argument:");
            TabRight();
            index.Argument.Accept(this);
            TabLeft();
            TabLeft();
        }

        public void VisitCast(Cast cast)
        {
            Print("Cast: " + TypeNamePrinter.PrintType(cast.TypeToCastTo));
            TabLeft();
            cast.Castee.Accept(this);
            TabRight();
        }

        public void VisitInitializerList(InitializerList initializerList)
        {
            Print("Initializer List:");
            TabRight();
            foreach (Expression expression in initializerList.Expressions)
                expression.Accept(this);
            TabLeft();
        }

        private void TabRight() { m_Index++; }
        private void TabLeft() { m_Index--; }

        private void Print(string text) { Console.WriteLine(GetOffset() + text); }

        private string GetOffset()
        {
            string s = "";
            for(int i = 0; i < m_Index; i++)
                s += m_Seperator;

            return s;
        }
    }
}
