using Ripple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Raucse;
using Ripple.Lexing;
using Raucse.Extensions;

namespace Ripple.Validation.Errors.ExpressionErrors
{
    class ExpressionError : ValidationError
    {
        public readonly ExpressionType Type;
        public readonly List<ValueInfo> Arguments;
        public readonly Option<ValueInfo> OperandObject;
        public readonly Option<TokenType> OperatorType;
        public readonly Option<TypeInfo> CastedType;
        public readonly bool NoOperators;

        private ExpressionError(SourceLocation location, ExpressionType type, List<ValueInfo> arguments, Option<ValueInfo> operandObject, Option<TokenType> operatorType, Option<TypeInfo> castedType, bool noOperators) : base(location)
        {
            Type = type;
            Arguments = arguments;
            OperandObject = operandObject;
            OperatorType = operatorType;
            CastedType = castedType;
            NoOperators = noOperators;
        }

        public static ExpressionError Unary(SourceLocation location, TokenType op, ValueInfo operand, bool noOperators)
        {
            return new ExpressionError(location, ExpressionType.Unary, new List<ValueInfo>(), operand, op, new Option<TypeInfo>(), noOperators);
        }

        public static ExpressionError Binary(SourceLocation location, ValueInfo left, TokenType op, ValueInfo right, bool noOperators)
        {
            return new ExpressionError(location, ExpressionType.Binary, new List<ValueInfo> { left, right }, new Option<ValueInfo>(), op, new Option<TypeInfo>(), noOperators);
        }

        public static ExpressionError Call(SourceLocation location, ValueInfo callee, List<ValueInfo> args, bool noOperators)
        {
            return new ExpressionError(location, ExpressionType.Call, args, callee, new Option<TokenType>(), new Option<TypeInfo>(), noOperators);
        }

        public static ExpressionError Index(SourceLocation location, ValueInfo indexee, ValueInfo arg, bool noOperators)
        {
            return new ExpressionError(location, ExpressionType.Index, new List<ValueInfo> { arg }, indexee, new Option<TokenType>(), new Option<TypeInfo>(), noOperators);
        }

        public static ExpressionError Cast(SourceLocation location, ValueInfo castee, TypeInfo typeToCastTo, bool noOperators)
        {
            return new ExpressionError(location, ExpressionType.Cast, new List<ValueInfo>(), castee, new Option<TokenType>(), typeToCastTo, noOperators);
        }

        public override string GetMessage()
        {
            switch (Type)
            {
                case ExpressionType.Unary:
                    if(NoOperators)
                    {
                        return $"No unary {OperatorType.Value.ToPrettyString()} operator for value {OperandObject}.";
                    }
                    else
                    {
                        return $"Too many unary {OperatorType.Value.ToPrettyString()} operators for value {OperandObject}.";
                    }
                case ExpressionType.Binary:
                    if(NoOperators)
                    {
                        return $"No binary {OperatorType.Value.ToPrettyString()} operators for values {Arguments[0]} and {Arguments[1]}";
                    }
                    else
                    {
                        return $"Too many binary {OperatorType.Value.ToPrettyString()} operators for values {Arguments[0]} and {Arguments[1]}";
                    }
                case ExpressionType.Index:
                    if (NoOperators)
                    {
                        return $"No index operator for value {OperandObject.Value} with argument {Arguments[0]}.";
                    }
                    else
                    {
                        return $"Too many index operators for value {OperandObject.Value} with argument {Arguments[0]}.";
                    }
                case ExpressionType.Call:
                    if (NoOperators)
                    {
                        return $"No call operator for value {OperandObject.Value} with arguments {Arguments.Select(a => a.ToString()).Concat(", ")}.";
                    }
                    else
                    {
                        return $"Too many call operators for value {OperandObject.Value} with arguments {Arguments.Select(a => a.ToString()).Concat(", ")}.";
                    }
                case ExpressionType.Cast:
                    if (NoOperators)
                    {
                        return $"No cast operator for value {OperandObject.Value} to cast to type {CastedType.Value}.";
                    }
                    else
                    {
                        return $"To many cast operators for value {OperandObject.Value} to cast to type {CastedType.Value}.";
                    }
                default:
                    throw new ArgumentException("Something has gone very wrong.");
            }
        }

        public enum ExpressionType
        {
            Unary,
            Binary,
            Index,
            Call,
            Cast
        }
    }
}
