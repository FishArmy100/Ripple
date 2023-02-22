using System;
using System.Collections.Generic;
using System.Linq;
using Ripple.Lexing;
using Ripple.Utils;
using Ripple.AST.Utils;
using Ripple.Utils.Extensions;
using Ripple.Validation.Info.Types;
using Ripple.AST;

namespace Ripple.Validation.Info
{
    class ValueOfExpressionVisitor : IExpressionVisitor<ValueInfo, Option<TypeInfo>>
    {
        private readonly LocalVariableStack m_VariableStack;
        private readonly FunctionList m_Functions;
        private readonly List<string> m_Primaries;
        private readonly OperatorEvaluatorLibrary m_OperatorLibrary;
        private readonly Dictionary<string, VariableInfo> m_Globals;
        private readonly SafetyContext m_SafetyContext;
        private readonly List<string> m_ActiveLifetimes;

        public ValueOfExpressionVisitor(ASTInfo astInfo, LocalVariableStack variableStack, List<string> activeLifetimes, SafetyContext safetyContext)
        {
            m_Functions = astInfo.Functions;
            m_Globals = astInfo.GlobalVariables;
            m_OperatorLibrary = astInfo.OperatorLibrary;
            m_VariableStack = variableStack;
            m_SafetyContext = safetyContext;
            m_Primaries = astInfo.PrimaryTypes;
            m_ActiveLifetimes = activeLifetimes;
        }

        public ValueOfExpressionVisitor(LocalVariableStack variableStack, FunctionList functions, OperatorEvaluatorLibrary operatorLibrary, Dictionary<string, VariableInfo> globals, SafetyContext safetyContext, List<string> primaries, List<string> activeLifetimes)
        {
            m_VariableStack = variableStack;
            m_Functions = functions;
            m_OperatorLibrary = operatorLibrary;
            m_Globals = globals;
            m_Primaries = primaries;
            m_SafetyContext = safetyContext;
            m_ActiveLifetimes = activeLifetimes;
        }

        public ValueInfo VisitBinary(Binary binary, Option<TypeInfo> expected)
        {
            LifetimeInfo currentLifetime = m_VariableStack.CurrentLifetime;
            ValueInfo left = binary.Left.Accept(this, expected);
            ValueInfo right = binary.Right.Accept(this, expected);

            TokenType operatorType = binary.Op.Type;

            return m_OperatorLibrary.Binaries.Evaluate(operatorType, (left, right), currentLifetime, binary.Op).Match(
                ok => ok,
                fail => throw new ValueOfExpressionExeption(fail.Message, fail.Token));
        }

        public ValueInfo VisitUnary(Unary unary, Option<TypeInfo> expected)
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

            ValueInfo operand = unary.Expr.Accept(this, innerExpected);

            return m_OperatorLibrary.Unaries.Evaluate(operatorType, operand, m_VariableStack.CurrentLifetime, unary.Op).Match(
                ok => 
                {
                    if(operatorType.IsType(TokenType.Ampersand, TokenType.RefMut))
                    {
                        return new ValueInfo(ok.Type.SetFirstMutable(expectedMutable), ok.Lifetime);
                    }

                    return ok;
                },
                fail =>
                {
                    throw new ValueOfExpressionExeption(fail.Message, fail.Token);
                });
        }

        public ValueInfo VisitCall(Call call, Option<TypeInfo> expected)
        {
            if (call.Callee is Identifier id &&
                !m_VariableStack.ContainsVariable(id.Name.Text) &&
                !m_Globals.ContainsKey(id.Name.Text) &&
                 m_Functions.ContainsFunctionWithName(id.Name.Text))
            {
                return CallFunction(call, id);
            }

            ValueInfo callee = call.Callee.Accept(this, new Option<TypeInfo>());
            List<ValueInfo> args = call.Args.ConvertAll(a => a.Accept(this, new Option<TypeInfo>()));
            return m_OperatorLibrary.Calls.Evaluate(callee, args, m_VariableStack.CurrentLifetime, call.OpenParen).Match(
                ok => ok,
                fail => throw new ValueOfExpressionExeption(fail.Message, fail.Token));
        }

        private ValueInfo CallFunction(Call call, Identifier id)
        {
            string name = id.Name.Text;
            int argCount = call.Args.Count;
            List<Option<ValueInfo>> arguments = new List<Option<ValueInfo>>();
            foreach (Expression expression in call.Args)
            {
                try
                {
                    arguments.Add(expression.Accept(this, new Option<TypeInfo>()));
                }
                catch (ValueOfExpressionExeption)
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
                throw new ValueOfExpressionExeption("No function with given arguments found.", id.Name);

            if (possibleCalled.Count > 1)
                throw new ValueOfExpressionExeption("Cannot disambiguate between given function overloads.", id.Name);

            for (int i = 0; argCount > i; i++)
            {
                TypeInfo expectedParam = possibleCalled[0].Parameters[i].Type;
                call.Args[i].Accept(this, expectedParam); // double checks to make sure that everything can be infered, like initalizer lists
            }

            FunctionInfo funcInfo = possibleCalled[0];

            if (funcInfo.IsUnsafe && m_SafetyContext.IsSafe)
                ThrowError("Use of unsafe function '" + funcInfo.Name + "' in a safe context.", call.OpenParen);

            List<ValueInfo> args = arguments.Select(at => at.Match(ok => ok, () => throw new ValueOfExpressionExeption("Invalid expression at call.", id.Name))).ToList();
            return m_OperatorLibrary.Calls.Evaluate(new ValueInfo(funcInfo.FunctionType, LifetimeInfo.Static), args, m_VariableStack.CurrentLifetime, id.Name)
                .Match(
                ok =>
                {
                    return ok;
                },
                fail => 
                {
                    throw new ValueOfExpressionExeption(fail);
                });
        }

        public ValueInfo VisitCast(Cast cast, Option<TypeInfo> expected)
        {
            TypeInfo typeToCastTo = TypeInfoUtils.FromASTType(cast.TypeToCastTo, m_Primaries, m_ActiveLifetimes, m_SafetyContext)
                .Match(ok => ok, fail => throw new ValueOfExpressionExeption(fail)); 

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
                            return new ValueInfo(type, LifetimeInfo.Static);
                    }
                }
            }

            ValueInfo castee = cast.Castee.Accept(this, typeToCastTo);

            return m_OperatorLibrary.Casts.Evaluate(typeToCastTo, castee, m_VariableStack.CurrentLifetime, cast.AsToken).Match(
                ok => ok,
                fail => throw new ValueOfExpressionExeption(fail));
        }

        public ValueInfo VisitGrouping(Grouping grouping, Option<TypeInfo> expected)
        {
            return grouping.Expr.Accept(this, expected);
        }

        public ValueInfo VisitIdentifier(Identifier identifier, Option<TypeInfo> expected)
        {
            string name = identifier.Name.Text;
            if (m_VariableStack.TryGetVariable(name, out VariableInfo info))
            {
                if(info.IsUnsafe && m_SafetyContext.IsSafe)
                    ThrowError("Use of unsafe local variable '" + info.Name + "' in a safe context.", identifier.Name);
                return new ValueInfo(info.Type, info.Lifetime);
            }

            if (m_Globals.TryGetValue(name, out info))
            {
                if (info.IsUnsafe && m_SafetyContext.IsSafe)
                    ThrowError("Use of unsafe global variable '" + info.Name + "' in a safe context.", identifier.Name);
                return new ValueInfo(info.Type, info.Lifetime);
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

                        return new ValueInfo(expected.Value, LifetimeInfo.Static);
                    }
                }
            }

            if (functions.Count == 1)
            {
                FunctionInfo funcInfo = functions[0];
                return new ValueInfo(funcInfo.FunctionType, LifetimeInfo.Static);
            }
            else if (functions.Count > 1)
            {
                string message = "Too many function overloads for function: " + name + " to distinguish.";
                throw new ValueOfExpressionExeption(message, identifier.Name);
            }
            else
            {
                string varMessage = "Variable: " + name + " is not defined.";
                throw new ValueOfExpressionExeption(varMessage, identifier.Name);
            }
        }

        public ValueInfo VisitIndex(AST.Index index, Option<TypeInfo> expected)
        {
            ValueInfo indexed = index.Indexed.Accept(this, new Option<TypeInfo>());
            ValueInfo arg = index.Argument.Accept(this, RipplePrimitives.Int32);
            return m_OperatorLibrary.Indexers.Evaluate(indexed, arg, m_VariableStack.CurrentLifetime, index.OpenBracket).Match(
                ok => ok,
                fail => throw new ValueOfExpressionExeption(fail.Message, fail.Token));
        }

        public ValueInfo VisitInitializerList(InitializerList initializerList, Option<TypeInfo> expected)
        {
            if (expected.HasValue() && expected.Value is ArrayInfo array)
            {
                foreach (Expression expression in initializerList.Expressions)
                {
                    TypeInfo info = expression.Accept(this, array.Contained).Type;
                    if (!info.IsEquatableTo(array.Contained))
                    {
                        string message = "Expected an array of: " +
                            array.Contained.ToPrettyString() +
                            ", but found an expression for: " +
                            info.ToPrettyString() + ".";

                        throw new ValueOfExpressionExeption(message, initializerList.OpenBrace);
                    }
                }

                if (initializerList.Expressions.Count > array.Size)
                    throw new ValueOfExpressionExeption("Initializer list size is bigger than the array size.", initializerList.OpenBrace);

                return ValueInfoFromType(array);
            }

            throw new ValueOfExpressionExeption("Could not infer the type of the initializer list.", initializerList.OpenBrace);
        }

        public ValueInfo VisitLiteral(Literal literal, Option<TypeInfo> expected)
        {
            switch (literal.Val.Type)
            {
                case TokenType.IntagerLiteral:
                    return expected.Match(e =>
                    {
                        if (e is BasicTypeInfo b)
                        {
                            if (b.Name == RipplePrimitives.Int32Name)
                                return ValueInfoFromType(b);
                            else if (b.Name == RipplePrimitives.Float32Name)
                                return ValueInfoFromType(b);
                        }

                        return ValueInfoFromType(RipplePrimitives.Int32);
                    },
                    () => ValueInfoFromType(RipplePrimitives.Int32));

                case TokenType.FloatLiteral:
                    return expected.Match(e =>
                    {
                        if (e is BasicTypeInfo b && b.Name == RipplePrimitives.Float32Name) // will return accurate mutable value
                            return ValueInfoFromType(b);

                        return ValueInfoFromType(RipplePrimitives.Float32);
                    },
                    () => ValueInfoFromType(RipplePrimitives.Float32));

                case TokenType.True:
                case TokenType.False:
                    return expected.Match(e =>
                    {
                        if (e is BasicTypeInfo b && b.Name == RipplePrimitives.BoolName)
                            return ValueInfoFromType(b);
                        return ValueInfoFromType(RipplePrimitives.Bool);
                    },
                    () => ValueInfoFromType(RipplePrimitives.Bool));

                case TokenType.CharactorLiteral:
                    return expected.Match(e =>
                    {
                        if (e is BasicTypeInfo b && b.Name == RipplePrimitives.CharName)
                            return ValueInfoFromType(b);
                        return ValueInfoFromType(RipplePrimitives.Char);
                    },
                    () => ValueInfoFromType(RipplePrimitives.Char));

                case TokenType.Nullptr:
                    if (expected.Match(e => e is PointerInfo, () => false))
                        return ValueInfoFromType(expected.Value);
                    throw new ValueOfExpressionExeption("Could not infer nullptr, in this context", literal.Val);

                case TokenType.CStringLiteral:
                    return new ValueInfo(new PointerInfo(false, RipplePrimitives.Char), LifetimeInfo.Static); // char* with a static lifetime

                default:
                    throw new ValueOfExpressionExeption("Unknown literal.", literal.Val);
            }
        }

        public ValueInfo VisitSizeOf(SizeOf sizeOf, Option<TypeInfo> expected)
        {
            return expected.Match(e =>
            {
                if (e is BasicTypeInfo b && b.Name == RipplePrimitives.Int32Name)
                    return ValueInfoFromType(b);
                return ValueInfoFromType(RipplePrimitives.Int32);
            }, () => ValueInfoFromType(RipplePrimitives.Int32));
        }

        public ValueInfo VisitTypeExpression(TypeExpression typeExpression, Option<TypeInfo> expected)
        {
            throw new NotImplementedException();
        }

        private void ThrowError(string message, Token token)
        {
            throw new ValueOfExpressionExeption(message, token);
        }

        private ValueInfo ValueInfoFromType(TypeInfo type)
        {
            return new ValueInfo(type, m_VariableStack.CurrentLifetime);
        }
    }
}
