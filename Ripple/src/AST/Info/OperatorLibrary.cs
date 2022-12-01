using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.AST.Utils;
using Ripple.Utils;
using Ripple.Utils.Extensions;

namespace Ripple.AST.Info
{
    class OperatorLibrary
    {
        public readonly OperatorList<OperatorInfo.Unary, TokenType, TypeInfo> UnaryOperators =
            new((u, type) => u.Operand.Equals(type), u => u.OperatorType, u => u.Operand, GetInrinsicUnaryOperator);

        public readonly OperatorList<OperatorInfo.Binary, TokenType, (TypeInfo, TypeInfo)> BinaryOperators =
            new((u, args) => u.Left.Equals(args.Item1) && u.Right.Equals(args.Item2), u => u.OperatorType, u => (u.Left, u.Right), GetIntrinsicBinaryOperator);

        public readonly OperatorList<OperatorInfo.Cast, TypeInfo, TypeInfo> CastOperators =
            new((u, type) => u.Operand.Equals(type), u => u.TypeToCastTo, u => u.Operand, GetIntrinsicCastOperator);

        public readonly OperatorList<OperatorInfo.Call, TypeInfo, List<TypeInfo>> CallOperators =
            new((u, args) => u.Arguments.SequenceEqual(args), u => u.Callee, u => u.Arguments, GetIntrisicCallOperator);

        public readonly OperatorList<OperatorInfo.Index, TypeInfo, TypeInfo> IndexOperators =
            new((u, arg) => u.Argument.Equals(arg), u => u.Indexed, u => u.Argument, GetIntrinsicIndexOperator);

        private static Option<OperatorInfo.Index> GetIntrinsicIndexOperator(TypeInfo callee, TypeInfo arg)
        {
            if(callee is TypeInfo.Array array && arg is TypeInfo.Basic barg && barg.Name == RipplePrimitives.Int32Name)
            {
                return new OperatorInfo.Index(callee, arg, array.Type);
            }
            else if (callee is TypeInfo.Pointer p && arg is TypeInfo.Basic parg && parg.Name == RipplePrimitives.Int32Name)
            {
                return new OperatorInfo.Index(callee, arg, p.Contained);
            }

            return new Option<OperatorInfo.Index>();
        }

        private static Option<OperatorInfo.Call> GetIntrisicCallOperator(TypeInfo callee, List<TypeInfo> args)
        {
            if(callee is TypeInfo.FunctionPointer fp)
            {
                if (fp.Parameters.SequenceEqual(args))
                    return new OperatorInfo.Call(callee, args, fp.Returned);
            }

            return new Option<OperatorInfo.Call>();
        }

        private static Option<OperatorInfo.Cast> GetIntrinsicCastOperator(TypeInfo typeToCastTo, TypeInfo arg)
        {
            if (typeToCastTo.Equals(arg))
                return new OperatorInfo.Cast(arg, typeToCastTo);
            else if (typeToCastTo.Equals(arg.ChangeMutable(false)))
                return new OperatorInfo.Cast(arg, typeToCastTo);
            else if (typeToCastTo is TypeInfo.Pointer && arg is TypeInfo.Reference)
            {
                if (!(typeToCastTo.Mutable && !arg.Mutable))
                    return new OperatorInfo.Cast(arg, typeToCastTo);
            }
            else if (typeToCastTo is TypeInfo.Reference && arg is TypeInfo.Pointer)
            {
                if (!(typeToCastTo.Mutable && !arg.Mutable))
                    return new OperatorInfo.Cast(arg, typeToCastTo);
            }

            return new Option<OperatorInfo.Cast>();
        }

        private static Option<OperatorInfo.Binary> GetIntrinsicBinaryOperator(TokenType operatorType, (TypeInfo, TypeInfo) operands)
        {
            TypeInfo left = operands.Item1;
            TypeInfo right = operands.Item2;

            if(operatorType == TokenType.Equal)
            {
                if (left.Mutable && left.Equals(right.ChangeMutable(true)))
                    return new OperatorInfo.Binary(left, right, operatorType, left);
            }
            else if(operatorType.IsType(TokenType.Plus, TokenType.Minus))
            {
                if(left is TypeInfo.Pointer p && right is TypeInfo.Basic b && b.Name == RipplePrimitives.Int32Name)
                {
                    return new OperatorInfo.Binary(left, right, operatorType, left);
                }
            }

            return new Option<OperatorInfo.Binary>();
        }

        private static Option<OperatorInfo.Unary> GetInrinsicUnaryOperator(TokenType operatorType, TypeInfo arg)
        {
            if(operatorType == TokenType.Ampersand)
            {
                return new OperatorInfo.Unary(arg, operatorType, new TypeInfo.Reference(false, arg.ChangeMutable(false)));
            }
            else if(operatorType == TokenType.RefMut && arg.Mutable)
            {
                return new OperatorInfo.Unary(arg, operatorType, new TypeInfo.Reference(false, arg));
            }
            else if(operatorType == TokenType.Star && arg is TypeInfo.Pointer p)
            {
                return new OperatorInfo.Unary(p, operatorType, p.Contained);
            }
            else if (operatorType == TokenType.Star && arg is TypeInfo.Reference r)
            {
                return new OperatorInfo.Unary(r, operatorType, r.Contained);
            }
            else
            {
                return new Option<OperatorInfo.Unary>();
            }
        }
    }
}
