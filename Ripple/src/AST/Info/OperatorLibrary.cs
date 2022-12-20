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
        public readonly UnaryOperatorEvaluator UnaryOperators = new UnaryOperatorEvaluator();

        public readonly OperatorList<OperatorInfo.Binary, TokenType, (TypeInfo, TypeInfo)> BinaryOperators =
            new((u, args) => u.Left.Equals(args.Item1) && u.Right.Equals(args.Item2),
                u => u.OperatorType, 
                u => (u.Left, u.Right),
                GetIntrinsicBinaryOperator,
                (op, args, token) => new ASTInfoError("No binary operator '" + op + "', for types '" + args.Item1 + "' and '" + args.Item2 + "',", token));

        public readonly OperatorList<OperatorInfo.Cast, TypeInfo, TypeInfo> CastOperators =
            new((u, type) => u.Operand.Equals(type),
                u => u.TypeToCastTo, 
                u => u.Operand,
                GetIntrinsicCastOperator,
                (type, casted, token) => new ASTInfoError("No cast operator for type '" + casted + "', to type '" + type + "'.", token));

        public readonly OperatorList<OperatorInfo.Call, TypeInfo, List<TypeInfo>> CallOperators =
            new((u, args) => u.Arguments.SequenceEqual(args), 
                u => u.Callee, 
                u => u.Arguments, 
                GetIntrisicCallOperator,
                (callee, args, token) => new ASTInfoError("No call operator for type '" + callee + "', with arguments " + 
                    args.ConvertAll(a => "'" + a + "'").Concat(", "), token));

        public readonly OperatorList<OperatorInfo.Index, TypeInfo, TypeInfo> IndexOperators =
            new((u, arg) => u.Argument.Equals(arg), 
                u => u.Indexed, 
                u => u.Argument, 
                GetIntrinsicIndexOperator,
                (indexed, arg, token) => new ASTInfoError("No index operator for type '" + indexed + "', with argument '" + arg + "'.", token));

        private static Option<Result<OperatorInfo.Index, ASTInfoError>> GetIntrinsicIndexOperator(TypeInfo indexee, TypeInfo arg)
        {
            if(indexee is TypeInfo.Array array && arg is TypeInfo.Basic barg && barg.Name == RipplePrimitives.Int32Name)
            {
                return OptionResult(new OperatorInfo.Index(indexee, arg, array.Type));
            }
            else if (indexee is TypeInfo.Pointer p && arg is TypeInfo.Basic parg && parg.Name == RipplePrimitives.Int32Name)
            {
                return OptionResult(new OperatorInfo.Index(indexee, arg, p.Contained));
            }

            return NoneOptionResult<OperatorInfo.Index>(); 
        }

        private static Option<Result<OperatorInfo.Call, ASTInfoError>> GetIntrisicCallOperator(TypeInfo callee, List<TypeInfo> args)
        {
            if(callee is TypeInfo.FunctionPointer fp)
            {
                if (fp.Parameters.SequenceEqual(args))
                    return OptionResult(new OperatorInfo.Call(callee, args, fp.Returned));
            }

            return NoneOptionResult<OperatorInfo.Call>();
        }

        private static Option<Result<OperatorInfo.Cast, ASTInfoError>> GetIntrinsicCastOperator(TypeInfo typeToCastTo, TypeInfo arg)
        {
            if (typeToCastTo.Equals(arg))
                return OptionResult(new OperatorInfo.Cast(arg, typeToCastTo));
            else if (typeToCastTo.ChangeMutable(false).Equals(arg.ChangeMutable(false)))
                return OptionResult(new OperatorInfo.Cast(arg, typeToCastTo));
            else if (typeToCastTo is TypeInfo.Pointer p1 && arg is TypeInfo.Reference r1)
            {
                if (!(p1.Contained.Mutable && !r1.Contained.Mutable) && 
                    r1.Contained.ChangeMutable(false).Equals(p1.Contained.ChangeMutable(false)))
                {
                    return OptionResult(new OperatorInfo.Cast(arg, typeToCastTo));
                }
            }
            else if (typeToCastTo is TypeInfo.Reference r2 && arg is TypeInfo.Pointer p2)
            {
                if (!(r2.Contained.Mutable && !p2.Contained.Mutable) &&
                    p2.Contained.ChangeMutable(false).Equals(r2.Contained.ChangeMutable(false)))
                {
                    return OptionResult(new OperatorInfo.Cast(arg, typeToCastTo));
                }
            }
            else if(typeToCastTo is TypeInfo.Pointer pointer && arg is TypeInfo.Pointer castedPointer)
            {
                if (!(pointer.Contained.Mutable && !castedPointer.Contained.Mutable))
                    return OptionResult(new OperatorInfo.Cast(arg, typeToCastTo));
            }

            return NoneOptionResult<OperatorInfo.Cast>();
        }

        private static Option<Result<OperatorInfo.Binary, ASTInfoError>> GetIntrinsicBinaryOperator(TokenType operatorType, (TypeInfo, TypeInfo) operands)
        {
            TypeInfo left = operands.Item1;
            TypeInfo right = operands.Item2;

            if(operatorType == TokenType.Equal && left.Mutable)
            {
                if (left.Equals(right.ChangeMutable(true)))
                {
                    return OptionResult(new OperatorInfo.Binary(left, right, operatorType, left));
                }
                else if(left is TypeInfo.Reference rl && right is TypeInfo.Reference rr)
                {
                    if(rl.Contained.Equals(rr.Contained) && rr.Lifetime.IsAssignableTo(rl.Lifetime))
                    {
                        return OptionResult(new OperatorInfo.Binary(left, right, operatorType, left));
                    }
                    else
                    {
                        return ErrorResult<OperatorInfo.Binary>(new ASTInfoError("Right operand does not live long enouph.", new Token()));
                    }
                }
            }
            else if(operatorType.IsType(TokenType.Plus, TokenType.Minus))
            {
                if(left is TypeInfo.Pointer && right is TypeInfo.Basic b && b.Name == RipplePrimitives.Int32Name)
                {
                    return OptionResult(new OperatorInfo.Binary(left, right, operatorType, left));
                }
            }

            return NoneOptionResult<OperatorInfo.Binary>();
        }

        private static Option<Result<T, ASTInfoError>> OptionResult<T>(T value)
        {
            return new Option<Result<T, ASTInfoError>>(value);
        }

        private static Option<Result<T, ASTInfoError>> ErrorResult<T>(ASTInfoError error)
        {
            return new Option<Result<T, ASTInfoError>>(error);
        }

        private static Option<Result<T, ASTInfoError>> NoneOptionResult<T>()
        {
            return new Option<Result<T, ASTInfoError>>();
        }
    }
}
