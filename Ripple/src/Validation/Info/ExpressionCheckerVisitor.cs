using System;
using System.Collections.Generic;
using System.Linq;
using Ripple.Lexing;
using Ripple.Utils;
using Ripple.AST.Utils;
using Ripple.Utils.Extensions;
using Ripple.Validation.Info.Types;
using Ripple.AST;
using Ripple.Validation.Info.Expressions;

namespace Ripple.Validation.Info
{
    class ExpressionCheckerVisitor : IExpressionVisitor<Pair<ValueInfo, TypedExpression>, Option<TypeInfo>>
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

        public Pair<ValueInfo, TypedExpression> VisitBinary(Binary binary, Option<TypeInfo> expected)
        {
            LifetimeInfo currentLifetime = m_VariableStack.CurrentLifetime;
            var left = binary.Left.Accept(this, expected);
            var right = binary.Right.Accept(this, expected);

            TokenType operatorType = binary.Op.Type;

            return m_OperatorLibrary.Binaries.Evaluate(operatorType, (left.First, right.First), currentLifetime, binary.Op).Match(
                ok =>
                {
                    TypedBinary bin = new TypedBinary(left.Second, operatorType, right.Second, ok.Type);
                    return new Pair<ValueInfo, TypedExpression>(ok, bin);
                },
                fail => throw new ExpressionCheckerException(fail.Message, fail.Token));
        }

        public Pair<ValueInfo, TypedExpression> VisitUnary(Unary unary, Option<TypeInfo> expected)
        {
            Option<TypeInfo> innerExpected = new Option<TypeInfo>();
            TokenType operatorType = unary.Op.Type;
            bool expectedMutable = expected.Match(ok => ok.IsMutable(), () => false);

            expected.Match(e =>
            {
                if (operatorType.IsType(TokenType.Minus, TokenType.Bang))
                {
                    innerExpected = e;
                }
                else if (e is ReferenceInfo r)
                {
                    if (operatorType == TokenType.Ampersand)
                        innerExpected = r.Contained.SetFirstMutable(false);
                    else if (operatorType == TokenType.RefMut && r.Contained.IsMutable())
                        innerExpected = r.Contained;
                }
            });

            var operand = unary.Expr.Accept(this, innerExpected);

            return m_OperatorLibrary.Unaries.Evaluate(operatorType, operand.First, m_VariableStack.CurrentLifetime, unary.Op).Match(
                ok => 
                {
                    if(operatorType.IsType(TokenType.Ampersand, TokenType.RefMut))
                    {
                        ValueInfo expectedValue = new ValueInfo(ok.Type.SetFirstMutable(expectedMutable), ok.Lifetime);
                        TypedUnary expectedTypedUnary = new TypedUnary(operand.Second, operatorType, ok.Type.SetFirstMutable(expectedMutable));
                        return new Pair<ValueInfo, TypedExpression>(expectedValue, expectedTypedUnary);
                    }

                    ValueInfo value = new ValueInfo(ok.Type, ok.Lifetime);
                    TypedUnary typedUnary = new TypedUnary(operand.Second, operatorType, ok.Type);
                    return new Pair<ValueInfo, TypedExpression>(value, typedUnary);
                },
                fail =>
                {
                    throw new ExpressionCheckerException(fail.Message, fail.Token);
                });
        }

        public Pair<ValueInfo, TypedExpression> VisitCall(Call call, Option<TypeInfo> expected)
        {
            if (call.Callee is Identifier id &&
                !m_VariableStack.ContainsVariable(id.Name.Text) &&
                !m_Globals.ContainsKey(id.Name.Text) &&
                 m_Functions.ContainsFunctionWithName(id.Name.Text))
            {
                return CallFunction(call, id);
            }

            var callee = call.Callee.Accept(this, new Option<TypeInfo>());
            var args = call.Args.Select(a => a.Accept(this, new Option<TypeInfo>()));
            return m_OperatorLibrary.Calls.Evaluate(callee.First, args.Select(a => a.First), m_VariableStack.CurrentLifetime, call.OpenParen).Match(
                ok => 
                {
                    TypedCall typedCall = new TypedCall(callee.Second, args.Select(a => a.Second).ToList(), ok.Type);
                    return new Pair<ValueInfo, TypedExpression>(ok, typedCall);
                },
                fail => throw new ExpressionCheckerException(fail.Message, fail.Token));
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
                    arguments.Add(expression.Accept(this, new Option<TypeInfo>()).First);
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
                    if (argInfo.HasValue() && !argInfo.Value.SetFirstMutable(false).EqualsWithoutLifetimes(expectedParam.SetFirstMutable(false)))
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                    possibleCalled.Add(info);
            }

            if (possibleCalled.Count == 0)
                throw new ExpressionCheckerException("No function with given arguments found.", id.Name);

            if (possibleCalled.Count > 1)
                throw new ExpressionCheckerException("Cannot disambiguate between given function overloads.", id.Name);

            for (int i = 0; argCount > i; i++)
            {
                TypeInfo expectedParam = possibleCalled[0].Parameters[i].Type;
                call.Args[i].Accept(this, expectedParam); // double checks to make sure that everything can be infered, like initalizer lists
            }

            FunctionInfo funcInfo = possibleCalled[0];

            if (funcInfo.IsUnsafe && m_SafetyContext.IsSafe)
				ThrowError("Use of unsafe function '" + funcInfo.Name + "' in a safe context.", call.OpenParen);

            IEnumerable<ValueInfo> args = arguments.Select(at => at.Match(ok => ok, () => throw new ExpressionCheckerException("Invalid expression at call.", id.Name)));

            var typedArgs = call.Args.Zip(funcInfo.Parameters.Select(p => p.Type)).Select(tuple =>
            {
                var (expression, expected) = tuple;
                return expression.Accept(this, expected).Second;
            }).ToList();

            return m_OperatorLibrary.Calls.Evaluate(new ValueInfo(funcInfo.FunctionType, LifetimeInfo.Static), args, m_VariableStack.CurrentLifetime, id.Name)
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

        public Pair<ValueInfo, TypedExpression> VisitCast(Cast cast, Option<TypeInfo> expected)
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
                            ValueInfo value = new ValueInfo(type, LifetimeInfo.Static);
                            TypedIdentifier functionId = new TypedIdentifier(name, overload, type);
                            return new Pair<ValueInfo, TypedExpression>(value, functionId);
                        }
                    }
                }
            }

            var castee = cast.Castee.Accept(this, typeToCastTo);

            return m_OperatorLibrary.Casts.Evaluate(typeToCastTo, castee.First, m_VariableStack.CurrentLifetime, cast.AsToken).Match(
                ok =>
                {
                    TypedCast typedCast = new TypedCast(castee.Second, typeToCastTo, ok.Type);
                    return new Pair<ValueInfo, TypedExpression>(ok, typedCast);
                },
                fail => throw new ExpressionCheckerException(fail));
        }

        public Pair<ValueInfo, TypedExpression> VisitGrouping(Grouping grouping, Option<TypeInfo> expected)
        {
            return grouping.Expr.Accept(this, expected);
        }

        public Pair<ValueInfo, TypedExpression> VisitIdentifier(Identifier identifier, Option<TypeInfo> expected)
        {
            string name = identifier.Name.Text;
            if (m_VariableStack.TryGetVariable(name, out VariableInfo info))
            {
                if(info.IsUnsafe && m_SafetyContext.IsSafe)
					ThrowError("Use of unsafe local variable '" + info.Name + "' in a safe context.", identifier.Name);
                return GetVariablePair(info);
            }

            if (m_Globals.TryGetValue(name, out info))
            {
                if (info.IsUnsafe && m_SafetyContext.IsSafe)
					ThrowError("Use of unsafe global variable '" + info.Name + "' in a safe context.", identifier.Name);
                return GetVariablePair(info);
            }

            List<FunctionInfo> functions = m_Functions.GetOverloadsWithName(name);

            if (expected.HasValue() && expected.Value is FuncPtrInfo)
            {
                foreach (FunctionInfo funcInfo in functions)
                {
                    TypeInfo funcType = funcInfo.FunctionType;
                    if (funcType.IsEquatableTo(expected.Value))
                    {
                        if (funcInfo.IsUnsafe && m_SafetyContext.IsSafe)
							ThrowError("Use of unsafe function '" + funcInfo.Name + "' in a safe context.", identifier.Name);

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
                string message = "Too many function overloads for function: " + name + " to distinguish.";
                throw new ExpressionCheckerException(message, identifier.Name);
            }
            else
            {
                string varMessage = "Variable: " + name + " is not defined.";
                throw new ExpressionCheckerException(varMessage, identifier.Name);
            }
        }

        public Pair<ValueInfo, TypedExpression> VisitIndex(AST.Index index, Option<TypeInfo> expected)
        {
            var indexed = index.Indexed.Accept(this, new Option<TypeInfo>());
            var arg = index.Argument.Accept(this, RipplePrimitives.Int32);
            return m_OperatorLibrary.Indexers.Evaluate(indexed.First, arg.First, m_VariableStack.CurrentLifetime, index.OpenBracket).Match(
                ok =>
                {
                    TypedIndex typedIndex = new TypedIndex(indexed.Second, arg.Second, ok.Type);
                    return new Pair<ValueInfo, TypedExpression>(ok, typedIndex);
                },
                fail => throw new ExpressionCheckerException(fail.Message, fail.Token));
        }

        public Pair<ValueInfo, TypedExpression> VisitInitializerList(InitializerList initializerList, Option<TypeInfo> expected)
        {
            if (expected.HasValue() && expected.Value is ArrayInfo array)
            {
                foreach (Expression expression in initializerList.Expressions)
                {
                    TypeInfo info = expression.Accept(this, array.Contained).First.Type;
                    if (!info.IsEquatableTo(array.Contained))
                    {
                        string message = "Expected an array of: " +
                            array.Contained.ToPrettyString() +
                            ", but found an expression for: " +
                            info.ToPrettyString() + ".";

                        throw new ExpressionCheckerException(message, initializerList.OpenBrace);
                    }
                }

                if (initializerList.Expressions.Count > array.Size)
                    throw new ExpressionCheckerException("Initializer list size is bigger than the array size.", initializerList.OpenBrace);

                ValueInfo value = new ValueInfo(array, m_VariableStack.CurrentLifetime);
                var typedExpressions = initializerList.Expressions
                    .Select(e => e.Accept(this, array.Contained))
                    .Select(e => e.Second)
                    .ToList();

                TypedInitalizerList typedInitalizerList = new TypedInitalizerList(typedExpressions, array);
                return new Pair<ValueInfo, TypedExpression>(value, typedInitalizerList);
            }

            throw new ExpressionCheckerException("Could not infer the type of the initializer list.", initializerList.OpenBrace);
        }

        public Pair<ValueInfo, TypedExpression> VisitLiteral(Literal literal, Option<TypeInfo> expected)
        {
            switch (literal.Val.Type)
            {
                case TokenType.IntagerLiteral:
                    return expected.Match(e =>
                    {
                        if (e is BasicTypeInfo b)
                        {
                            if (b.Name == RipplePrimitives.Int32Name)
                                return GetLiteralPair(b, literal.Val.Type, literal.Val.Text);
                            else if (b.Name == RipplePrimitives.Float32Name)
                                return GetLiteralPair(b, literal.Val.Type, literal.Val.Text);
                        }

                        return GetLiteralPair(RipplePrimitives.Int32, literal.Val.Type, literal.Val.Text);
                    },
                    () => GetLiteralPair(RipplePrimitives.Int32, literal.Val.Type, literal.Val.Text));

                case TokenType.FloatLiteral:
                    return expected.Match(e =>
                    {
                        if (e is BasicTypeInfo b && b.Name == RipplePrimitives.Float32Name) // will return accurate mutable value
                            return GetLiteralPair(b, literal.Val.Type, literal.Val.Text);

                        return GetLiteralPair(RipplePrimitives.Float32, literal.Val.Type, literal.Val.Text);
                    },
                    () => GetLiteralPair(RipplePrimitives.Float32, literal.Val.Type, literal.Val.Text));

                case TokenType.True:
                case TokenType.False:
                    return expected.Match(e =>
                    {
                        if (e is BasicTypeInfo b && b.Name == RipplePrimitives.BoolName)
                            return GetLiteralPair(b, literal.Val.Type, literal.Val.Text);
                        return GetLiteralPair(RipplePrimitives.Bool, literal.Val.Type, literal.Val.Text);
                    },
                    () => GetLiteralPair(RipplePrimitives.Bool, literal.Val.Type, literal.Val.Text));

                case TokenType.CharactorLiteral:
                    return expected.Match(e =>
                    {
                        if (e is BasicTypeInfo b && b.Name == RipplePrimitives.CharName)
                            return GetLiteralPair(b, literal.Val.Type, literal.Val.Text);
                        return GetLiteralPair(RipplePrimitives.Char, literal.Val.Type, literal.Val.Text);
                    },
                    () => GetLiteralPair(RipplePrimitives.Char, literal.Val.Type, literal.Val.Text));

                case TokenType.Nullptr:
                    if (expected.Match(e => e is PointerInfo, () => false))
                    {
                        return GetLiteralPair(expected.Value, TokenType.Nullptr, "nullptr");
                    }
                    throw new ExpressionCheckerException("Could not infer nullptr, in this context", literal.Val);

                case TokenType.CStringLiteral:
                    {
                        TypeInfo type = new PointerInfo(false, RipplePrimitives.Char);
                        ValueInfo value = new ValueInfo(type, LifetimeInfo.Static); // char* with a static lifetime
                        TypedLiteral typedLiteral = new TypedLiteral(literal.Val.Text, literal.Val.Type, type);

                        return new Pair<ValueInfo, TypedExpression>(value, typedLiteral);
                    }

                default:
                    throw new ExpressionCheckerException("Unknown literal.", literal.Val);
            }
        }

        public Pair<ValueInfo, TypedExpression> VisitSizeOf(SizeOf sizeOf, Option<TypeInfo> expected)
        {
            TypeInfo typeToCastTo = GetTypeInfo(sizeOf.Type);
            return expected.Match(e =>
            {
                if (e is BasicTypeInfo b && b.Name == RipplePrimitives.Int32Name)
                    return GetSizeOfPair(typeToCastTo, b);
                return GetSizeOfPair(typeToCastTo, RipplePrimitives.Int32);
            }, () => GetSizeOfPair(typeToCastTo, RipplePrimitives.Int32));
        }

        public Pair<ValueInfo, TypedExpression> VisitTypeExpression(TypeExpression typeExpression, Option<TypeInfo> expected)
        {
            throw new NotImplementedException();
        }

        private static Pair<ValueInfo, TypedExpression> GetVariablePair(VariableInfo variableInfo)
		{
            ValueInfo value = new ValueInfo(variableInfo.Type, variableInfo.Lifetime);
            TypedIdentifier identifier = new TypedIdentifier(variableInfo.Name, variableInfo, variableInfo.Type);
            return new Pair<ValueInfo, TypedExpression>(value, identifier);
		}

        private static Pair<ValueInfo, TypedExpression> GetFunctionPair(FunctionInfo functionInfo)
		{
            ValueInfo value = new ValueInfo(functionInfo.FunctionType, LifetimeInfo.Static);
            TypedIdentifier identifier = new TypedIdentifier(functionInfo.Name, functionInfo, functionInfo.ReturnType);
            return new Pair<ValueInfo, TypedExpression>(value, identifier);
		}

        private Pair<ValueInfo, TypedExpression> GetSizeOfPair(TypeInfo sizeOfType, TypeInfo returned)
        {
            ValueInfo value = new ValueInfo(sizeOfType, m_VariableStack.CurrentLifetime);
            TypedSizeOf typedSizeOf = new TypedSizeOf(sizeOfType, RipplePrimitives.Int32);
            return new Pair<ValueInfo, TypedExpression>(value, typedSizeOf);
        }

        private static void ThrowError(string message, Token token)
        {
            throw new ExpressionCheckerException(message, token);
        }

        private Pair<ValueInfo, TypedExpression> GetLiteralPair(TypeInfo typeInfo, TokenType literalType, string literalValue)
		{
            ValueInfo value = new ValueInfo(typeInfo, m_VariableStack.CurrentLifetime);
            TypedLiteral literal = new TypedLiteral(literalValue, literalType, typeInfo);
            return new Pair<ValueInfo, TypedExpression>(value, literal);
		}

        private TypeInfo GetTypeInfo(TypeName type)
        {
            return TypeInfoUtils.FromASTType(type, m_Primaries, m_ActiveLifetimes, m_SafetyContext)
                .Match(ok => ok, fail => throw new ExpressionCheckerException(fail));
        }
    }
}
