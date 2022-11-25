using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;

namespace Ripple.AST.Info
{
    abstract class OperatorInfo
    {
        public readonly TokenType OperatorType;
        public readonly TypeInfo ReturnType;

        public abstract bool IsOperator(OperatorInfo other);
        public abstract bool IsOperator(TokenType type, List<TypeInfo> args);

        protected OperatorInfo(TokenType operatorType, TypeInfo returnType)
        {
            OperatorType = operatorType;
            ReturnType = returnType;
        }

        public class Binary : OperatorInfo
        {
            public readonly TypeInfo LeftOperand;
            public readonly TypeInfo RightOperand;

            public Binary(TokenType operatorType, TypeInfo returnType, TypeInfo left, TypeInfo right) : 
                base(operatorType, returnType)
            {
                LeftOperand = left;
                RightOperand = right;
            }

            public override bool IsOperator(TokenType type, List<TypeInfo> args)
            {
                if (args.Count != 2 || type != OperatorType)
                    return false;

                return args[0] == LeftOperand && args[1] == RightOperand;
            }

            public override bool IsOperator(OperatorInfo other)
            {
                if (other.OperatorType != OperatorType)
                    return false;

                if(other is Binary b)
                {
                    return RightOperand == b.RightOperand && 
                           LeftOperand == b.LeftOperand;
                }

                return false;
            }
        }

        public class Unary : OperatorInfo
        {
            public readonly TypeInfo OperandType;

            public Unary(TokenType operatorType, TypeInfo returnType, TypeInfo operandType) : 
                base(operatorType, returnType)
            {
                OperandType = operandType;
            }

            public override bool IsOperator(TokenType type, List<TypeInfo> args)
            {
                if (args.Count != 1 || type != OperatorType)
                    return false;

                return args[0] == OperandType;
            }

            public override bool IsOperator(OperatorInfo other)
            {
                if (OperatorType != other.OperatorType)
                    return false;

                if(other is Unary u)
                {
                    return OperandType == u.OperandType;
                }

                return false;
            }
        }
    }
}
