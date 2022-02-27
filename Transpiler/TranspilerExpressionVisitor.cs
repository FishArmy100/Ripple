using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    struct TranspilerExpressionVisitor : Expression.IExpressionVisitor<string>
    {
        public string VisitBinary(Expression.Binary binary)
        {
            return binary.Left.Accept(this) + " " + binary.Operator.Lexeme + " " + binary.Right.Accept(this);
        }

        public string VisitGrouping(Expression.Grouping grouping)
        {
            return "(" + grouping.GroupedExpression.Accept(this) + ")";
        }

        public string VisitLiteral(Expression.Literal literal)
        {
            switch(literal.Value.Type)
            {
                case TokenType.Int:
                    return literal.Value.Literal.ToString();
                case TokenType.Uint:
                    return literal.Value.Literal.ToString();
                case TokenType.Float:
                    return literal.Value.Literal.ToString();
                case TokenType.True:
                    return "true";
                case TokenType.False:
                    return "false";
                case TokenType.String:
                    return literal.Value.Lexeme;
                case TokenType.Char:
                    return literal.Value.Lexeme;
                default:
                    return string.Empty;
            }
        }

        public string VisitUnary(Expression.Unary unary)
        {
            return unary.Operator.Lexeme + unary.Right.Accept(this);
        }
    }
}
