using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;

namespace Ripple.Validation.AstInfo
{
    abstract class OperatorData
    {
        public readonly TokenType OperatorType;
        public readonly string ReturnType;

        public abstract bool IsOperator(OperatorData other);
        public abstract bool IsOperator(TokenType type, List<string> args);

        protected OperatorData(TokenType operatorType, string returnType)
        {
            OperatorType = operatorType;
            ReturnType = returnType;
        }

        public class Binary : OperatorData
        {
            public readonly string LeftOperand;
            public readonly string RightOperand;

            public Binary(TokenType operatorType, string returnType, string left, string right) : 
                base(operatorType, returnType)
            {
                LeftOperand = left;
                RightOperand = right;
            }

            public override bool IsOperator(TokenType type, List<string> args)
            {
                if (args.Count != 2 || type != OperatorType)
                    return false;

                return args[0] == LeftOperand && args[1] == RightOperand;
            }

            public override bool IsOperator(OperatorData other)
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

        public class Unary : OperatorData
        {
            public readonly string OperandType;

            public Unary(TokenType operatorType, string returnType, string operandType) : 
                base(operatorType, returnType)
            {
                OperandType = operandType;
            }

            public override bool IsOperator(TokenType type, List<string> args)
            {
                if (args.Count != 1 || type != OperatorType)
                    return false;

                return args[0] == OperandType;
            }

            public override bool IsOperator(OperatorData other)
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
