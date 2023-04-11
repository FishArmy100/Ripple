using Ripple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Validation.Errors.ExpressionErrors
{
    class CouldNotInferExpressionExeption : ValidationError
    {
        public enum ExprType
        {
            InitalizerList,
            Nullptr,
        }

        public ExprType Type;

        public static CouldNotInferExpressionExeption Initalizer(SourceLocation location) => new CouldNotInferExpressionExeption(location, ExprType.InitalizerList);
        public static CouldNotInferExpressionExeption Nullptr(SourceLocation location) => new CouldNotInferExpressionExeption(location, ExprType.Nullptr);

        private CouldNotInferExpressionExeption(SourceLocation location, ExprType type) : base(location)
        {
            Type = type;
        }

        public override string GetMessage()
        {
            return Type switch
            {
                ExprType.InitalizerList => "Could not infer the type of the initalizer list.",
                ExprType.Nullptr => "Could not infer the type of the nullptr.",
                _ => throw new NotImplementedException(),
            };
        }
    }
}
