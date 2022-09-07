using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;

namespace Ripple.Validation
{
    abstract class OperatorData
    {
        public readonly TokenType OperatorType;
        public readonly string ReturnType;

        public abstract bool IsSameOperator(OperatorData other);

        protected OperatorData(TokenType operatorType, string returnType)
        {
            OperatorType = operatorType;
            ReturnType = returnType;
        }

        public class Binary : OperatorData
        {
            public readonly string Left;
            public readonly string Right;

            public Binary(TokenType operatorType, string returnType, string left, string right) : 
                base(operatorType, returnType)
            {
                Left = left;
                Right = right;
            }

            public override bool IsSameOperator(OperatorData other)
            {
                if(other is Binary binary)
                {
                    return this.OperatorType == binary.OperatorType &&
                           this.ReturnType == binary.ReturnType &&
                           this.Left == binary.Left &&
                           this.Right == binary.Right;
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

            public override bool IsSameOperator(OperatorData other)
            {
                if (other is Unary unary)
                {
                    return this.OperatorType == unary.OperatorType &&
                           this.ReturnType == unary.ReturnType &&
                           this.OperandType == unary.OperandType;
                }

                return false;
            }
        }
    }
}
