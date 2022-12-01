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

        public static OperatorLibrary GetPrimitiveOperators()
        {
            OperatorLibrary library = new OperatorLibrary();
            AppendBinaryOperators(ref library);
            AppendUnaryOperators(ref library);
            AppendCastOperators(ref library);

            return library;
        }

        private static void AppendBinaryOperators(ref OperatorLibrary library)
        {
            List<OperatorInfo.Binary> intOperators = GenBinaries(RipplePrimitives.Int32Name,
                TokenType.Plus, TokenType.Minus, TokenType.Star, TokenType.Slash, TokenType.Mod,
                TokenType.GreaterThan, TokenType.GreaterThanEqual, TokenType.LessThan,
                TokenType.LessThanEqual, TokenType.EqualEqual, TokenType.BangEqual);

            List<OperatorInfo.Binary> floatOperators = GenBinaries(RipplePrimitives.Float32Name,
                TokenType.Plus, TokenType.Minus, TokenType.Star, TokenType.Slash,
                TokenType.GreaterThan, TokenType.GreaterThanEqual, TokenType.LessThan,
                TokenType.LessThanEqual, TokenType.EqualEqual, TokenType.BangEqual);

            List<OperatorInfo.Binary> boolOperators = GenBinaries(RipplePrimitives.BoolName,
                TokenType.EqualEqual, TokenType.BangEqual,
                TokenType.AmpersandAmpersand, TokenType.PipePipe);

            List<OperatorInfo.Binary> operators = new List<OperatorInfo.Binary>();
            operators.AddRange(intOperators);
            operators.AddRange(floatOperators);
            operators.AddRange(boolOperators);

            foreach (var op in operators)
                _ = library.BinaryOperators.TryAdd(op);
        }

        private static void AppendUnaryOperators(ref OperatorLibrary library)
        {
            library.UnaryOperators.TryAdd(GenUnary(TokenType.Minus, RipplePrimitives.Int32Name));
            library.UnaryOperators.TryAdd(GenUnary(TokenType.Minus, RipplePrimitives.Float32Name));
            library.UnaryOperators.TryAdd(GenUnary(TokenType.Bang, RipplePrimitives.BoolName));
        }

        private static void AppendCastOperators(ref OperatorLibrary operatorLibrary)
        {
            OperatorInfo.Cast intToFloat = new OperatorInfo.Cast(RipplePrimitives.Int32, RipplePrimitives.Float32);
            OperatorInfo.Cast floatToInt = new OperatorInfo.Cast(RipplePrimitives.Float32, RipplePrimitives.Int32);
            operatorLibrary.CastOperators.TryAdd(floatToInt);
            operatorLibrary.CastOperators.TryAdd(intToFloat);
        }

        public static FunctionList GetBuiltInFunctions()
        {
            var infos = new List<FunctionInfo>()
            {
                GenFunctionData("print", new (){(RipplePrimitives.Int32Name,    "value")}, RipplePrimitives.VoidName),
                GenFunctionData("print", new (){(RipplePrimitives.Float32Name,  "value")}, RipplePrimitives.VoidName),
                GenFunctionData("print", new (){(RipplePrimitives.BoolName,     "value")}, RipplePrimitives.VoidName),
            };

            FunctionList list = new FunctionList();

            foreach (FunctionInfo info in infos)
                list.TryAddFunction(info);

            return list;
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

            return new FunctionInfo(false, funcName, parameterInfos, returnType);
        }

        private static List<OperatorInfo.Binary> GenBinaries(string typeName, params TokenType[] operatorTypes)
        {
            List<OperatorInfo.Binary> operatorDatas = new List<OperatorInfo.Binary>();
            foreach (TokenType operatorType in operatorTypes)
                operatorDatas.Add(GenBinary(operatorType, typeName));

            return operatorDatas;
        }

        private static OperatorInfo.Binary GenBinary(TokenType operatorType, string typeName)
        {
            if (operatorType.IsType(TokenType.EqualEqual, TokenType.GreaterThanEqual, TokenType.BangEqual, 
                TokenType.LessThanEqual, TokenType.GreaterThan, TokenType.LessThan))
                return new OperatorInfo.Binary(GenBasicType(typeName), GenBasicType(typeName), operatorType, RipplePrimitives.Bool);

            return new OperatorInfo.Binary(GenBasicType(typeName), GenBasicType(typeName), operatorType, GenBasicType(typeName));
        }

        private static OperatorInfo.Unary GenUnary(TokenType operatorType, string typeName)
        {
            return new OperatorInfo.Unary(GenBasicType(typeName), operatorType, GenBasicType(typeName));
        }

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
