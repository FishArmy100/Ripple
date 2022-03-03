using System;
using System.Collections.Generic;
using System.Text;

namespace Ripple
{
    class ASTPrinter : IASTVisitor<string>
    {
        private int IndentCount = 0;

        public static string PrintTree(Expression expression)
        {
            return expression.Accept(new ASTPrinter());
        }

        public string VisitBinary(Expression.Binary binary)
        {
            string sExpr = GetOffset() + "Binary Expression: operator " + binary.Operator.Lexeme;
            IndentCount++;
               sExpr += "\n" + binary.Left.Accept(this) + binary.Right.Accept(this);
            IndentCount--;
            return sExpr;
        }

        public string VisitGrouping(Expression.Grouping grouping)
        {
            string sExpr = GetOffset() + "Grouped Expression: \n";
            IndentCount++;
            sExpr += grouping.GroupedExpression.Accept(this);
            IndentCount--;
            return sExpr;
        }

        public string VisitLiteral(Expression.Literal literal)
        {
            string sExpr = GetOffset() + "Literal: " + literal.Value.Lexeme + "\n";
            return sExpr;
        }

        public string VisitUnary(Expression.Unary unary)
        {
            string sExpr = GetOffset() + "Binary Expression: operator " + unary.Operator.Lexeme;
            IndentCount++;
             sExpr += "\n" + unary.Right.Accept(this);
            IndentCount--;
            return sExpr;
        }

        private string GetOffset()
        {
            string offset = "";
            for(int i = 0; i < IndentCount; i++)
            {
                offset += "    ";
            }

            return offset;
        }
    }
}
