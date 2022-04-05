using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    class NewArrayExpr : Expression
    {
        public readonly Token Keyword;
        public readonly RippleType ArrayType;
        public readonly Expression SizeExpr;
        public readonly Expression DefultValueExpr;
        public readonly List<Expression> InitializerArrayArgs;

        public NewArrayExpr(Token keyword, RippleType arrayType, Expression sizeExpr, Expression defultValueExpr, List<Expression> initializerArrayArgs)
        {
            Keyword = keyword;
            ArrayType = arrayType;
            SizeExpr = sizeExpr;
            DefultValueExpr = defultValueExpr;
            InitializerArrayArgs = initializerArrayArgs;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitNewArray(this);
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitNewArray(this);
        }
    }
}
