using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Utils.Extensions;

namespace Ripple.Validation.AstInfo
{
    static class RippleBuiltins
    {
        public static List<TypeData> GetPrimitives()
        {
            return new List<TypeData>()
            {
                new TypeData(RipplePrimitives.Int32Name),
                new TypeData(RipplePrimitives.Float32Name),
                new TypeData(RipplePrimitives.BoolName),
                new TypeData(RipplePrimitives.VoidName),
            };
        }

        public static List<OperatorData> GetPrimitiveOperators()
        {
            List<OperatorData> intOperators = GenBinaries(RipplePrimitives.Int32Name,
                TokenType.Plus, TokenType.Minus, TokenType.Star, TokenType.Slash, TokenType.Mod,
                TokenType.GreaterThan, TokenType.GreaterThanEqual, TokenType.LessThan,
                TokenType.LessThanEqual, TokenType.EqualEqual, TokenType.BangEqual, TokenType.Equal);

            List<OperatorData> floatOperators = GenBinaries(RipplePrimitives.Float32Name,
                TokenType.Plus, TokenType.Minus, TokenType.Star, TokenType.Slash,
                TokenType.GreaterThan, TokenType.GreaterThanEqual, TokenType.LessThan,
                TokenType.LessThanEqual, TokenType.EqualEqual, TokenType.BangEqual, TokenType.Equal);

            List<OperatorData> boolOperators = GenBinaries(RipplePrimitives.BoolName,
                TokenType.EqualEqual, TokenType.BangEqual, TokenType.Equal,
                TokenType.AmpersandAmpersand, TokenType.PipePipe);

            List<OperatorData> operators = new List<OperatorData>();
            operators.AddRange(intOperators);
            operators.AddRange(floatOperators);
            operators.AddRange(boolOperators);

            operators.Add(GenUnary(TokenType.Minus, RipplePrimitives.Int32Name));
            operators.Add(GenUnary(TokenType.Minus, RipplePrimitives.Float32Name));
            operators.Add(GenUnary(TokenType.Bang, RipplePrimitives.BoolName));

            return operators;
        }

        public static List<FunctionData> GetBuiltInFunctions()
        {
            return new List<FunctionData>()
            {
                GenFunctionData("print", new (){("int", "value")}, "void"),
                GenFunctionData("print", new (){("float", "value")}, "void"),
                GenFunctionData("print", new (){("bool", "value")}, "void"),
            };
        }

        private static Token GenIdTok(string name)
        {
            return new Token(name, TokenType.Identifier, -1, -1);
        }

        private static FunctionData GenFunctionData(string name, List<(string, string)> paramaters, string returnTypeName)
        {
            Token funcName = GenIdTok(name);
            Token returnType = GenIdTok(returnTypeName);
            List<(Token, Token)> parameterTokens = paramaters
                .ConvertAll(p => (GenIdTok(p.Item1), GenIdTok(p.Item2)));

            return new FunctionData(funcName, parameterTokens, returnType);
        }

        private static List<OperatorData> GenBinaries(string typeName, params TokenType[] operatorTypes)
        {
            List<OperatorData> operatorDatas = new List<OperatorData>();
            foreach (TokenType operatorType in operatorTypes)
                operatorDatas.Add(GenBinary(operatorType, typeName));

            return operatorDatas;
        }

        private static OperatorData GenBinary(TokenType operatorType, string typeName)
        {
            if (operatorType.IsType(TokenType.EqualEqual, TokenType.GreaterThanEqual, TokenType.BangEqual, 
                TokenType.LessThanEqual, TokenType.GreaterThan, TokenType.LessThan))
                return new OperatorData.Binary(operatorType, RipplePrimitives.BoolName, typeName, typeName);

            return new OperatorData.Binary(operatorType, typeName, typeName, typeName);
        }

        private static OperatorData GenUnary(TokenType operatorType, string typeName)
        {
            return new OperatorData.Unary(operatorType, typeName, typeName);
        }
    }
}
