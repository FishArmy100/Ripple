using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.Lexing;

namespace Ripple.Transpiling
{
    class TranspilerExpressionVisitor : IExpressionVisitor<CExpression>
    {
        public static CExpression Visit(Expression expression)
        {
            TranspilerExpressionVisitor visitor = new TranspilerExpressionVisitor();
            return expression.Accept(visitor);
        }


        public CExpression VisitBinary(Binary binary)
        {
            CExpression left = binary.Left.Accept(this);
            CExpression right = binary.Right.Accept(this);
            CBinaryOperator op = ConvertBinaryOperator(binary.Op.Type);
            return new CExpression.Binary(left, op, right);
        }

        public CExpression VisitCall(Call call)
        {
            List<CExpression> args = new List<CExpression>();

            foreach (Expression arg in call.Args)
                args.Add(arg.Accept(this));

            return new CExpression.Call(args, call.Identifier.Text);
        }

        public CExpression VisitGrouping(Grouping grouping)
        {
            return new CExpression.Grouping(grouping.Expr.Accept(this));
        }

        public CExpression VisitIdentifier(Identifier identifier)
        {
            return CExpression.Value.FromIdentifier(identifier.Name.Text);
        }

        public CExpression VisitLiteral(Literal literal)
        {
            string text = literal.Val.Text;
            TokenType type = literal.Val.Type;
            return type switch
            {
                TokenType.IntagerLiteral => CExpression.Value.FromInt(int.Parse(text)),
                TokenType.FloatLiteral => CExpression.Value.FromFloat(float.Parse(text)),
                TokenType.True => CExpression.Value.FromBool(true),
                TokenType.False => CExpression.Value.FromBool(false),
                _ => throw new ArgumentException("Token is not a literal")
            };
        }

        public CExpression VisitUnary(Unary unary)
        {
            CExpression operand = unary.Expr.Accept(this);
            CUnaryOperator op = ConvertUnaryOperator(unary.Op.Type);
            return new CExpression.Unary(operand, op);
        }

        private static CBinaryOperator ConvertBinaryOperator(TokenType type)
        {
            return type switch
            {
                TokenType.Plus => CBinaryOperator.Plus,
                TokenType.Minus => CBinaryOperator.Minus,
                TokenType.Star => CBinaryOperator.Times,
                TokenType.Slash => CBinaryOperator.Divide,
                TokenType.Equal => CBinaryOperator.Assign,
                TokenType.EqualEqual => CBinaryOperator.EqualEqual,
                TokenType.BangEqual => CBinaryOperator.BangEqual,
                TokenType.GreaterThan => CBinaryOperator.GreaterThan,
                TokenType.GreaterThanEqual => CBinaryOperator.GreaterThanEqual,
                TokenType.LessThan => CBinaryOperator.LessThan,
                TokenType.LessThanEqual => CBinaryOperator.LessThanEqual,
                TokenType.Mod => CBinaryOperator.Mod,
                _ => throw new ArgumentException("Cannot convert operator")
            };
        }

        private static CUnaryOperator ConvertUnaryOperator(TokenType type)
        {
            return type switch
            {
                TokenType.Minus => CUnaryOperator.Negate,
                TokenType.Bang => CUnaryOperator.Bang,
                _ => throw new ArgumentException("Cannot convert operator")
            };
        }
    }
}
