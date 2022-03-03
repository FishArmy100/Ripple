using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple;

namespace Ripple
{
    class ASTTypeCheckerVisitor : IASTVisitor<ASTType>
    {
        private ASTInfo m_ASTInfo;

        public bool CheckAST(AbstractSyntaxTree ast, ASTInfo info, out string message)
        {
            m_ASTInfo = info;

            message = "No Error";
            try
            {
                _ = ast.Accept(this);
                return true;
            }
            catch(TypeErrorExeption e)
            {
                message = e.Message;
                return false;
            }
        }

        public ASTType VisitBinary(Expression.Binary binary)
        {
            if (m_ASTInfo.TryGetBinaryOperator(binary.Left.Accept(this).Name, binary.Operator.Type, binary.Right.Accept(this).Name, out ASTBinaryOperator op))
            {
                return op.ReturnType;
            }

            throw new TypeErrorExeption("Cannot use binary operator: " + binary.Operator.ToString() + " on types");
        }

        public ASTType VisitGrouping(Expression.Grouping grouping)
        {
            return grouping.GroupedExpression.Accept(this);
        }

        public ASTType VisitLiteral(Expression.Literal literal)
        {
            if(Utils.TryGetTypeFromTokenType(literal.Value.Type, m_ASTInfo, out ASTType type))
            {
                return type;
            }

            throw new TypeErrorExeption("Literal type not found from token " + literal.Value.ToString());
        }

        public ASTType VisitUnary(Expression.Unary unary)
        {
            if (m_ASTInfo.TryGetUnaryOperator(unary.Right.Accept(this).Name, unary.Operator.Type, out ASTUnaryOperator op))
            {
                return op.ReturnType;
            }

            throw new TypeErrorExeption("Cannot use unary operator: " + unary.Operator.ToString() + " on types");
        }

        private class TypeErrorExeption : Exception
        {
            public TypeErrorExeption(string message) : base(message) { }
        }
    }
}
