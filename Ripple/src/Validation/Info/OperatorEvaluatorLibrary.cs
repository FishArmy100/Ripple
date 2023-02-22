using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Utils;
using Ripple.Utils.Extensions;
using Ripple.Validation.Info.Types;

namespace Ripple.Validation.Info
{
    class OperatorEvaluatorLibrary
    {
        public readonly OperatorEvaluator<TokenType, ValueInfo> Unaries =
            new OperatorEvaluator<TokenType, ValueInfo>(
                (t, v, et) => new ASTInfoError("Too many '" + t + "' operators for value '" + v + "'.", et), 
                (t, v, et) => new ASTInfoError("No '" + t + "' operators for value '" + v + "'.", et));

        public readonly OperatorEvaluator<TokenType, (ValueInfo, ValueInfo)> Binaries =
            new OperatorEvaluator<TokenType, (ValueInfo, ValueInfo)>(
                (t, v, et) => new ASTInfoError("Too many '" + t + "' operators for values '" + v.Item1 + "' and '" + v.Item2 + "'.", et),
                (t, v, et) => new ASTInfoError("No '" + t + "' operators for values '" + v.Item1 + "' and '" + v.Item2 + "'.", et));

        public readonly OperatorEvaluator<ValueInfo, List<ValueInfo>> Calls =
            new OperatorEvaluator<ValueInfo, List<ValueInfo>>(
                (o, p, t) => new ASTInfoError("Too many call operators for type '" + o.Type.ToPrettyString() + "' with arguments: '" + p.ConvertAll(v => v.ToString()).Concat("', '") + "'.", t),
                (o, p, t) => new ASTInfoError("No call operators for type '" + o.Type.ToPrettyString() + "' with arguments: '" + p.ConvertAll(v => v.ToString()).Concat("', '") + "'.", t));

        public readonly OperatorEvaluator<ValueInfo, ValueInfo> Indexers =
            new OperatorEvaluator<ValueInfo, ValueInfo>(
                (indexed, arg, tok) => new ASTInfoError("Multiple index operators possible for type '" + indexed.Type.ToPrettyString() + "' with argument '" + arg + "'.", tok),
                (indexed, arg, tok) => new ASTInfoError("No index operator for type '" + indexed.Type.ToPrettyString() + "' with argument '" + arg + "'.", tok));

        public readonly OperatorEvaluator<TypeInfo, ValueInfo> Casts =
            new OperatorEvaluator<TypeInfo, ValueInfo>(
                (type, value, tok) => new ASTInfoError("Multiple possible cast operators from type '" + value.Type.ToPrettyString() + "', to type '" + type.ToPrettyString() + "'.", tok),
                (type, value, tok) => new ASTInfoError("No cast operator from type '" + value.Type.ToPrettyString() + "', to type '" + type.ToPrettyString() + "'.", tok));
    }
}
