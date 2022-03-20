using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST.Info
{
    class ASTInfoBuilder
    {
        private readonly Dictionary<string, ASTTypeInfo> m_Types;
        private Dictionary<Tuple<string, TokenType, string>, ASTBinaryOperator> m_BinaryOperators;
        private Dictionary<Tuple<string, TokenType>, ASTUnaryOperator> m_UnaryOperators;

        public ASTInfoBuilder()
        {
            m_Types = new Dictionary<string, ASTTypeInfo>();
            m_BinaryOperators = new Dictionary<Tuple<string, TokenType, string>, ASTBinaryOperator>();
            m_UnaryOperators = new Dictionary<Tuple<string, TokenType>, ASTUnaryOperator>();
        }

        public void PushPrimaryTypes()
        {
            const string boolType = RippleKeywords.BOOL_TYPE_NAME;
            const string charType = RippleKeywords.CHAR_TYPE_NAME;
            const string intType = RippleKeywords.INT_TYPE_NAME;
            const string floatType = RippleKeywords.FLOAT_TYPE_NAME;
            const string stringType = RippleKeywords.STRING_TYPE_NAME;

            PushType(boolType);
            PushType(charType);
            PushType(intType);
            PushType(floatType);
            PushType(stringType);

            // ints
            PushBinaryOperator(intType, TokenType.Plus, intType, intType);
            PushBinaryOperator(intType, TokenType.Minus, intType, intType);
            PushBinaryOperator(intType, TokenType.Star, intType, intType);
            PushBinaryOperator(intType, TokenType.Slash, intType, intType);
            PushBinaryOperator(intType, TokenType.Percent, intType, intType);

            PushBinaryOperator(intType, TokenType.EqualEqual, intType, boolType);
            PushBinaryOperator(intType, TokenType.BangEqual, intType, boolType);
            PushBinaryOperator(intType, TokenType.GreaterThen, intType, boolType);
            PushBinaryOperator(intType, TokenType.GreaterThenEqual, intType, boolType);
            PushBinaryOperator(intType, TokenType.LessThen, intType, boolType);
            PushBinaryOperator(intType, TokenType.LessThenEqual, intType, boolType);

            PushUnaryOperator(intType, TokenType.Minus, intType);

            // floats
            PushBinaryOperator(floatType, TokenType.Plus, floatType, floatType);
            PushBinaryOperator(floatType, TokenType.Minus, floatType, floatType);
            PushBinaryOperator(floatType, TokenType.Star, floatType, floatType);
            PushBinaryOperator(floatType, TokenType.Slash, floatType, floatType);

            PushUnaryOperator(floatType, TokenType.Minus, floatType);

            PushBinaryOperator(floatType, TokenType.EqualEqual, floatType, boolType);
            PushBinaryOperator(floatType, TokenType.BangEqual, floatType, boolType);
            PushBinaryOperator(floatType, TokenType.GreaterThen, floatType, boolType);
            PushBinaryOperator(floatType, TokenType.GreaterThenEqual, floatType, boolType);
            PushBinaryOperator(floatType, TokenType.LessThen, floatType, boolType);
            PushBinaryOperator(floatType, TokenType.LessThenEqual, floatType, boolType);

            // bools
            PushBinaryOperator(boolType, TokenType.EqualEqual, boolType, boolType);
            PushBinaryOperator(boolType, TokenType.BangEqual, boolType, boolType);
            PushBinaryOperator(boolType, TokenType.AmpersandAmpersand, boolType, boolType);
            PushBinaryOperator(boolType, TokenType.PipePipe, boolType, boolType);

            PushUnaryOperator(boolType, TokenType.Bang, boolType);

            // strings
            PushBinaryOperator(stringType, TokenType.Plus, stringType, stringType);
        }

        public ASTInfo BuildInfo()
        {
            return new ASTInfo(m_Types.Values.ToList(), m_BinaryOperators.Values.ToList(), m_UnaryOperators.Values.ToList());
        }

        public bool PushType(string name)
        {
            if(!m_Types.ContainsKey(name))
            {
                m_Types.Add(name, new ASTTypeInfo(name));
            }

            return false;
        }

        public bool HasType(string typeName)
        {
            return m_Types.ContainsKey(typeName);
        }

        public bool HasTypes(params string[] typeNames)
        {
            foreach (string type in typeNames)
            {
                if (!HasType(type))
                    return false;
            }

            return true;
        }

        public ASTTypeInfo? GetType(string typeName)
        {
            if(m_Types.TryGetValue(typeName, out ASTTypeInfo type))
            {
                return type;
            }

            return null;
        }

        public bool HasBinaryOperator(string leftTypeName, TokenType operatorTokenType, string rightTypeName)
        {
            return m_BinaryOperators.ContainsKey(Tuple.Create(leftTypeName, operatorTokenType, rightTypeName));
        }

        public bool HasUnaryOperator(string typeName, TokenType operatorTokenType)
        {
            return m_UnaryOperators.ContainsKey(Tuple.Create(typeName, operatorTokenType));
        }

        public bool PushBinaryOperator(string leftTypeName, TokenType operatorTokenType, string rightTypeName, string returnTypeName)
        {
            bool hasOperator = HasBinaryOperator(leftTypeName, operatorTokenType, rightTypeName);
            if (HasTypes(leftTypeName, rightTypeName, returnTypeName) && !hasOperator)
            {
                ASTBinaryOperator op = new ASTBinaryOperator(GetType(leftTypeName).Value, operatorTokenType, GetType(rightTypeName).Value, GetType(returnTypeName).Value);
                m_BinaryOperators.Add(Tuple.Create(leftTypeName, operatorTokenType, rightTypeName), op);
            }

            return false;
        }

        public bool PushUnaryOperator(string typeName, TokenType operatorTokenType, string returnTypeName)
        {
            bool hasOperator = HasUnaryOperator(typeName, operatorTokenType);
            if (HasTypes(typeName, returnTypeName) && !hasOperator)
            {
                ASTUnaryOperator op = new ASTUnaryOperator(GetType(typeName).Value, operatorTokenType, GetType(returnTypeName).Value);
                m_UnaryOperators.Add(Tuple.Create(typeName, operatorTokenType), op);
            }

            return false;
        }
    }
}
