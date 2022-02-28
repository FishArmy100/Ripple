using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    static class TypeChecker
    {
        public static List<TypeCheckerError> CheckExpression(Expression expr)
        {
            ASTType intType = new ASTType(TokenType.Int);
            ASTType floatType = new ASTType(TokenType.Float);
            ASTType boolTrueType = new ASTType(TokenType.True);
            ASTType boolFalseType = new ASTType(TokenType.False);

            var unaryDictionary = new Dictionary<ASTType, Dictionary<TokenType, UnaryOperator>>()
            {
                { intType, CreateUnaryOperators(intType, new KeyValuePair<TokenType, ASTType>(TokenType.Minus, intType)) },
                { floatType, CreateUnaryOperators(floatType, new KeyValuePair<TokenType, ASTType>(TokenType.Minus, floatType)) },
                { boolTrueType, CreateUnaryOperators(boolTrueType, new KeyValuePair<TokenType, ASTType>(TokenType.Bang, boolFalseType)) },
                { boolFalseType, CreateUnaryOperators(boolFalseType, new KeyValuePair<TokenType, ASTType>(TokenType.Bang, boolTrueType)) }
            };

            var binaryDictionary = new Dictionary<Tuple<ASTType, ASTType>, Dictionary<TokenType, BinaryOperator>>
            {
                { Pair(intType, intType), CreateBinaryOperators(Pair(intType, intType), 
                    new KeyValuePair<TokenType, ASTType>(TokenType.Plus, intType),
                    new KeyValuePair<TokenType, ASTType>(TokenType.Minus, intType),
                    new KeyValuePair<TokenType, ASTType>(TokenType.Star, intType),
                    new KeyValuePair<TokenType, ASTType>(TokenType.Slash, intType),
                    new KeyValuePair<TokenType, ASTType>(TokenType.Percent, intType)
                ) },

                { Pair(floatType, floatType), CreateBinaryOperators(Pair(floatType, floatType),
                    new KeyValuePair<TokenType, ASTType>(TokenType.Plus, floatType),
                    new KeyValuePair<TokenType, ASTType>(TokenType.Minus, floatType),
                    new KeyValuePair<TokenType, ASTType>(TokenType.Star, floatType),
                    new KeyValuePair<TokenType, ASTType>(TokenType.Slash, floatType),
                    new KeyValuePair<TokenType, ASTType>(TokenType.Percent, floatType)
                ) },
            };

            TypeCheckerVisitor typeCheckerVisitor = new TypeCheckerVisitor(unaryDictionary, binaryDictionary);
            return typeCheckerVisitor.VisitExpression(expr);
        }

        private static Dictionary<TokenType, UnaryOperator> CreateUnaryOperators(ASTType type, params KeyValuePair<TokenType, ASTType>[] operatorTypes)
        {
            Dictionary<TokenType, UnaryOperator> operators = new Dictionary<TokenType, UnaryOperator>();

            foreach(var opt in operatorTypes)
            {
                operators.Add(opt.Key, new UnaryOperator(type, opt.Value, opt.Key));
            }

            return operators;
        }

        private static Dictionary<TokenType, BinaryOperator> CreateBinaryOperators(Tuple<ASTType, ASTType> type, params KeyValuePair<TokenType, ASTType>[] operatorTypes)
        {
            Dictionary<TokenType, BinaryOperator> operators = new Dictionary<TokenType, BinaryOperator>();

            foreach (var opt in operatorTypes)
            {
                operators.Add(opt.Key, new BinaryOperator(type.Item1, opt.Key, type.Item2, opt.Value));
            }

            return operators;
        }

        private static Tuple<ASTType, ASTType> Pair(ASTType first, ASTType second)
        {
            return Tuple.Create(first, second);
        }
    }
}
