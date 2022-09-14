using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Transpiling
{
    enum CBinaryOperator
    {
        Plus,
        Minus,
        Times,
        Divide,
        Mod,

        Assign,

        EqualEqual,
        BangEqual,
        GreaterThan,
        GreaterThanEqual,
        LessThan,
        LessThanEqual,
    }

    static class CBinaryOperatorExtensions
    {
        public static string ToCCode(this CBinaryOperator op)
        {
            return op switch
            {
                CBinaryOperator.Plus => "+",
                CBinaryOperator.Minus => "-",
                CBinaryOperator.Times => "*",
                CBinaryOperator.Divide => "/",
                CBinaryOperator.Assign => "=",
                CBinaryOperator.EqualEqual => "==",
                CBinaryOperator.BangEqual => "!=",
                CBinaryOperator.GreaterThan => ">",
                CBinaryOperator.GreaterThanEqual => ">=",
                CBinaryOperator.LessThan => "<",
                CBinaryOperator.LessThanEqual => "<=",
                CBinaryOperator.Mod => "%",
                _ => throw new ArgumentException("Unknown operator"),
            };
        }
    }
}
