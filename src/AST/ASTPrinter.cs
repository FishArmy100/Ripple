using System;
using System.Collections.Generic;
using System.Text;

namespace Ripple
{
    class ASTPrinter : IASTVisitor<string>
    {
        private int m_IndentCount = 0;
        private readonly string m_IndentSeperator;

        public static string PrintTree(AbstractSyntaxTree ast, string seperator)
        {
            List<string> statements = ast.Accept(new ASTPrinter(seperator));
            string tree = "";
            foreach(string s in statements)
            {
                tree += s;
            }

            return tree;
        }

        public ASTPrinter(string seperator = "\t")
        {
            m_IndentSeperator = seperator;
            m_IndentCount = 0;
        }

        public string VisitAssignment(Expression.Assignment assignment)
        {
            string sExpr = GetOffset() + "Assignment Expression: Name " + assignment.Name.Lexeme;
            m_IndentCount++;
            sExpr += "\n" + assignment.Value.Accept(this);
            m_IndentCount--;
            return sExpr;
        }

        public string VisitBinary(Expression.Binary binary)
        {
            string sExpr = GetOffset() + "Binary Expression: operator " + binary.Operator.Lexeme;
            m_IndentCount++;
            sExpr += "\n" + binary.Left.Accept(this) + binary.Right.Accept(this);
            m_IndentCount--;
            return sExpr;
        }

        public string VisitGrouping(Expression.Grouping grouping)
        {
            string sExpr = GetOffset() + "Grouped Expression: \n";
            m_IndentCount++;
            sExpr += grouping.GroupedExpression.Accept(this);
            m_IndentCount--;
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
            m_IndentCount++;
            sExpr += "\n" + unary.Right.Accept(this);
            m_IndentCount--;
            return sExpr;
        }

        public string VisitVariable(Expression.Variable variable)
        {
            string sExpr = GetOffset() + "Variable: " + variable.Name.Lexeme + "\n";
            return sExpr;
        }

        public string VisitVarDeclaration(Statement.VarDeclaration variable)
        {
            string sExpr = GetOffset() + "Variable declaration: " + variable.TypeName.Lexeme + " " + variable.Name.Lexeme;
            m_IndentCount++;
            sExpr += "\n" + variable.Initializer.Accept(this);
            m_IndentCount--;
            return sExpr;
        }

        public string VisitFuncDeclaration(Statement.FuncDeclaration funcDeclaration)
        {
            string sFunc = GetOffset() + "Func declaration: " + funcDeclaration.Name.Lexeme + "\n";
            m_IndentCount++;
            sFunc += GetOffset() + "Return type: " + funcDeclaration.ReturnType.Lexeme + "\n";

            sFunc += GetOffset() + "Parameters: ";
            foreach (Tuple<Token, Token> parameter in funcDeclaration.Parameters)
                sFunc += "type: " + parameter.Item1.Lexeme + ", name: " + parameter.Item2.Lexeme + "; ";
            sFunc = sFunc.Remove(sFunc.Length - 2);

            sFunc += "\n";

            sFunc += funcDeclaration.Body.Accept(this);

            m_IndentCount--;
            return sFunc;
        }

        public string VisitExpressionStmt(Statement.ExpressionStmt expressionStmt)
        {
            string sExpr = GetOffset() + "Expression Statement: ";
            m_IndentCount++;
            sExpr += "\n" + expressionStmt.Expr.Accept(this);
            m_IndentCount--;
            return sExpr;
        }

        public string VisitBlock(Statement.Block block)
        {
            string sBlock = GetOffset() + "Block statement: \n";
            m_IndentCount++;
            foreach (Statement s in block.Statements)
                sBlock += s.Accept(this);
            m_IndentCount--;
            return sBlock;
        }

        public string VisitCall(Expression.Call call)
        {
            string sCall = GetOffset() + "Call expression: \n";

            m_IndentCount++;
            sCall += GetOffset() + "Name: " + call.Name.Lexeme + "\n";
            
            sCall += GetOffset() + "Parameters: \n";
            m_IndentCount++;
            foreach (Expression e in call.Parameters)
                sCall += e.Accept(this);
            m_IndentCount--;

            m_IndentCount--;

            return sCall;
        }

        public string VisitIfStmt(Statement.IfStmt ifStmt)
        {
            string sIf = GetOffset() + "If statement:\n";
            m_IndentCount++;

            sIf += GetOffset() + "Condition: \n";
            m_IndentCount++;
            sIf += ifStmt.Condition.Accept(this);
            m_IndentCount--;

            sIf += GetOffset() + "Then branch: \n";
            m_IndentCount++;
            sIf += ifStmt.ThenBranch.Accept(this);
            m_IndentCount--;


            if(ifStmt.ElseBranch != null)
            {
                sIf += GetOffset() + "Else branch: \n";
                m_IndentCount++;
                sIf += ifStmt.ElseBranch.Accept(this);
                m_IndentCount--;
            }
            m_IndentCount--;
            return sIf;
        }

        public string VisitWhileLoop(Statement.WhileLoop whileLoop)
        {
            string sWhile = GetOffset() + "While loop:\n";
            m_IndentCount++;


            sWhile += GetOffset() + "Condition: \n";
            m_IndentCount++;
            sWhile += whileLoop.Condition.Accept(this);
            m_IndentCount--;


            sWhile += GetOffset() + "Body: \n";
            m_IndentCount++;
            sWhile += whileLoop.Body.Accept(this);
            m_IndentCount--;

            m_IndentCount--;
            return sWhile;
        }

        public string VisitContinueStmt(Statement.ContinueStmt continueStmt)
        {
            return GetOffset() + "Continue statement \n";
        }

        public string VisitBreakStmt(Statement.BreakStmt breakStmt)
        {
            return GetOffset() + "Break statement \n";
        }

        public string VisitReturnStmt(Statement.ReturnStmt returnStmt)
        {
            string sReturn = GetOffset() + "Return statment: \n";
            

            if(returnStmt.ReturnExpression != null)
            {
                m_IndentCount++;
                sReturn += GetOffset() + "Expression: \n";

                m_IndentCount++;
                sReturn += returnStmt.ReturnExpression.Accept(this);
                m_IndentCount--;

                m_IndentCount--;
            }
            return sReturn;
        }

        public string VisitForLoop(Statement.ForLoop forLoop)
        {
            string sFor = GetOffset() + "For loop:\n";
            m_IndentCount++;
            if (forLoop.Initializer != null)
            {
                sFor += GetOffset() + "Initializer: \n";
                m_IndentCount++;
                sFor += forLoop.Initializer.Accept(this);
                m_IndentCount--;
            }

            if(forLoop.Condition != null)
            {
                sFor += GetOffset() + "Condition: \n";
                m_IndentCount++;
                sFor += forLoop.Condition.Accept(this);
                m_IndentCount--;
            }

            if (forLoop.Incrementer != null)
            {
                sFor += GetOffset() + "Incrementer: \n";
                m_IndentCount++;
                sFor += forLoop.Incrementer.Accept(this);
                m_IndentCount--;
            }

            sFor += GetOffset() + "Body: \n";
            m_IndentCount++;
            sFor += forLoop.Body.Accept(this);
            m_IndentCount--;

            m_IndentCount--;
            return sFor;
        }

        private string GetOffset()
        {
            string offset = "";
            for (int i = 0; i < m_IndentCount; i++)
            {
                offset += "   ";
            }

            return offset;
        }
    }
}
