using System;
using System.Collections.Generic;
using System.Linq;
using Ripple.Lexing;
using Ripple.Utils;
using Ripple.AST.Utils;
using Ripple.Utils.Extensions;

namespace Ripple.AST.Info
{
    class ValueOfExpressionVisitor : IExpressionVisitor<ValueInfo, Option<TypeInfo>>
    {
        private readonly LocalVariableStack m_VariableStack;
        private readonly FunctionList m_Functions;
        private readonly OperatorEvaluatorLibrary m_OperatorLibrary;
        private readonly Dictionary<string, VariableInfo> m_Globals;

        public ValueOfExpressionVisitor(ASTInfo astInfo, LocalVariableStack variableStack)
        {
            m_Functions = astInfo.Functions;
            m_Globals = astInfo.GlobalVariables;
            m_OperatorLibrary = astInfo.OperatorLibrary;
            m_VariableStack = variableStack;
        }

        public ValueOfExpressionVisitor(LocalVariableStack variableStack, FunctionList functions, OperatorEvaluatorLibrary operatorLibrary, Dictionary<string, VariableInfo> globals)
        {
            m_VariableStack = variableStack;
            m_Functions = functions;
            m_OperatorLibrary = operatorLibrary;
            m_Globals = globals;
        }

        public ValueInfo VisitBinary(Binary binary, Option<TypeInfo> expected)
        {
            LifetimeInfo currentLifetime = m_VariableStack.CurrentLifetime;
            ValueInfo left = binary.Left.Accept(this, expected);
            ValueInfo right = binary.Right.Accept(this, expected);

            TokenType operatorType = binary.Op.Type;

            return m_OperatorLibrary.Binaries.Evaluate(operatorType, (left, right), currentLifetime, binary.Op).Match(
                ok => ok,
                fail => throw new TypeOfExpressionExeption(fail.Message, fail.Token));
        }

        public ValueInfo VisitUnary(Unary unary, Option<TypeInfo> expected)
        {
            Option<TypeInfo> innerExpected = new Option<TypeInfo>();
            TokenType operatorType = unary.Op.Type;

            expected.Match(e =>
            {
                if (operatorType.IsType(TokenType.Minus, TokenType.Bang))
                    innerExpected = e;

                else if (operatorType == TokenType.Ampersand && e is TypeInfo.Reference r1)
                    innerExpected = r1.Contained.ChangeMutable(false);

                else if (operatorType == TokenType.RefMut && e is TypeInfo.Reference r2 && r2.Contained.Mutable)
                    innerExpected = r2.Contained;
            });

            ValueInfo operand = unary.Expr.Accept(this, innerExpected);

            return m_OperatorLibrary.Unaries.Evaluate(operatorType, operand, m_VariableStack.CurrentLifetime, unary.Op).Match(
                ok =>  ok,
                fail => throw new TypeOfExpressionExeption(fail.Message, fail.Token));
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

            if (callee.Type is TypeInfo.FunctionPointer fp)
            {
                if (call.Args.Count > fp.Parameters.Count)
                    throw new TypeOfExpressionExeption("Too many arguments for this function pointer type.", call.OpenParen);
                if (call.Args.Count < fp.Parameters.Count)
                    throw new TypeOfExpressionExeption("Too few arguments for this function pointer type.", call.OpenParen);

                for (int i = 0; i < call.Args.Count; i++)
                {
                    TypeInfo param = fp.Parameters[i];
                    ValueInfo arg = call.Args[i].Accept(this, param);
                    if (!arg.Type.EqualsWithoutFirstMutable(param))
                        throw new TypeOfExpressionExeption("Expected type: " + param + ", but found type: " + arg + ".", call.OpenParen);
                }

                return new ValueInfo(fp.Returned, m_VariableStack.CurrentLifetime);
            }

            List<ValueInfo> args = call.Args.ConvertAll(a => a.Accept(this, new Option<TypeInfo>()));
            return m_OperatorLibrary.Calls.Evaluate(callee, args, m_VariableStack.CurrentLifetime, call.OpenParen).Match(
                ok => ok,
                fail => throw new TypeOfExpressionExeption(fail.Message, fail.Token));
        }

        private ValueInfo CallFunction(Call call, Identifier id)
        {
            string name = id.Name.Text;
            int argCount = call.Args.Count;
            List<Option<TypeInfo>> argumentTypes = new List<Option<TypeInfo>>();
            foreach (Expression expression in call.Args)
            {
                try
                {
                    argumentTypes.Add(expression.Accept(this, new Option<TypeInfo>()).Type);
                }
                catch (AmbiguousTypeException)
                {
                    argumentTypes.Add(new Option<TypeInfo>());
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
                    Option<TypeInfo> argInfo = argumentTypes[i];
                    TypeInfo expectedParam = info.Parameters[i].Type;
                    if (argInfo.HasValue() && !argInfo.Value.EqualsWithoutFirstMutable(expectedParam))
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                    possibleCalled.Add(info);
            }

            if (possibleCalled.Count == 0)
                throw new TypeOfExpressionExeption("No function with given arguments found.", id.Name);

            if (possibleCalled.Count > 1)
                throw new TypeOfExpressionExeption("Cannot disambiguate between given function overloads.", id.Name);

            for (int i = 0; argCount > i; i++)
            {
                TypeInfo expectedParam = possibleCalled[0].Parameters[i].Type;
                call.Args[i].Accept(this, expectedParam); // double checks to make sure that everything can be infered, like initalizer lists
            }

            return new ValueInfo(possibleCalled[0].ReturnType, m_VariableStack.CurrentLifetime);
        }

        private TypeInfo CheckCallVariable(Call call, Identifier id, string name, VariableInfo varInfo)
        {
            if (varInfo.Type is TypeInfo.FunctionPointer fp)
            {
                if (call.Args.Count < fp.Parameters.Count)
                    throw new TypeOfExpressionExeption("Too few arguments for call expression.", id.Name);
                if (call.Args.Count > fp.Parameters.Count)
                    throw new TypeOfExpressionExeption("Too many arguments for call expression.", id.Name);

                for (int i = 0; i < fp.Parameters.Count; i++)
                {
                    TypeInfo paramInfo = fp.Parameters[i];
                    TypeInfo argInfo = call.Args[i].Accept(this, paramInfo).Type;
                    if (!paramInfo.ChangeMutable(false).Equals(argInfo.ChangeMutable(false)))
                        throw new TypeOfExpressionExeption("Expected type: " + paramInfo + ", but found type: " + argInfo + ".", id.Name);
                }

                return fp.Returned;
            }
            else
            {
                throw new TypeOfExpressionExeption("Identifier: " + name + ", cannot be called like a function.", id.Name);
            }
        }

        public ValueInfo VisitCast(Cast cast, Option<TypeInfo> expected)
        {
            //if (cast.Castee is Identifier id) // casting to find a function overload
            //{
            //    string name = id.Name.Text;
            //    List<FunctionInfo> funcOverloads = m_ASTInfo.Functions.GetOverloadsWithName(name);
            //    if (!m_VariableStack.ContainsVariable(name) && funcOverloads.Count > 0)
            //    {
            //        foreach (FunctionInfo overload in funcOverloads)
            //        {
            //            TypeInfo.FunctionPointer type = new TypeInfo.FunctionPointer(overload);
            //            if (type.Equals(TypeInfo.FromASTType(cast.TypeToCastTo)))
            //                return new ValueInfo(type, m_VariableStack.ScopeCount);
            //        }
            //    }
            //}

            //TypeInfo typeToCastTo = TypeInfo.FromASTType(cast.TypeToCastTo);
            //TypeInfo castee = cast.Castee.Accept(this, typeToCastTo).Type;

            //return m_OperatorLibrary.CastOperators.TryGet(typeToCastTo, castee, cast.AsToken).Match(
            //    ok => new ValueInfo(ok.TypeToCastTo, m_VariableStack.ScopeCount),
            //    fail => throw new TypeOfExpressionExeption(fail.Message, fail.Token));

            throw new NotImplementedException();
        }

        public ValueInfo VisitGrouping(Grouping grouping, Option<TypeInfo> expected)
        {
            return grouping.Expr.Accept(this, expected);
        }

        public ValueInfo VisitIdentifier(Identifier identifier, Option<TypeInfo> expected)
        {
            string name = identifier.Name.Text;
            if (m_VariableStack.TryGetVariable(name, out VariableInfo info))
                return new ValueInfo(info.Type, info.Lifetime);

            if (m_Globals.TryGetValue(name, out info))
                return new ValueInfo(info.Type, info.Lifetime);

            List<FunctionInfo> functions = m_Functions.GetOverloadsWithName(name);

            if (expected.HasValue() && expected.Value is TypeInfo.FunctionPointer)
            {
                foreach (FunctionInfo funcInfo in functions)
                {
                    TypeInfo funcType = new TypeInfo.FunctionPointer(funcInfo);
                    if (funcType.ChangeMutable(false).Equals(expected.Value.ChangeMutable(false)))
                        return new ValueInfo(expected.Value, LifetimeInfo.Static);
                }
            }

            if (functions.Count == 1)
            {
                FunctionInfo funcInfo = functions[0];
                return new ValueInfo(new TypeInfo.FunctionPointer(funcInfo), LifetimeInfo.Static);
            }
            else if (functions.Count > 1)
            {
                string message = "Too many function overloads for function: " + name + " to distinguish.";
                throw new TypeOfExpressionExeption(message, identifier.Name);
            }
            else
            {
                string varMessage = "Variable: " + name + " is not defined.";
                throw new TypeOfExpressionExeption(varMessage, identifier.Name);
            }
        }

        public ValueInfo VisitIndex(AST.Index index, Option<TypeInfo> expected)
        {
            ValueInfo indexed = index.Indexed.Accept(this, new Option<TypeInfo>());
            ValueInfo arg = index.Argument.Accept(this, RipplePrimitives.Int32);
            return m_OperatorLibrary.Indexers.Evaluate(indexed, arg, m_VariableStack.CurrentLifetime, index.OpenBracket).Match(
                ok => ok,
                fail => throw new TypeOfExpressionExeption(fail.Message, fail.Token));
        }

        public ValueInfo VisitInitializerList(InitializerList initializerList, Option<TypeInfo> expected)
        {
            if (expected.HasValue() && expected.Value is TypeInfo.Array array)
            {
                foreach (Expression expression in initializerList.Expressions)
                {
                    TypeInfo info = expression.Accept(this, array.Type).Type;
                    if (!info.Equals(array.Type))
                    {
                        string message = "Expected an array of: " +
                            array.Type +
                            ", but found an expression for: " +
                            info + ".";

                        throw new TypeOfExpressionExeption(message, initializerList.OpenBrace);
                    }
                }

                if (initializerList.Expressions.Count > array.Size)
                    throw new TypeOfExpressionExeption("Initializer list size is bigger than the array size.", initializerList.OpenBrace);

                return ValueInfoFromType(array);
            }

            throw new AmbiguousTypeException("Could not infer the type of the initializer list.", initializerList.OpenBrace);
        }

        public ValueInfo VisitLiteral(Literal literal, Option<TypeInfo> expected)
        {
            switch (literal.Val.Type)
            {
                case TokenType.IntagerLiteral:
                    return expected.Match(e =>
                    {
                        if (e is TypeInfo.Basic b)
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
                        if (e is TypeInfo.Basic b && b.Name == RipplePrimitives.Float32Name) // will return accurate mutable value
                            return ValueInfoFromType(b);

                        return ValueInfoFromType(RipplePrimitives.Float32);
                    },
                    () => ValueInfoFromType(RipplePrimitives.Float32));

                case TokenType.True:
                case TokenType.False:
                    return expected.Match(e =>
                    {
                        if (e is TypeInfo.Basic b && b.Name == RipplePrimitives.BoolName)
                            return ValueInfoFromType(b);
                        return ValueInfoFromType(RipplePrimitives.Bool);
                    },
                    () => ValueInfoFromType(RipplePrimitives.Bool));

                case TokenType.CharactorLiteral:
                    return expected.Match(e =>
                    {
                        if (e is TypeInfo.Basic b && b.Name == RipplePrimitives.CharName)
                            return ValueInfoFromType(b);
                        return ValueInfoFromType(RipplePrimitives.Char);
                    },
                    () => ValueInfoFromType(RipplePrimitives.Char));

                case TokenType.Nullptr:
                    if (expected.Match(e => e is TypeInfo.Pointer, () => false))
                        return ValueInfoFromType(expected.Value);
                    throw new AmbiguousTypeException("Could not infer nullptr, in this context", literal.Val);

                default:
                    throw new TypeOfExpressionExeption("Unknown literal.", literal.Val);
            }
        }

        public ValueInfo VisitSizeOf(SizeOf sizeOf, Option<TypeInfo> expected)
        {
            return expected.Match(e =>
            {
                if (e is TypeInfo.Basic b && b.Name == RipplePrimitives.Int32Name)
                    return ValueInfoFromType(b);
                return ValueInfoFromType(RipplePrimitives.Int32);
            }, () => ValueInfoFromType(RipplePrimitives.Int32));
        }

        public ValueInfo VisitTypeExpression(TypeExpression typeExpression, Option<TypeInfo> expected)
        {
            throw new NotImplementedException();
        }

        private ValueInfo ValueInfoFromType(TypeInfo type)
        {
            return new ValueInfo(type, m_VariableStack.CurrentLifetime);
        }
    }
}
