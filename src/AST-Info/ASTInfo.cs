using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST.Info
{
    class ASTInfo
    {
        public readonly List<ASTTypeInfo> Types;
        private readonly Dictionary<string, ASTTypeInfo> m_TypeDictionary;

        public readonly List<ASTBinaryOperator> BinaryOperators;
        private readonly Dictionary<Tuple<ASTTypeInfo, TokenType, ASTTypeInfo>, ASTBinaryOperator> m_BinaryOperatorDictionary;

        public readonly List<ASTUnaryOperator> UnaryOperators;
        private readonly Dictionary<Tuple<ASTTypeInfo, TokenType>, ASTUnaryOperator> m_UnaryOperatorDictionary;

        public ASTInfo(List<ASTTypeInfo> types, List<ASTBinaryOperator> binaryOperators, List<ASTUnaryOperator> unaryOperators)
        {
            Types = types;
            BinaryOperators = binaryOperators;

            m_TypeDictionary = new Dictionary<string, ASTTypeInfo>();
            Types.ForEach(type => m_TypeDictionary.Add(type.Name, type));

            m_BinaryOperatorDictionary = new Dictionary<Tuple<ASTTypeInfo, TokenType, ASTTypeInfo>, ASTBinaryOperator>();
            foreach(ASTBinaryOperator op in binaryOperators)
            {
                m_BinaryOperatorDictionary.Add(Tuple.Create(op.Left, op.Operator, op.Right), op);
            }

            m_UnaryOperatorDictionary = new Dictionary<Tuple<ASTTypeInfo, TokenType>, ASTUnaryOperator>();
            foreach(ASTUnaryOperator op in unaryOperators)
            {
                m_UnaryOperatorDictionary.Add(Tuple.Create(op.Type, op.Operator), op);
            }
        }

        public bool ContainsType(string name)
        {
            return m_TypeDictionary.ContainsKey(name);
        }

        public bool TryGetType(string name, out ASTTypeInfo type)
        {
            if(m_TypeDictionary.TryGetValue(name, out type))
            {
                return true;
            }

            return false;
        }

        public ASTTypeInfo GetType(string name)
        {
            return m_TypeDictionary[name];
        }

        public bool HasUnaryOperator(string typeName, TokenType operatorToken)
        {
            if (ContainsType(typeName))
            {
                var key = Tuple.Create(GetType(typeName), operatorToken);
                return m_UnaryOperatorDictionary.ContainsKey(key);
            }

            return false;
        }

        public bool HasBinaryOperator(string leftTypeName, TokenType operatorToken, string rightTypeName)
        {
            if (ContainsType(leftTypeName) && ContainsType(rightTypeName))
            {
                var key = Tuple.Create(GetType(leftTypeName), operatorToken, GetType(rightTypeName));
                return m_BinaryOperatorDictionary.ContainsKey(key);
            }

            return false;
        }

        public ASTUnaryOperator GetUnaryOperator(string typeName, TokenType operatorToken)
        {
            return m_UnaryOperatorDictionary[Tuple.Create(GetType(typeName), operatorToken)];
        }

        public ASTBinaryOperator GetBinaryOperator(string leftTypeName, TokenType operatorToken, string rightTypeName)
        {
            return m_BinaryOperatorDictionary[Tuple.Create(GetType(leftTypeName), operatorToken, GetType(rightTypeName))];
        }

        public bool TryGetUnaryOperator(string typeName, TokenType operatorTokenType, out ASTUnaryOperator op)
        {
            op = new ASTUnaryOperator();

            if (ContainsType(typeName))
            {
                var key = Tuple.Create(GetType(typeName), operatorTokenType);
                if (m_UnaryOperatorDictionary.TryGetValue(key, out op))
                    return true;
            }

            return false;
        }

        public bool TryGetBinaryOperator(string leftTypeName, TokenType operatorToken, string rightTypeName, out ASTBinaryOperator op)
        {
            op = new ASTBinaryOperator();

            if(ContainsType(leftTypeName) && ContainsType(rightTypeName))
            {
                var key = Tuple.Create(GetType(leftTypeName), operatorToken, GetType(rightTypeName));
                if (m_BinaryOperatorDictionary.TryGetValue(key, out op))
                    return true;
            }

            return false;
        }
    }
}
