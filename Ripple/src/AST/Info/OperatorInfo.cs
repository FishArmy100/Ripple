using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;

namespace Ripple.AST.Info
{
    static class OperatorInfo
    {
        public class Unary
        {
            public readonly TypeInfo Operand;
            public readonly TokenType OperatorType;
            public readonly TypeInfo Returned;

            public Unary(TypeInfo operand, TokenType operatorType, TypeInfo returned)
            {
                Operand = operand;
                OperatorType = operatorType;
                Returned = returned;
            }

            public override bool Equals(object obj)
            {
                return obj is Unary unary &&
                       EqualityComparer<TypeInfo>.Default.Equals(Operand, unary.Operand) &&
                       OperatorType == unary.OperatorType &&
                       EqualityComparer<TypeInfo>.Default.Equals(Returned, unary.Returned);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Operand, OperatorType, Returned);
            }
        }

        public class Binary
        {
            public readonly TypeInfo Left;
            public readonly TypeInfo Right;
            public readonly TokenType OperatorType;
            public readonly TypeInfo Returned;

            public Binary(TypeInfo left, TypeInfo right, TokenType operatorType, TypeInfo returned)
            {
                Left = left;
                Right = right;
                OperatorType = operatorType;
                Returned = returned;
            }

            public override bool Equals(object obj)
            {
                return obj is Binary binary &&
                       EqualityComparer<TypeInfo>.Default.Equals(Left, binary.Left) &&
                       EqualityComparer<TypeInfo>.Default.Equals(Right, binary.Right) &&
                       OperatorType == binary.OperatorType &&
                       EqualityComparer<TypeInfo>.Default.Equals(Returned, binary.Returned);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Left, Right, OperatorType, Returned);
            }
        }

        public class Cast
        {
            public readonly TypeInfo Operand;
            public readonly TypeInfo TypeToCastTo;

            public Cast(TypeInfo operand, TypeInfo typeToCastTo)
            {
                Operand = operand;
                TypeToCastTo = typeToCastTo;
            }

            public override bool Equals(object obj)
            {
                return obj is Cast cast &&
                       EqualityComparer<TypeInfo>.Default.Equals(Operand, cast.Operand) &&
                       EqualityComparer<TypeInfo>.Default.Equals(TypeToCastTo, cast.TypeToCastTo);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Operand, TypeToCastTo);
            }
        }

        public class Call
        {
            public readonly TypeInfo Callee;
            public readonly List<TypeInfo> Arguments;
            public readonly TypeInfo Returned;

            public Call(TypeInfo callee, List<TypeInfo> arguments, TypeInfo returned)
            {
                Callee = callee;
                Arguments = arguments;
                Returned = returned;
            }

            public override bool Equals(object obj)
            {
                return obj is Call call &&
                       EqualityComparer<TypeInfo>.Default.Equals(Callee, call.Callee) &&
                       EqualityComparer<List<TypeInfo>>.Default.Equals(Arguments, call.Arguments) &&
                       EqualityComparer<TypeInfo>.Default.Equals(Returned, call.Returned);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Callee, Arguments, Returned);
            }
        }

        public class Index
        {
            public readonly TypeInfo Indexed;
            public readonly TypeInfo Argument;
            public readonly TypeInfo Returned;

            public Index(TypeInfo indexed, TypeInfo argument, TypeInfo returned)
            {
                Indexed = indexed;
                Argument = argument;
                Returned = returned;
            }

            public override bool Equals(object obj)
            {
                return obj is Index index &&
                       EqualityComparer<TypeInfo>.Default.Equals(Indexed, index.Indexed) &&
                       EqualityComparer<TypeInfo>.Default.Equals(Argument, index.Argument) &&
                       EqualityComparer<TypeInfo>.Default.Equals(Returned, index.Returned);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Indexed, Argument, Returned);
            }
        }
    }
}
