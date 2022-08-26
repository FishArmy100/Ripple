using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    class AstPrinter : IExpressionVisitor
    {
        private int m_Index = 0;
        private string m_Seperator;

        public AstPrinter(string seperator)
        {
            m_Seperator = seperator;
        }

        public void PrintAst(Expression expr)
        {
            expr.Accept(this);
            m_Index = 0;
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
