using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    class AstPrinter : IAstVisitor
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

            Print("Body:");
            TabRight();
            ifStmt.Body.Accept(this);
            TabLeft();

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
            Print("Type: " + varDecl.TypeName.Text);

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
            TabLeft();
            returnStmt.Expr.Accept(this);
            TabRight();
        }

        public void VisitParameters(Parameters parameters)
        {
            Print("Parameters: ");
            TabRight();
            foreach (var pair in parameters.ParamList)
                Print(pair.Item1.Text + " " + pair.Item2.Text);
            TabLeft();
        }

        public void VisitFuncDecl(FuncDecl funcDecl)
        {
            Print("Function Declaration:");
            TabRight();
            funcDecl.Param.Accept(this);
            Print("Return type: " + funcDecl.ReturnType.Text);
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
            Print("Call");
            TabRight();

            Print("Arguments:");
            TabRight();
            foreach (Expression expr in call.Args)
                expr.Accept(this);
            TabLeft();

            call.Expr.Accept(this);
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
