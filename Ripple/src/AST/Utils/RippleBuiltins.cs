using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Utils.Extensions;
using Ripple.AST.Info;
using Ripple.Utils;
using Ripple.AST.Info.Types;

namespace Ripple.AST.Utils
{
    static class RippleBuiltins
    {
        public static List<string> GetPrimitives()
        {
            return new List<string>()
            {
                RipplePrimitives.Int32Name,
                RipplePrimitives.Float32Name,
                RipplePrimitives.BoolName,
                RipplePrimitives.VoidName,
                RipplePrimitives.CharName,
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
                if(op == TokenType.Plus && left.Type is  PointerInfo p && right.Type.Equals(RipplePrimitives.Int32))
                {
                    ValueInfo info = new ValueInfo(p, lifetime);
                    return new Option<ValueInfo>(info);
                }

                return new Option<ValueInfo>();
            });

            library.Binaries.AddOperatorEvaluator((op, args, lifetime) => // assignment operators
            {
                (var left, var right) = args;
                if (left.Type.IsMutable())
                {
                    if(left.Type.EqualsWithoutFirstMutable(right.Type))
                    {
                        ValueInfo info = new ValueInfo(left.Type, lifetime);
                        return new Option<ValueInfo>(info);
                    }
                    else if(left.Type is ReferenceInfo rl && right.Type is ReferenceInfo rr &&
                            rr.Lifetime.Value.IsAssignableTo(rl.Lifetime.Value))
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
                if(op == TokenType.Star && operand.Type is PointerInfo p)
                {
                    ValueInfo info = new ValueInfo(p.Contained, operand.Lifetime);
                    return new Option<ValueInfo>(info);
                }
                else if (op == TokenType.Star && operand.Type is ReferenceInfo r)
                {
                    ValueInfo info = new ValueInfo(r.Contained, r.Lifetime.Value);
                    return new Option<ValueInfo>(info);
                }
                else if(op == TokenType.Ampersand)
                {
                    TypeInfo type = new ReferenceInfo(false, operand.Type.SetFirstMutable(false), operand.Lifetime);
                    ValueInfo value = new ValueInfo(type, lifetime);
                    return new Option<ValueInfo>(value);
                }
                else if(op == TokenType.RefMut && operand.Type.IsMutable())
                {
                    TypeInfo type = new ReferenceInfo(false, operand.Type, operand.Lifetime);
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
                if(type is PointerInfo ptype && value.Type is PointerInfo pvalue && !(ptype.Contained.IsMutable() && !pvalue.Contained.IsMutable()))
                {
                    return new Option<ValueInfo>(new ValueInfo(type, lifetime));
                }

                return new Option<ValueInfo>();
            });

            library.Casts.AddOperatorEvaluator((type, value, lifetime) => // reference -> pointer casts
            {
                if (type is PointerInfo ptype && value.Type is PointerInfo rvalue && !(ptype.Contained.IsMutable() && !rvalue.Contained.IsMutable()))
                {
                    if(ptype.Contained.EqualsWithoutFirstMutable(rvalue.Contained))
                        return new Option<ValueInfo>(new ValueInfo(type, lifetime));
                }

                return new Option<ValueInfo>();
            });

            library.Casts.AddOperatorEvaluator((type, value, lifetime) => // pointer -> reference casts
            {
                if (type is ReferenceInfo rtype && value.Type is PointerInfo pvalue && !(rtype.Contained.IsMutable() && !pvalue.Contained.IsMutable()))
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

        private static TypeInfo GenBasicType(string name)
        {
            return new BasicTypeInfo(false, name);
        }
    }
}
