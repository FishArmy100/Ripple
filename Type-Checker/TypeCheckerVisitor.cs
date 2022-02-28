using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    class TypeCheckerVisitor : Expression.IExpressionVisitor<ASTType>
    {
        private readonly Dictionary<ASTType, Dictionary<TokenType, UnaryOperator>> UnaryOperatorDictionary;
        private readonly Dictionary<Tuple<ASTType, ASTType>, Dictionary<TokenType, BinaryOperator>> BinaryOpratorDictionary;

        public TypeCheckerVisitor(Dictionary<ASTType, Dictionary<TokenType, UnaryOperator>> unaryOperatorDictionary, Dictionary<Tuple<ASTType, ASTType>, Dictionary<TokenType, BinaryOperator>> binaryOpratorDictionary)
        {
            UnaryOperatorDictionary = unaryOperatorDictionary;
            BinaryOpratorDictionary = binaryOpratorDictionary;
        }

        public List<TypeCheckerError> VisitExpression(Expression expr)
        {
            List<TypeCheckerError> errors = new List<TypeCheckerError>();

            try
            {
                expr.Accept(this);
            }
            catch(TypeCheckExeption e)
            {
                errors.Add(new TypeCheckerError(e.Message));
            }

            return errors;
        }

        public ASTType VisitBinary(Expression.Binary binary)
        {
            ASTType lType = binary.Left.Accept(this);
            ASTType rType = binary.Right.Accept(this);
            Tuple<ASTType, ASTType> t = new Tuple<ASTType, ASTType>(lType, rType);

            if (BinaryOpratorDictionary.TryGetValue(t, out var opDict))
            {
                if (opDict.TryGetValue(binary.Operator.Type, out var op))
                {
                    return op.ReturnType;
                }
            }


            throw new TypeCheckExeption("Invalid binary operator " + binary.Operator.Type.ToString() + " with types " + lType.Type.ToString() +
                                        " and " + rType.Type.ToString());
        }

        public ASTType VisitGrouping(Expression.Grouping grouping)
        {
            return grouping.GroupedExpression.Accept(this);
        }

        public ASTType VisitLiteral(Expression.Literal literal)
        {
            return new ASTType(literal.Value);
        }

        public ASTType VisitUnary(Expression.Unary unary)
        {
            ASTType type = unary.Right.Accept(this);

            if(UnaryOperatorDictionary.TryGetValue(type, out var opDict))
            {
                if(opDict.TryGetValue(unary.Operator.Type, out var op))
                {
                    return op.ReturnType;
                }
            }


            throw new TypeCheckExeption("Invalid binary operator " + unary.Operator.Type.ToString() + " with type " + type.Type.ToString());
        }
    }
}
