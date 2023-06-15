using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Validation.Info;
using Ripple.Utils;
using Ripple.Validation.Info.Types;
using Ripple.AST.Utils;
using Raucse;
using Raucse.Extensions;
using Ripple.AST;
using Ripple.Core;
using Ripple.Validation.Errors;
using Ripple.Validation.Info.Functions;
using Ripple.Validation.Info.Lifetimes;
using Ripple.Validation.Info.Checking;
using Ripple.Validation.Info.Values;

namespace Ripple.Validation
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
                //GenFunctionData("print", new (){(RipplePrimitives.Int32,    "value")}, RipplePrimitives.VoidName),
                //GenFunctionData("print", new (){(RipplePrimitives.Float32,  "value")}, RipplePrimitives.VoidName),
                //GenFunctionData("print", new (){(RipplePrimitives.Bool,     "value")}, RipplePrimitives.VoidName),
                //GenFunctionData("print", new (){(RipplePrimitives.CString , "value")}, RipplePrimitives.VoidName),
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
            AppendIndexOperators(ref library);
            AppendCallOperators(ref library);
            return library;
        }

        public static Linker GetBuiltInLinker()
        {
            List<Pair<FunctionInfo, string>> externals = new List<Pair<FunctionInfo, string>>();
            externals.Add(new Pair<FunctionInfo, string>(GenLinkingFunction("printf", new List<(TypeName, string)>()
            {
               (GenPointerType("char"), "fmt")
            }, RipplePrimitives.VoidName, true), "stdio.h"));

            return new Linker(externals);
        }

		private static void AppendCallOperators(ref OperatorEvaluatorLibrary library)
		{
            library.Calls.AddOperatorEvaluator((callee, args, lifetime) =>
			{
                if(callee.Type is not FuncPtrInfo fp)
                    return new Option<ValueInfo>();

                if(fp.LifetimeCount == 0)
				    return EvaluateBasicFuncPtr(fp, args, lifetime);

                return EvaluateFuncPtrWithLifetimes(fp, args, lifetime);
			});
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
                    ValueInfo info = new ValueInfo(p, lifetime, false, ValueCatagory.RValue);
                    return new Option<ValueInfo>(info);
                }

                return new Option<ValueInfo>();
            });

            library.Binaries.AddOperatorEvaluator((op, args, lifetime) => // assignment operators
            {
                (var left, var right) = args;
                if (op == TokenType.Equal && left.IsMutable)
                {
                    if(left.Type.Equals(right.Type))
                    {
                        ValueInfo info = new ValueInfo(left.Type, lifetime, false, ValueCatagory.RValue);
                        return new Option<ValueInfo>(info);
                    }
                    else if(left.Type is ReferenceInfo rl && right.Type is ReferenceInfo rr)
                    {
                        if(rr.Lifetime.HasValue() && rr.Lifetime.Value.IsLifetimeInfo && 
                           rl.Lifetime.HasValue() && rl.Lifetime.Value.IsLifetimeInfo)
						{
                            if (!rr.Lifetime.Value.GetLifetimeInfo().Value.IsAssignableTo(rl.Lifetime.Value.GetLifetimeInfo().Value))
                                return new Option<ValueInfo>();
                            
                            ValueInfo info = new ValueInfo(left.Type, lifetime, false, ValueCatagory.RValue);
                            return new Option<ValueInfo>(info);
                        }
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
                    ValueInfo info = new ValueInfo(p.Contained, operand.Lifetime, p.IsMutable, ValueCatagory.LValue);
                    return new Option<ValueInfo>(info);
                }
                else if (op == TokenType.Star && operand.Type is ReferenceInfo r)
                {
                    if(r.Lifetime.HasValue() && r.Lifetime.Value.IsLifetimeInfo)
					{
                        ValueInfo info = new ValueInfo(r.Contained, r.Lifetime.Value.GetLifetimeInfo().Value, r.IsMutable, ValueCatagory.LValue);
                        return new Option<ValueInfo>(info);
                    }
                }
                else if(op == TokenType.Ampersand)
                {
                    TypeInfo type = new ReferenceInfo(false, operand.Type, new ReferenceLifetime(operand.Lifetime));
                    ValueInfo value = new ValueInfo(type, lifetime, false, ValueCatagory.RValue);
                    return new Option<ValueInfo>(value);
                }
                else if(op == TokenType.RefMut && (operand.IsMutable || operand.Catagory == ValueCatagory.RValue))
                {
                    TypeInfo type = new ReferenceInfo(true, operand.Type, new ReferenceLifetime(operand.Lifetime));
                    ValueInfo value = new ValueInfo(type, lifetime, false, ValueCatagory.RValue);
                    return new Option<ValueInfo>(value);
                }

                return new Option<ValueInfo>();
            });
        }

        private static void AppendIndexOperators(ref OperatorEvaluatorLibrary library)
        {
            library.Indexers.AddOperatorEvaluator((indexed, arg, lifetime) => // array indexing
            {
                if(indexed.Type is ArrayInfo array && arg.Type.Equals(RipplePrimitives.Int32))
                {
                    ValueInfo info = new ValueInfo(array.Contained, lifetime, indexed.IsMutable, ValueCatagory.LValue);
                    return new Option<ValueInfo>(info);
                }

                return new Option<ValueInfo>();
            });

            library.Indexers.AddOperatorEvaluator((indexed, arg, lifetime) => // pointer indexing
            {
                if (indexed.Type is PointerInfo pointer && arg.Type.Equals(RipplePrimitives.Int32))
                {
                    ValueInfo info = new ValueInfo(pointer.Contained, lifetime, pointer.IsMutable, ValueCatagory.LValue);
                    return new Option<ValueInfo>(info);
                }

                return new Option<ValueInfo>();
            });
        }

        private static void AppendCastOperators(ref OperatorEvaluatorLibrary library)
        {
            AddCast(RipplePrimitives.Int32, RipplePrimitives.Float32, ref library); // int -> float
            AddCast(RipplePrimitives.Float32, RipplePrimitives.Int32, ref library); // float -> int

            library.Casts.AddOperatorEvaluator((type, value, lifetime) => // identity casts: int -> int
            {
                if(type.Equals(value.Type))
                {
                    ValueInfo info = new ValueInfo(type, lifetime, false, ValueCatagory.RValue);
                    return new Option<ValueInfo>(info);
                }

                return new Option<ValueInfo>();
            });

            library.Casts.AddOperatorEvaluator((type, value, lifetime) => // pointer -> pointer casts
            {
                if(type is PointerInfo ptype && value.Type is PointerInfo pvalue && !(ptype.IsMutable && !pvalue.IsMutable))
                {
                    return new Option<ValueInfo>(new ValueInfo(type, lifetime, false, ValueCatagory.RValue));
                }

                return new Option<ValueInfo>();
            });

            library.Casts.AddOperatorEvaluator((type, value, lifetime) => // reference -> pointer casts
            {
                if (type is PointerInfo ptype && value.Type is ReferenceInfo rvalue && !(ptype.IsMutable && !rvalue.IsMutable))
                {
                    if(ptype.Contained.Equals(rvalue.Contained))
                        return new Option<ValueInfo>(new ValueInfo(type, lifetime, false, ValueCatagory.RValue));
                }

                return new Option<ValueInfo>();
            });

            library.Casts.AddOperatorEvaluator((type, value, lifetime) => // pointer -> reference casts
            {
                if (type is ReferenceInfo rtype && value.Type is PointerInfo pvalue && !(rtype.IsMutable && !pvalue.IsMutable))
                {
                    if (rtype.Contained.Equals(pvalue.Contained))
                        return new Option<ValueInfo>(new ValueInfo(type, lifetime, false, ValueCatagory.RValue));
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
                        return new Option<ValueInfo>(new ValueInfo(RipplePrimitives.Bool, lifetime, false, ValueCatagory.RValue));
                    }
                    else
                    {
                        return new Option<ValueInfo>(new ValueInfo(type, lifetime, false, ValueCatagory.RValue));
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
                    return new Option<ValueInfo>(new ValueInfo(type, lifetime, false, ValueCatagory.RValue));
                }

                return new Option<ValueInfo>();
            });
        }

        private static void AddCast(TypeInfo valueType, TypeInfo castType, ref OperatorEvaluatorLibrary library)
        {
            library.Casts.AddOperatorEvaluator((type, value, lifetime) =>
            {
                if(valueType.Equals(value.Type) && castType.Equals(type))
                {
                    ValueInfo info = new ValueInfo(castType, lifetime, false, ValueCatagory.RValue);
                    return new Option<ValueInfo>(info);
                }
                return new Option<ValueInfo>();
            });
        }

        private static Option<ValueInfo> EvaluateBasicFuncPtr(FuncPtrInfo fp, IEnumerable<ValueInfo> args, LifetimeInfo lifetime)
        {
            List<TypeInfo> parameters = fp.Parameters;
            if (parameters.Count() != args.Count())
                return new Option<ValueInfo>();

            bool callable = parameters.Zip(args).All(t =>
            {
                (var param, var arg) = t;

                return arg.Type.Equals(param);
            });

            if (!callable)
                return new Option<ValueInfo>();

            return new Option<ValueInfo>(new ValueInfo(fp.Returned, lifetime, false, ValueCatagory.RValue));
        }
        
        private static Option<ValueInfo> EvaluateFuncPtrWithLifetimes(FuncPtrInfo fp, IEnumerable<ValueInfo> args, LifetimeInfo lifetime)
		{
            List<TypeInfo> parameters = fp.Parameters;
            if (parameters.Count() != args.Count())
                return new Option<ValueInfo>();

            var functionLifetimeValues = parameters.Zip(args)
                .Select(t =>
                {
                    (var p, var a) = t;
                    return GenericLifetimeValueEvaluator.GetGenericLifetimeValues(p, a.Type, fp.FunctionIndex);
                })
                .Aggregate((a, b) => 
                {
                    if (!a.HasValue() || !b.HasValue())
                        return new Option<Dictionary<int, List<LifetimeInfo>>>();

                    foreach((int index, List<LifetimeInfo> lifetimes) in b.Value)
				    {
                        a.Value.GetOrCreate(index).AddRange(lifetimes);
				    }

                    return a;
                });

            return functionLifetimeValues.Match(
                ok =>
                {
                    List<LifetimeInfo> lifetimes = ok.OrderBy(v => v.Key)
                        .Select(v => v.Value)
                        .Select(v => GetSmallestLifetime(v))
                        .ToList();

                    if (lifetimes.Count != fp.LifetimeCount)
                        return new Option<ValueInfo>();

                    TypeInfo fpType = FuncPointerInstantiator.InstantiateFunctionPointer(lifetimes, fp);
                    TypeInfo returned = (fpType as FuncPtrInfo).Returned;
                    return new Option<ValueInfo>(new ValueInfo(returned, lifetime, false, ValueCatagory.RValue));
                },
                () => 
                {
                    return new Option<ValueInfo>();
                });
        }

        private static LifetimeInfo GetSmallestLifetime(IEnumerable<LifetimeInfo> lifetimes)
		{
            return lifetimes.Aggregate((a, b) =>
            {
                return a.Match(alocal =>
                {
                    return b.Match(blocal =>
                    {
                        return alocal > blocal ? a : b; // whichever local is highest, is the smallest lifetime
                    },
                    bparam =>
                    {
                        return a;
                    });
                },
                aparam =>
                {
                    return b.Match(blocal =>
                    {
                        return b;
                    },
                    bparam =>
                    {
                        throw new ExpressionCheckerException(new CannotDicernBetweenLifetimesError(aparam.Location, aparam, bparam));
                    });
                });
            });
		}
        private static Token GenIdTok(string name)
        {
            return new Token(name, new SourceLocation(), TokenType.Identifier, false);
        }

        private static TypeName GenBasicTypeName(string name)
        {
            return new BasicType(GenIdTok(name));
        }

        private static TypeName GenPointerType(string name)
        {
            return new PointerType(GenBasicTypeName(name), null, new Token());
        }

        private static FunctionInfo GenLinkingFunction(string name, List<(TypeName, string)> paramaters, string returnTypeName, bool isUnsafe)
        {
            Parameters parameters = new Parameters(new Token(), paramaters.Select(p => new Pair<TypeName, Token>(p.Item1, GenIdTok(p.Item2))).ToList(), new Token());
            var result = FunctionInfo.FromASTExternalFunction(new ExternalFuncDecl(new Token(), new Token(), new Token(), new Token(), GenIdTok(name), parameters, new Token(), GenBasicTypeName(returnTypeName), new Token()), GetPrimitives());
            return result.Value;
        }
    }
}
