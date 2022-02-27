using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    static class Transpiler
    {
        public static string TranspileExpression(Expression expression)
        {
            TranspilerExpressionVisitor v = new TranspilerExpressionVisitor();
            return expression.Accept(v);
        }
    }
}
