using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Utils.Extensions;
using Ripple.AST.Info;
using Ripple.Utils;

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

        public static OperatorEvaluatorLibrary GetBuiltInOperators()
        {
            OperatorEvaluatorLibrary library = new OperatorEvaluatorLibrary();
            AppendBinaryOperators(ref library);
            AppendUnaryOperators(ref library);
            AppendCastOperators(ref library);
            return library;
        }

        private static void AppendBinaryOperators(ref OperatorEvaluatorLibrary library)
        {
            TokenType[] intagerOperators =      { TokenType.Plus, TokenType.Minus, TokenType.Star, TokenType.Slash, TokenType.Mod, TokenType.EqualEqual, TokenType.BangEqual, TokenType.GreaterThan, TokenType.GreaterThanEqual, TokenType.LessThan, TokenType.LessThanEqual };
            TokenType[] floatOperators =        { TokenType.Plus, TokenType.Minus, TokenType.Star, TokenType.Slash, TokenType.EqualEqual, TokenType.BangEqual, TokenType.GreaterThan, TokenType.GreaterThanEqual, TokenType.LessThan, TokenType.LessThanEqual };
            TokenType[] boolOperators =         { TokenType.EqualEqual, TokenType.BangEqual, TokenType.AmpersandAmpersand, TokenType.PipePipe };
            TokenType[] charactorOperators =    { TokenType.EqualEqual, TokenType.BangEqual };

            AddBinaryOperatorsForType(ref library, RipplePrimitives.Int32,      intagerOperators);
            AddBinaryOperatorsForType(ref library, RipplePrimitives.Float32,    floatOperators);
            AddBinaryOperatorsForType(ref library, RipplePrimitives.Bool,       boolOperators);
            AddBinaryOperatorsForType(ref library, RipplePrimitives.Char,       charactorOperators);

            library.Binaries.AddOperatorEvaluator((op, args, lifetime) => // pointer arithmatic operators
            {
                (var left, var right) = args;
                if(op == TokenType.Plus && left.Type is TypeInfo.Pointer p && right.Type.Equals(RipplePrimitives.Int32))
                {
                    ValueInfo info = new ValueInfo(p, lifetime);
                    return new Option<ValueInfo>(info);
                }

                return new Option<ValueInfo>();
            });

            library.Binaries.AddOperatorEvaluator((op, args, lifetime) => // assignment operators
            {
                (var left, var right) = args;
                if (left.Type.Mutable)
                {
                    if(left.Type.EqualsWithoutFirstMutable(right.Type))
                    {
                        ValueInfo info = new ValueInfo(left.Type, lifetime);
                        return new Option<ValueInfo>(info);
                    }
                    else if(left.Type is TypeInfo.Reference rl && right.Type is TypeInfo.Reference rr &&
                            rr.Lifetime.IsAssignableTo(rl.Lifetime))
                    {
                        ValueInfo info = new ValueInfo(left.Type, lifetime);
                        return new Option<ValueInfo>(info);
                    }
                }

                return new Option<ValueInfo>();
            });
        }

        private static void AppendUnaryOperators(ref OperatorEvaluatorLibrary library)
        {
            AddUnaryOperatorsForType(ref library, RipplePrimitives.Int32, TokenType.Minus);
            AddUnaryOperatorsForType(ref library, RipplePrimitives.Float32, TokenType.Minus);
            AddUnaryOperatorsForType(ref library, RipplePrimitives.Bool, TokenType.Bang);

            library.Unaries.AddOperatorEvaluator((op, operand, lifetime) => // dereference and reference of operators
            {
                if(op == TokenType.Star && operand.Type is TypeInfo.Pointer p)
                {
                    ValueInfo info = new ValueInfo(p.Contained, operand.Lifetime);
                    return new Option<ValueInfo>(info);
                }
                else if (op == TokenType.Star && operand.Type is TypeInfo.Reference r)
                {
                    ValueInfo info = new ValueInfo(r.Contained, r.Lifetime);
                    return new Option<ValueInfo>(info);
                }
                else if(op == TokenType.Ampersand)
                {
                    TypeInfo type = new TypeInfo.Reference(false, operand.Type.ChangeMutable(false), operand.Lifetime);
                    ValueInfo value = new ValueInfo(type, lifetime);
                    return new Option<ValueInfo>(value);
                }
                else if(op == TokenType.RefMut && operand.Type.Mutable)
                {
                    TypeInfo type = new TypeInfo.Reference(false, operand.Type, operand.Lifetime);
                    ValueInfo value = new ValueInfo(type, lifetime);
                    return new Option<ValueInfo>(value);
                }

                return new Option<ValueInfo>();
            });
        }

        private static void AppendCastOperators(ref OperatorEvaluatorLibrary library)
        {
            library.Casts.AddOperatorEvaluator((type, value, lifetime) => // identity casts: int -> int
            {
                if(type.EqualsWithoutFirstMutable(value.Type))
                {
                    ValueInfo info = new ValueInfo(type, lifetime);
                    return new Option<ValueInfo>(info);
                }

                return new Option<ValueInfo>();
            });

            library.Casts.AddOperatorEvaluator((type, value, lifetime) => // pointer -> pointer casts
            {
                if(type is TypeInfo.Pointer ptype && value.Type is TypeInfo.Pointer pvalue && !(ptype.Contained.Mutable && !pvalue.Contained.Mutable))
                {
                    return new Option<ValueInfo>(new ValueInfo(type, lifetime));
                }

                return new Option<ValueInfo>();
            });

            library.Casts.AddOperatorEvaluator((type, value, lifetime) => // reference -> pointer casts
            {
                if (type is TypeInfo.Pointer ptype && value.Type is TypeInfo.Reference rvalue && !(ptype.Contained.Mutable && !rvalue.Contained.Mutable))
                {
                    if(ptype.Contained.EqualsWithoutFirstMutable(rvalue.Contained))
                        return new Option<ValueInfo>(new ValueInfo(type, lifetime));
                }

                return new Option<ValueInfo>();
            });

            library.Casts.AddOperatorEvaluator((type, value, lifetime) => // pointer -> reference casts
            {
                if (type is TypeInfo.Reference rtype && value.Type is TypeInfo.Pointer pvalue && !(rtype.Contained.Mutable && !pvalue.Contained.Mutable))
                {
                    if (rtype.Contained.EqualsWithoutFirstMutable(pvalue.Contained))
                        return new Option<ValueInfo>(new ValueInfo(type, lifetime));
                }

                return new Option<ValueInfo>();
            });
        }

        private static void AddBinaryOperatorsForType(ref OperatorEvaluatorLibrary library, TypeInfo type, params TokenType[] operators)
        {
            library.Binaries.AddOperatorEvaluator((op, args, lifetime) => 
            {
                (var left, var right) = args;
                if (operators.Contains(op) && left.Type.Equals(type) && right.Type.Equals(type))
                {
                    if(op.IsType(TokenType.EqualEqual, TokenType.BangEqual, TokenType.GreaterThan, 
                                 TokenType.GreaterThanEqual, TokenType.LessThan, TokenType.LessThanEqual))
                    {
                        return new Option<ValueInfo>(new ValueInfo(RipplePrimitives.Bool, lifetime));
                    }
                    else
                    {
                        return new Option<ValueInfo>(new ValueInfo(type, lifetime));
                    }
                }

                return new Option<ValueInfo>();
            });
        }

        private static void AddUnaryOperatorsForType(ref OperatorEvaluatorLibrary library, TypeInfo type, params TokenType[] operators)
        {
            library.Unaries.AddOperatorEvaluator((op, arg, lifetime) =>
            {
                if (operators.Contains(op) && arg.Equals(op))
                {
                    return new Option<ValueInfo>(new ValueInfo(type, lifetime));
                }

                return new Option<ValueInfo>();
            });
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

            return new FunctionInfo(false, funcName, new List<Token>(), parameterInfos, returnType); 
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
