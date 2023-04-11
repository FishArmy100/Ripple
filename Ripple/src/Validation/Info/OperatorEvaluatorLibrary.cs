using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Utils;
using Ripple.Validation.Info.Types;
using Raucse.Extensions;
using Ripple.Validation.Errors.ExpressionErrors;

namespace Ripple.Validation.Info
{
    public class OperatorEvaluatorLibrary
    {
        public readonly OperatorEvaluator<TokenType, ValueInfo> Unaries =
            new OperatorEvaluator<TokenType, ValueInfo>(
                (t, v, el) => ExpressionError.Unary(el, t, v, false), 
                (t, v, el) => ExpressionError.Unary(el, t, v, true));

        public readonly OperatorEvaluator<TokenType, (ValueInfo, ValueInfo)> Binaries =
            new OperatorEvaluator<TokenType, (ValueInfo, ValueInfo)>(
                (t, v, el) => ExpressionError.Binary(el, v.Item1, t, v.Item2, false),
                (t, v, el) => ExpressionError.Binary(el, v.Item1, t, v.Item2, true));

        public readonly OperatorEvaluator<ValueInfo, IEnumerable<ValueInfo>> Calls =
            new OperatorEvaluator<ValueInfo, IEnumerable<ValueInfo>>(
                (o, p, el) => ExpressionError.Call(el, o, p.ToList(), false),
                (o, p, el) => ExpressionError.Call(el, o, p.ToList(), true));

        public readonly OperatorEvaluator<ValueInfo, ValueInfo> Indexers =
            new OperatorEvaluator<ValueInfo, ValueInfo>(
                (indexed, arg, el) => ExpressionError.Index(el, indexed, arg, false),
                (indexed, arg, el) => ExpressionError.Index(el, indexed, arg, true));

        public readonly OperatorEvaluator<TypeInfo, ValueInfo> Casts =
            new OperatorEvaluator<TypeInfo, ValueInfo>(
                (type, value, el) => ExpressionError.Cast(el, value, type, false),
                (type, value, el) => ExpressionError.Cast(el, value, type, true));
    }
}
