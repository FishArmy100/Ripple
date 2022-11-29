using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Utils.Extensions;
using Ripple.AST.Info;

namespace Ripple.AST.Utils
{
    static class RippleBuiltins
    {
        public static List<PrimaryTypeInfo> GetPrimitives()
        {
            return new List<PrimaryTypeInfo>()
            {
                GenPrimative(RipplePrimitives.Int32Name),
                GenPrimative(RipplePrimitives.Float32Name),
                GenPrimative(RipplePrimitives.BoolName),
                GenPrimative(RipplePrimitives.VoidName),
                GenPrimative(RipplePrimitives.CharName),
            };
        }

        //public static List<OperatorInfo> GetPrimitiveOperators()
        //{
        //    List<OperatorInfo> intOperators = GenBinaries(RipplePrimitives.Int32Name,
        //        TokenType.Plus, TokenType.Minus, TokenType.Star, TokenType.Slash, TokenType.Mod,
        //        TokenType.GreaterThan, TokenType.GreaterThanEqual, TokenType.LessThan,
        //        TokenType.LessThanEqual, TokenType.EqualEqual, TokenType.BangEqual, TokenType.Equal);

        //    List<OperatorInfo> floatOperators = GenBinaries(RipplePrimitives.Float32Name,
        //        TokenType.Plus, TokenType.Minus, TokenType.Star, TokenType.Slash,
        //        TokenType.GreaterThan, TokenType.GreaterThanEqual, TokenType.LessThan,
        //        TokenType.LessThanEqual, TokenType.EqualEqual, TokenType.BangEqual, TokenType.Equal);

        //    List<OperatorInfo> boolOperators = GenBinaries(RipplePrimitives.BoolName,
        //        TokenType.EqualEqual, TokenType.BangEqual, TokenType.Equal,
        //        TokenType.AmpersandAmpersand, TokenType.PipePipe);

        //    List<OperatorInfo> operators = new List<OperatorInfo>();
        //    operators.AddRange(intOperators);
        //    operators.AddRange(floatOperators);
        //    operators.AddRange(boolOperators);

        //    operators.Add(GenUnary(TokenType.Minus, RipplePrimitives.Int32Name));
        //    operators.Add(GenUnary(TokenType.Minus, RipplePrimitives.Float32Name));
        //    operators.Add(GenUnary(TokenType.Bang, RipplePrimitives.BoolName));

        //    return operators;
        //}

        public static List<FunctionInfo> GetBuiltInFunctions()
        {
            return new List<FunctionInfo>()
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

        private static FunctionInfo GenFunctionData(string name, List<(string, string)> paramaters, string returnTypeName)
        {
            Token funcName = GenIdTok(name);
            TypeInfo returnType = GenBasicType(returnTypeName);
            List<ParameterInfo> parameterInfos = paramaters
                .ConvertAll(p => new ParameterInfo(GenIdTok(p.Item2), GenBasicType(p.Item1)));

            return new FunctionInfo(funcName, parameterInfos, returnType);
        }

        //private static List<OperatorInfo> GenBinaries(string typeName, params TokenType[] operatorTypes)
        //{
        //    List<OperatorInfo> operatorDatas = new List<OperatorInfo>();
        //    foreach (TokenType operatorType in operatorTypes)
        //        operatorDatas.Add(GenBinary(operatorType, typeName));

        //    return operatorDatas;
        //}

        //private static OperatorInfo GenBinary(TokenType operatorType, string typeName)
        //{
        //    if (operatorType.IsType(TokenType.EqualEqual, TokenType.GreaterThanEqual, TokenType.BangEqual, 
        //        TokenType.LessThanEqual, TokenType.GreaterThan, TokenType.LessThan))
        //        return new OperatorInfo.Binary(operatorType, GenBasicType(RipplePrimitives.BoolName), GenBasicType(typeName), GenBasicType(typeName));

        //    return new OperatorInfo.Binary(operatorType, GenBasicType(typeName), GenBasicType(typeName), GenBasicType(typeName));
        //}

        //private static OperatorInfo GenUnary(TokenType operatorType, string typeName)
        //{
        //    return new OperatorInfo.Unary(operatorType, GenBasicType(typeName), GenBasicType(typeName));
        //}

        private static PrimaryTypeInfo GenPrimative(string name)
        {
            return new PrimaryTypeInfo(GenIdTok(name));
        }

        private static TypeInfo GenBasicType(string name)
        {
            return new TypeInfo.Basic(false, GenPrimative(name));
        }
    }
}
