﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Utils;
using Ripple.Validation.Info.Types;
using Raucse.Extensions;

namespace Ripple.Validation.Info
{
    public class OperatorEvaluatorLibrary
    {
        public readonly OperatorEvaluator<TokenType, ValueInfo> Unaries =
            new OperatorEvaluator<TokenType, ValueInfo>(
                (t, v, et) => new ValidationError("Too many '" + t + "' operators for value '" + v + "'.", et), 
                (t, v, et) => new ValidationError("No '" + t + "' operators for value '" + v + "'.", et));

        public readonly OperatorEvaluator<TokenType, (ValueInfo, ValueInfo)> Binaries =
            new OperatorEvaluator<TokenType, (ValueInfo, ValueInfo)>(
                (t, v, et) => new ValidationError("Too many '" + t + "' operators for values '" + v.Item1 + "' and '" + v.Item2 + "'.", et),
                (t, v, et) => new ValidationError("No '" + t + "' operators for values '" + v.Item1 + "' and '" + v.Item2 + "'.", et));

        public readonly OperatorEvaluator<ValueInfo, IEnumerable<ValueInfo>> Calls =
            new OperatorEvaluator<ValueInfo, IEnumerable<ValueInfo>>(
                (o, p, t) => new ValidationError("Too many call operators for type '" + o.Type.ToPrettyString() + "' with arguments: '" + p.Select(v => v.ToString()).Concat("', '") + "'.", t),
                (o, p, t) => new ValidationError("No call operators for type '" + o.Type.ToPrettyString() + "' with arguments: '" + p.Select(v => v.ToString()).Concat("', '") + "'.", t));

        public readonly OperatorEvaluator<ValueInfo, ValueInfo> Indexers =
            new OperatorEvaluator<ValueInfo, ValueInfo>(
                (indexed, arg, tok) => new ValidationError("Multiple index operators possible for type '" + indexed.Type.ToPrettyString() + "' with argument '" + arg + "'.", tok),
                (indexed, arg, tok) => new ValidationError("No index operator for type '" + indexed.Type.ToPrettyString() + "' with argument '" + arg + "'.", tok));

        public readonly OperatorEvaluator<TypeInfo, ValueInfo> Casts =
            new OperatorEvaluator<TypeInfo, ValueInfo>(
                (type, value, tok) => new ValidationError("Multiple possible cast operators from type '" + value.Type.ToPrettyString() + "', to type '" + type.ToPrettyString() + "'.", tok),
                (type, value, tok) => new ValidationError("No cast operator from type '" + value.Type.ToPrettyString() + "', to type '" + type.ToPrettyString() + "'.", tok));
    }
}
