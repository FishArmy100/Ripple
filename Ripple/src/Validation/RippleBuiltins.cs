using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Utils.Extensions;

namespace Ripple.Validation
{
    static class RippleBuiltins
    {
        public static List<TypeData> GetPrimitives()
        {
            return new List<TypeData>()
            {
                new TypeData(RipplePrimitiveNames.Int32),
                new TypeData(RipplePrimitiveNames.Float32),
                new TypeData(RipplePrimitiveNames.Bool),
                new TypeData(RipplePrimitiveNames.Void),
            };
        }

        public static List<OperatorData> GetPrimitiveOperators()
        {
            List<OperatorData> intOperators = GenBinaries(RipplePrimitiveNames.Int32,
                TokenType.Plus, TokenType.Minus, TokenType.Star, TokenType.Slash, TokenType.Mod,
                TokenType.GreaterThan, TokenType.GreaterThanEqual, TokenType.LessThan,
                TokenType.LessThanEqual, TokenType.EqualEqual, TokenType.BangEqual, TokenType.Equal);

            List<OperatorData> floatOperators = GenBinaries(RipplePrimitiveNames.Float32,
                TokenType.Plus, TokenType.Minus, TokenType.Star, TokenType.Slash,
                TokenType.GreaterThan, TokenType.GreaterThanEqual, TokenType.LessThan,
                TokenType.LessThanEqual, TokenType.EqualEqual, TokenType.BangEqual, TokenType.Equal);

            List<OperatorData> boolOperators = GenBinaries(RipplePrimitiveNames.Bool,
                TokenType.EqualEqual, TokenType.BangEqual, TokenType.Equal,
                TokenType.AmpersandAmpersand, TokenType.PipePipe);

            List<OperatorData> operators = new List<OperatorData>();
            operators.AddRange(intOperators);
            operators.AddRange(floatOperators);
            operators.AddRange(boolOperators);

            operators.Add(GenUnary(TokenType.Minus, RipplePrimitiveNames.Int32));
            operators.Add(GenUnary(TokenType.Minus, RipplePrimitiveNames.Float32));
            operators.Add(GenUnary(TokenType.Bang, RipplePrimitiveNames.Bool));

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
                return new OperatorData.Binary(operatorType, RipplePrimitiveNames.Bool, typeName, typeName);

            return new OperatorData.Binary(operatorType, typeName, typeName, typeName);
        }

        private static OperatorData GenUnary(TokenType operatorType, string typeName)
        {
            return new OperatorData.Unary(operatorType, typeName, typeName);
        }
    }
}
