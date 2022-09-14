using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils;

namespace Ripple.Transpiling
{
    abstract class CExpression
    {
        public abstract string ConvertToString();

        public class Binary : CExpression
        {
            public readonly CExpression Left;
            public readonly CBinaryOperator Op;
            public readonly CExpression Right;

            public Binary(CExpression left, CBinaryOperator op, CExpression right)
            {
                Left = left;
                Op = op;
                Right = right;
            }

            public override string ConvertToString()
            {
                return Left.ConvertToString() + " " + Op.ToCCode() + " " + Right.ConvertToString();
            }
        }

        public class Unary : CExpression
        {
            public readonly CExpression Operand;
            public readonly CUnaryOperator Op;

            public Unary(CExpression operand, CUnaryOperator op)
            {
                Operand = operand;
                Op = op;
            }

            public override string ConvertToString()
            {
                return Op.ToCCode() + " " + Operand.ConvertToString();
            }
        }

        public class Call : CExpression
        {
            public readonly List<CExpression> Arguments;
            public readonly string FuncionName;

            public Call(List<CExpression> arguments, string funcionName)
            {
                Arguments = arguments;
                FuncionName = funcionName;
            }

            public override string ConvertToString()
            {
                string str = FuncionName;
                str += "(";
                for(int i = 0; i < Arguments.Count; i++)
                {
                    if (i != 0)
                        str += ", ";
                    str += Arguments[i].ConvertToString();
                }

                return str;
            }
        }

        public class Grouping : CExpression
        {
            public readonly CExpression GroupedExpression;

            public Grouping(CExpression groupedExpression)
            {
                GroupedExpression = groupedExpression;
            }

            public override string ConvertToString()
            {
                return "(" + GroupedExpression.ConvertToString() + ")";
            }
        }

        public class Value : CExpression
        {
            public readonly string ValueString;

            private Value(string valueString)
            {
                ValueString = valueString;
            }

            public static Value FromFloat(float value)
            {
                return new Value(value.ToString() + "f");
            }

            public static Value FromInt(int value)
            {
                return new Value(value.ToString());
            }

            public static Value FromBool(bool value)
            {
                if (value)
                    return new Value(CKeywords.True);
                else
                    return new Value(CKeywords.False);
            }

            public static Value FromIdentifier(string id)
            {
                return new Value(id);
            }

            public override string ConvertToString()
            {
                return ValueString;
            }
        }
    }
}
