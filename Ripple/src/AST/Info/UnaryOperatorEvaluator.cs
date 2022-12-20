using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Utils.Extensions;
using Ripple.Utils;

namespace Ripple.AST.Info
{
    class UnaryOperatorEvaluator
    {
        private readonly Dictionary<TokenType, List<OperatorInfo.Unary>> m_AddedUnaries = new Dictionary<TokenType, List<OperatorInfo.Unary>>();

        public bool TryAdd(OperatorInfo.Unary unary)
        {
            List<OperatorInfo.Unary> ops = m_AddedUnaries.GetOrCreate(unary.OperatorType);
            if(!ops.Any(u => u.Operand.Equals(unary.Operand)))
            {
                ops.Add(unary);
                return true;
            }

            return false;
        }

        public Result<ValueInfo, ASTInfoError> EvaluateOperator(TokenType operatorType, ValueInfo arg, Token errorToken)
        {
            var op = TryEvaluateIntrinsic(operatorType, arg);
            return op.Match(result =>
            {
                return result;
            },
            () =>
            {
                List<OperatorInfo.Unary> overloads = m_AddedUnaries.GetOrCreate(operatorType);
                var operatorData = overloads.FirstOrDefault(o => o.Operand.Equals(arg.Type));
                if (operatorData != null)
                    return new ValueInfo(operatorData.Returned, arg.Liftime);

                string message = "No unary operator '" + operatorType + "', exists for operand of type '" + arg.Type + "'.";
                return new Result<ValueInfo, ASTInfoError>(new ASTInfoError(message, errorToken));
            });
        }

        private static Option<Result<ValueInfo, ASTInfoError>> TryEvaluateIntrinsic(TokenType operatorType, ValueInfo arg)
        {
            TypeInfo argType = arg.Type;

            if (operatorType == TokenType.Ampersand)
            {
                TypeInfo.Reference returned = new TypeInfo.Reference(false, argType.ChangeMutable(false), arg.Liftime);
                ValueInfo val = new ValueInfo(returned, arg.Liftime);
                return OptionResult(val);
            }
            else if (operatorType == TokenType.RefMut && argType.Mutable)
            {
                TypeInfo.Reference returned = new TypeInfo.Reference(true, argType, arg.Liftime);
                ValueInfo val = new ValueInfo(returned, arg.Liftime);
                return OptionResult(val);
            }
            else if (operatorType == TokenType.Star && argType is TypeInfo.Pointer p)
            {
                ValueInfo val = new ValueInfo(p.Contained, arg.Liftime);
                return OptionResult(val);
            }
            else if (operatorType == TokenType.Star && argType is TypeInfo.Reference r)
            {
                ValueInfo val = new ValueInfo(r.Contained, r.Lifetime);
                return OptionResult(val);
            }
            else
            {
                return NoneOptionResult<ValueInfo>();
            }
        }

        private static Option<Result<T, ASTInfoError>> OptionResult<T>(T value)
        {
            return new Option<Result<T, ASTInfoError>>(value);
        }

        private static Option<Result<T, ASTInfoError>> NoneOptionResult<T>()
        {
            return new Option<Result<T, ASTInfoError>>();
        }
    }
}
