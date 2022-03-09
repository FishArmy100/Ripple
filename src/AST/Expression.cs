using System;
using System.Collections.Generic;
using System.Text;

namespace Ripple
{
    abstract class Expression
    {
        public abstract T Accept<T>(IExpressionVisitor<T> visitor);
        public abstract void Accept(IExpressionVisitor visitor);

        public class Literal : Expression
        {
            public readonly Token Value;

            public Literal(Token value)
            {
                Value = value;
            }

            public override T Accept<T>(IExpressionVisitor<T> visitor)
            {
                return visitor.VisitLiteral(this);
            }

            public override void Accept(IExpressionVisitor visitor)
            {
                visitor.VisitLiteral(this);
            }
        }

        public class Binary : Expression
        {
            public readonly Expression Right;
            public readonly Token Operator;
            public readonly Expression Left;

            public Binary(Expression right, Token operatorToken, Expression left)
            {
                Right = right;
                Operator = operatorToken;
                Left = left;
            }

            public override T Accept<T>(IExpressionVisitor<T> visitor)
            {
                return visitor.VisitBinary(this);
            }

            public override void Accept(IExpressionVisitor visitor)
            {
                visitor.VisitBinary(this);
            }
        }

        public class Unary : Expression
        {
            public readonly Expression Right;
            public readonly Token Operator;

            public Unary(Expression right, Token operatorToken)
            {
                Right = right;
                Operator = operatorToken;
            }

            public override T Accept<T>(IExpressionVisitor<T> visitor)
            {
                return visitor.VisitUnary(this);
            }

            public override void Accept(IExpressionVisitor visitor)
            {
                visitor.VisitUnary(this);
            }
        }

        public class Grouping : Expression
        {
            public readonly Expression GroupedExpression;

            public Grouping(Expression groupedExpression)
            {
                GroupedExpression = groupedExpression;
            }

            public override T Accept<T>(IExpressionVisitor<T> visitor)
            {
                return visitor.VisitGrouping(this);
            }

            public override void Accept(IExpressionVisitor visitor)
            {
                visitor.VisitGrouping(this);
            }
        }

        public class Assignment : Expression
        {
            public readonly Token Name;
            public readonly Expression Value;

            public Assignment(Token name, Expression value)
            {
                Name = name;
                Value = value;
            }

            public override T Accept<T>(IExpressionVisitor<T> visitor)
            {
                return visitor.VisitAssignment(this);
            }

            public override void Accept(IExpressionVisitor visitor)
            {
                visitor.VisitAssignment(this);
            }
        }

        public class Variable : Expression
        {
            public readonly Token Name;

            public Variable(Token name)
            {
                Name = name;
            }

            public override T Accept<T>(IExpressionVisitor<T> visitor)
            {
                return visitor.VisitVariable(this);
            }

            public override void Accept(IExpressionVisitor visitor)
            {
                visitor.VisitVariable(this);
            }
        }


        public interface IExpressionVisitor<T>
        {
            public T VisitBinary(Binary binary);
            public T VisitLiteral(Literal literal);
            public T VisitUnary(Unary unary);
            public T VisitGrouping(Grouping grouping);
            public T VisitAssignment(Assignment assignment);
            public T VisitVariable(Variable variable);
        }

        public interface IExpressionVisitor
        {
            public void VisitBinary(Binary binary);
            public void VisitLiteral(Literal literal);
            public void VisitUnary(Unary unary);
            public void VisitGrouping(Grouping grouping);
            public void VisitAssignment(Assignment assignment);
            public void VisitVariable(Variable variable);
        }
    }
}
