using System;
using System.Collections.Generic;
using System.Linq;
using Ripple.Lexing;
using Ripple.Utils;
using Ripple.AST.Utils;
using Ripple.Validation.Info.Types;
using Ripple.AST;
using Ripple.Validation.Info.Expressions;
using Raucse;
using Ripple.Validation.Errors;
using Ripple.Validation.Errors.ExpressionErrors;
using Ripple.Validation.Info.Functions;
using Ripple.Validation.Info.Values;
using Ripple.Validation.Info.Lifetimes;

namespace Ripple.Validation.Info.Checking
{
    public class ExpressionCheckerVisitor : IExpressionVisitor<Pair<ValueInfo, TypedExpression>, Option<ExpectedValue>>
    {
        private readonly LocalVariableStack m_VariableStack;
        private readonly FunctionList m_Functions;
        private readonly IReadOnlyList<string> m_Primaries;
        private readonly OperatorEvaluatorLibrary m_OperatorLibrary;
        private readonly IReadOnlyDictionary<string, VariableInfo> m_Globals;
        private readonly SafetyContext m_SafetyContext;
        private readonly List<string> m_ActiveLifetimes;

        public ExpressionCheckerVisitor(ASTData astInfo, LocalVariableStack variableStack, List<string> activeLifetimes, SafetyContext safetyContext)
        {
            m_Functions = astInfo.Functions;
            m_Globals = astInfo.GlobalVariables;
            m_OperatorLibrary = astInfo.OperatorLibrary;
            m_VariableStack = variableStack;
            m_SafetyContext = safetyContext;
            m_Primaries = astInfo.PrimaryTypes;
            m_ActiveLifetimes = activeLifetimes;
        }

        public ExpressionCheckerVisitor(LocalVariableStack variableStack, FunctionList functions, OperatorEvaluatorLibrary operatorLibrary, Dictionary<string, VariableInfo> globals, SafetyContext safetyContext, List<string> primaries, List<string> activeLifetimes)
        {
            m_VariableStack = variableStack;
            m_Functions = functions;
            m_OperatorLibrary = operatorLibrary;
            m_Globals = globals;
            m_Primaries = primaries;
            m_SafetyContext = safetyContext;
            m_ActiveLifetimes = activeLifetimes;
        }

        public Pair<ValueInfo, TypedExpression> VisitBinary(Binary binary, Option<ExpectedValue> expected)
        {
            LifetimeInfo currentLifetime = m_VariableStack.CurrentLifetime;
            var left = binary.Left.Accept(this, expected);
            var right = binary.Right.Accept(this, expected);

            TokenType operatorType = binary.Op.Type;

            return m_OperatorLibrary.Binaries.Evaluate(operatorType, (left.First, right.First), currentLifetime, binary.GetLocation()).Match(
                ok =>
                {
                    TypedBinary bin = new TypedBinary(left.Second, operatorType, right.Second, ok.Type);
                    return new Pair<ValueInfo, TypedExpression>(ok, bin);
                },
                fail => throw new ExpressionCheckerException(fail));
        }

        public Pair<ValueInfo, TypedExpression> VisitUnary(Unary unary, Option<ExpectedValue> expected)
        {
            Option<ExpectedValue> innerExpected = new Option<ExpectedValue>();
            TokenType operatorType = unary.Op.Type;

            expected.Match(e =>
            {
                if (operatorType.IsType(TokenType.Minus, TokenType.Bang))
                {
                    innerExpected = e;
                }
                else if (e.Type is ReferenceInfo r)
                {
                    if (operatorType == TokenType.Ampersand)
                        innerExpected = new ExpectedValue(r.Contained, false);
                    else if (operatorType == TokenType.RefMut && r.IsMutable)
                        innerExpected = new ExpectedValue(r.Contained, true);
                }
            });

            var operand = unary.Expr.Accept(this, innerExpected);

            return m_OperatorLibrary.Unaries.Evaluate(operatorType, operand.First, m_VariableStack.CurrentLifetime, unary.GetLocation()).Match(
                ok => 
                {
                    return new Pair<ValueInfo, TypedExpression>(ok, new TypedUnary(operand.Second, operatorType, ok.Type));
                },
                fail =>
                {
                    throw new ExpressionCheckerException(fail);
                });
        }

        public Pair<ValueInfo, TypedExpression> VisitCall(Call call, Option<ExpectedValue> expected)
        {
            if (call.Callee is Identifier id &&
                !m_VariableStack.ContainsVariable(id.Name.Text) &&
                !m_Globals.ContainsKey(id.Name.Text) &&
                 m_Functions.ContainsFunctionWithName(id.Name.Text))
            {
                return CallFunction(call, id);
            }

            var callee = call.Callee.Accept(this, new Option<ExpectedValue>());
            var args = call.Args.Select(a => a.Accept(this, new Option<ExpectedValue>()));
            return m_OperatorLibrary.Calls.Evaluate(callee.First, args.Select(a => a.First), m_VariableStack.CurrentLifetime, call.GetLocation()).Match(
                ok => 
                {
                    TypedCall typedCall = new TypedCall(callee.Second, args.Select(a => a.Second).ToList(), ok.Type);
                    return new Pair<ValueInfo, TypedExpression>(ok, typedCall);
                },
                fail => throw new ExpressionCheckerException(fail));
        }

        private Pair<ValueInfo, TypedExpression> CallFunction(Call call, Identifier id)
        {
            string name = id.Name.Text;
            int argCount = call.Args.Count;
            List<Option<ValueInfo>> arguments = new List<Option<ValueInfo>>();
            foreach (Expression expression in call.Args)
            {
                try
                {
                    arguments.Add(expression.Accept(this, new Option<ExpectedValue>()).First);
                }
                catch (ExpressionCheckerException)
                {
                    arguments.Add(new Option<ValueInfo>());
                }
            }

            List<FunctionInfo> possibleCalled = new List<FunctionInfo>();
            foreach (FunctionInfo info in m_Functions.GetOverloadsWithName(name))
            {
                if (argCount != info.Parameters.Count)
                    continue;

                bool isValid = true;
                for (int i = 0; argCount > i; i++)
                {
                    Option<TypeInfo> argInfo = arguments[i].MatchOrConstruct(ok => new Option<TypeInfo>(ok.Type));
                    TypeInfo expectedParam = info.Parameters[i].Type;
                    if (argInfo.HasValue() && !argInfo.Value.EqualsWithoutLifetimes(expectedParam))
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                    possibleCalled.Add(info);
            }

            if (possibleCalled.Count == 0)
                throw new ExpressionCheckerException(new NoFunctionFoundError(id.Name.Location));

            if (possibleCalled.Count > 1)
                throw new ExpressionCheckerException(new OverloadDisambiguationError(id.Name.Location));

            for (int i = 0; argCount > i; i++)
            {
                ExpectedValue expectedParam = new ExpectedValue(possibleCalled[0].Parameters[i].Type, false);
                call.Args[i].Accept(this, expectedParam); // double checks to make sure that everything can be infered, like initalizer lists
            }

            FunctionInfo funcInfo = possibleCalled[0];

            if (funcInfo.IsUnsafe && m_SafetyContext.IsSafe)
                throw new ExpressionCheckerException(UnsafeThingIsInSafeContextError.Function(call.GetLocation(), name));

            IEnumerable<ValueInfo> args = arguments.Select(at => at.Match(ok => ok, () => throw new ExpressionCheckerException(new InvalidExpressionError(id.GetLocation()))));

            var typedArgs = call.Args.Zip(funcInfo.Parameters.Select(p => p.Type)).Select(tuple =>
            {
                var (expression, expected) = tuple;
                return expression.Accept(this, new ExpectedValue(expected, false)).Second;
            }).ToList();

            return m_OperatorLibrary.Calls.Evaluate(new ValueInfo(funcInfo.FunctionType, LifetimeInfo.Static, false, ValueCatagory.LValue), args, m_VariableStack.CurrentLifetime, id.GetLocation())
                .Match(
                ok =>
                {
                    TypedIdentifier id = new TypedIdentifier(funcInfo.Name, funcInfo, funcInfo.FunctionType);
                    TypedCall typedCall = new TypedCall(id, typedArgs, ok.Type);
                    return new Pair<ValueInfo, TypedExpression>(ok, typedCall);
                },
                fail => 
                {
                    throw new ExpressionCheckerException(fail);
                });
        }

        public Pair<ValueInfo, TypedExpression> VisitCast(Cast cast, Option<ExpectedValue> expected)
        {
            TypeInfo typeToCastTo = TypeInfoUtils.FromASTType(cast.TypeToCastTo, m_Primaries, m_ActiveLifetimes, m_SafetyContext)
                .Match(ok => ok, fail => throw new ExpressionCheckerException(fail)); 

            if (cast.Castee is Identifier id) // casting to find a function overload
            {
                string name = id.Name.Text;
                List<FunctionInfo> funcOverloads = m_Functions.GetOverloadsWithName(name);
                if (!m_VariableStack.ContainsVariable(name) && funcOverloads.Count > 0)
                {
                    foreach (FunctionInfo overload in funcOverloads)
                    {
                        FuncPtrInfo type = overload.FunctionType;
                        if (type.IsEquatableTo(typeToCastTo))
						{
                            ValueInfo value = new ValueInfo(type, LifetimeInfo.Static, false, ValueCatagory.LValue);
                            TypedIdentifier functionId = new TypedIdentifier(name, overload, type);
                            return new Pair<ValueInfo, TypedExpression>(value, functionId);
                        }
                    }
                }
            }

            var castee = cast.Castee.Accept(this, new ExpectedValue(typeToCastTo, false));

            return m_OperatorLibrary.Casts.Evaluate(typeToCastTo, castee.First, m_VariableStack.CurrentLifetime, cast.GetLocation()).Match(
                ok =>
                {
                    TypedCast typedCast = new TypedCast(castee.Second, typeToCastTo, ok.Type);
                    return new Pair<ValueInfo, TypedExpression>(ok, typedCast);
                },
                fail => throw new ExpressionCheckerException(fail));
        }

        public Pair<ValueInfo, TypedExpression> VisitGrouping(Grouping grouping, Option<ExpectedValue> expected)
        {
            return grouping.Expr.Accept(this, expected);
        }

        public Pair<ValueInfo, TypedExpression> VisitIdentifier(Identifier identifier, Option<ExpectedValue> expected)
        {
            string name = identifier.Name.Text;
            if (m_VariableStack.TryGetVariable(name, out VariableInfo info))
            {
                if(info.IsUnsafe && m_SafetyContext.IsSafe)
                    throw new ExpressionCheckerException(UnsafeThingIsInSafeContextError.Variable(identifier.GetLocation(), name));
                return GetVariablePair(info);
            }

            if (m_Globals.TryGetValue(name, out info))
            {
                if (info.IsUnsafe && m_SafetyContext.IsSafe)
                    throw new ExpressionCheckerException(UnsafeThingIsInSafeContextError.Variable(identifier.GetLocation(), name));
                return GetVariablePair(info);
            }

            List<FunctionInfo> functions = m_Functions.GetOverloadsWithName(name);

            if (expected.HasValue() && expected.Value.Type is FuncPtrInfo)
            {
                foreach (FunctionInfo funcInfo in functions)
                {
                    TypeInfo funcType = funcInfo.FunctionType;
                    if (funcType.IsEquatableTo(expected.Value.Type))
                    {
                        if (funcInfo.IsUnsafe && m_SafetyContext.IsSafe)
							throw new ExpressionCheckerException(UnsafeThingIsInSafeContextError.Function(identifier.GetLocation(), name));

                        return GetFunctionPair(funcInfo);
                    }
                }
            }

            if (functions.Count == 1)
            {
                FunctionInfo funcInfo = functions[0];
                return GetFunctionPair(funcInfo);
            }
            else if (functions.Count > 1)
            {
                throw new ExpressionCheckerException(new OverloadDisambiguationError(identifier.GetLocation()));
            }
            else
            {
                throw new ExpressionCheckerException(new DefinitionError.Variable(identifier.GetLocation(), false, name));
            }
        }

        public Pair<ValueInfo, TypedExpression> VisitIndex(AST.Index index, Option<ExpectedValue> expected)
        {
            var indexed = index.Indexed.Accept(this, new Option<ExpectedValue>());
            var arg = index.Argument.Accept(this, new ExpectedValue(RipplePrimitives.Int32, false));
            return m_OperatorLibrary.Indexers.Evaluate(indexed.First, arg.First, m_VariableStack.CurrentLifetime, index.GetLocation()).Match(
                ok =>
                {
                    TypedIndex typedIndex = new TypedIndex(indexed.Second, arg.Second, ok.Type);
                    return new Pair<ValueInfo, TypedExpression>(ok, typedIndex);
                },
                fail => throw new ExpressionCheckerException(fail));
        }

        public Pair<ValueInfo, TypedExpression> VisitInitializerList(InitializerList initializerList, Option<ExpectedValue> expected)
        {
            if (expected.HasValue() && expected.Value.Type is ArrayInfo array)
            {
                foreach (Expression expression in initializerList.Expressions)
                {
                    TypeInfo info = expression.Accept(this, new ExpectedValue(array.Contained, false)).First.Type;
                    if (!info.IsEquatableTo(array.Contained))
                    {
                        string message = "Expected an array of: " +
                            array.Contained.ToPrettyString() +
                            ", but found an expression for: " +
                            info.ToPrettyString() + ".";

                        throw new ExpressionCheckerException(new ArrayInitalizerListTypeArrayError(initializerList.GetLocation(), array.Contained, info));
                    }
                }

                if (initializerList.Expressions.Count > array.Size)
                    throw new ExpressionCheckerException(new InitalizerListSizeError(initializerList.GetLocation()));

                ValueInfo value = new ValueInfo(array, m_VariableStack.CurrentLifetime, false, ValueCatagory.RValue);
                var typedExpressions = initializerList.Expressions
                    .Select(e => e.Accept(this, new ExpectedValue(array.Contained, false)))
                    .Select(e => e.Second)
                    .ToList();

                TypedInitalizerList typedInitalizerList = new TypedInitalizerList(typedExpressions, array);
                return new Pair<ValueInfo, TypedExpression>(value, typedInitalizerList);
            }

            throw new ExpressionCheckerException(CouldNotInferExpressionExeption.Initalizer(initializerList.GetLocation()));
        }

        public Pair<ValueInfo, TypedExpression> VisitLiteral(Literal literal, Option<ExpectedValue> expected)
        {
            switch (literal.Val.Type)
            {
                case TokenType.IntagerLiteral:
                    return expected.Match(e =>
                    {
                        if (e.Type is BasicTypeInfo b)
                        {
                            if (b.Name == RipplePrimitives.Int32Name)
                                return GetLiteralPair(b, literal.Val.Type, literal.Val.Text, e.IsMutable);
                        }

                        return GetLiteralPair(RipplePrimitives.Int32, literal.Val.Type, literal.Val.Text, false);
                    },
                    () => GetLiteralPair(RipplePrimitives.Int32, literal.Val.Type, literal.Val.Text, false));

                case TokenType.FloatLiteral:
                    return expected.Match(e =>
                    {
                        if (e.Type is BasicTypeInfo b && b.Name == RipplePrimitives.Float32Name) // will return accurate mutable value
                            return GetLiteralPair(b, literal.Val.Type, literal.Val.Text, e.IsMutable);

                        return GetLiteralPair(RipplePrimitives.Float32, literal.Val.Type, literal.Val.Text, false);
                    },
                    () => GetLiteralPair(RipplePrimitives.Float32, literal.Val.Type, literal.Val.Text, false));

                case TokenType.True:
                case TokenType.False:
                    return expected.Match(e =>
                    {
                        if (e.Type is BasicTypeInfo b && b.Name == RipplePrimitives.BoolName)
                            return GetLiteralPair(b, literal.Val.Type, literal.Val.Text, e.IsMutable);
                        return GetLiteralPair(RipplePrimitives.Bool, literal.Val.Type, literal.Val.Text, false);
                    },
                    () => GetLiteralPair(RipplePrimitives.Bool, literal.Val.Type, literal.Val.Text, false));

                case TokenType.CharactorLiteral:
                    return expected.Match(e =>
                    {
                        if (e.Type is BasicTypeInfo b && b.Name == RipplePrimitives.CharName)
                            return GetLiteralPair(b, literal.Val.Type, literal.Val.Text, e.IsMutable);
                        return GetLiteralPair(RipplePrimitives.Char, literal.Val.Type, literal.Val.Text, false);
                    },
                    () => GetLiteralPair(RipplePrimitives.Char, literal.Val.Type, literal.Val.Text, false));

                case TokenType.Nullptr:
                    if (expected.Match(e => e.Type is PointerInfo, () => false))
                    {
                        return GetLiteralPair(expected.Value.Type, TokenType.Nullptr, "nullptr", expected.Value.IsMutable);
                    }
                    throw new ExpressionCheckerException(CouldNotInferExpressionExeption.Nullptr(literal.Val.Location));

                case TokenType.CStringLiteral:
                    {
                        TypeInfo type = new PointerInfo(false, RipplePrimitives.Char);
                        ValueInfo value = new ValueInfo(type, LifetimeInfo.Static, false, ValueCatagory.RValue); // char* with a static lifetime
                        TypedLiteral typedLiteral = new TypedLiteral(literal.Val.Text, literal.Val.Type, type);

                        return new Pair<ValueInfo, TypedExpression>(value, typedLiteral);
                    }

                default:
                    throw new ArgumentException("Unknown literal.");
            }
        }

        public Pair<ValueInfo, TypedExpression> VisitSizeOf(SizeOf sizeOf, Option<ExpectedValue> expected)
        {
            TypeInfo typeToCastTo = GetTypeInfo(sizeOf.Type);
            return expected.Match(e =>
            {
                if (e.Type is BasicTypeInfo b && b.Name == RipplePrimitives.Int32Name)
                    return GetSizeOfPair(typeToCastTo);
                return GetSizeOfPair(typeToCastTo);
            }, () => GetSizeOfPair(typeToCastTo));
        }

        private static Pair<ValueInfo, TypedExpression> GetVariablePair(VariableInfo variableInfo)
		{
            ValueInfo value = new ValueInfo(variableInfo.Type, variableInfo.Lifetime, variableInfo.IsMutable, ValueCatagory.LValue);
            TypedIdentifier identifier = new TypedIdentifier(variableInfo.Name, variableInfo, variableInfo.Type);
            return new Pair<ValueInfo, TypedExpression>(value, identifier);
		}

        private static Pair<ValueInfo, TypedExpression> GetFunctionPair(FunctionInfo functionInfo)
		{
            ValueInfo value = new ValueInfo(functionInfo.FunctionType, LifetimeInfo.Static, false, ValueCatagory.LValue);
            TypedIdentifier identifier = new TypedIdentifier(functionInfo.Name, functionInfo, functionInfo.ReturnType);
            return new Pair<ValueInfo, TypedExpression>(value, identifier);
		}

        private Pair<ValueInfo, TypedExpression> GetSizeOfPair(TypeInfo sizeOfType)
        {
            ValueInfo value = new ValueInfo(sizeOfType, m_VariableStack.CurrentLifetime, false, ValueCatagory.RValue);
            TypedSizeOf typedSizeOf = new TypedSizeOf(sizeOfType, RipplePrimitives.Int32);
            return new Pair<ValueInfo, TypedExpression>(value, typedSizeOf);
        }

        private Pair<ValueInfo, TypedExpression> GetLiteralPair(TypeInfo typeInfo, TokenType literalType, string literalValue, bool isMutable)
		{
            ValueInfo value = new ValueInfo(typeInfo, m_VariableStack.CurrentLifetime, isMutable, ValueCatagory.RValue);
            TypedLiteral literal = new TypedLiteral(literalValue, literalType, typeInfo);
            return new Pair<ValueInfo, TypedExpression>(value, literal);
		}

        private TypeInfo GetTypeInfo(TypeName type)
        {
            return TypeInfoUtils.FromASTType(type, m_Primaries, m_ActiveLifetimes, m_SafetyContext)
                .Match(ok => ok, fail => throw new ExpressionCheckerException(fail));
        }

        public Pair<ValueInfo, TypedExpression> VisitMemberAccess(MemberAccess memberAccess, Option<ExpectedValue> arg)
        {
            throw new NotImplementedException();
        }
    }
}
