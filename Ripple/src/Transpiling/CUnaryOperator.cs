using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Transpiling
{
    enum CUnaryOperator
    {
        Negate,
        Bang,
    }

    static class CUnaryOperatorExtensions
    {
        public static string ToCCode(this CUnaryOperator op)
        {
            return op switch
            {
                CUnaryOperator.Negate => "-",
                CUnaryOperator.Bang => "!",
                _ => throw new ArgumentException("Unknown operator"),
            };
        }
    }
}
