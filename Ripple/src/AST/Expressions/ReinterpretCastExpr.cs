using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    class ReinterpretCastExpr : Expression
    {
        public readonly Token Keyword;
        public readonly RippleType CastToType;
        public readonly Expression ConvertedExpr;
        public readonly Expression SizeExpr;

        public ReinterpretCastExpr(Token keyword, RippleType castToType, Expression convertedExpr, Expression sizeExpr)
        {
            Keyword = keyword;
            CastToType = castToType;
            ConvertedExpr = convertedExpr;
            SizeExpr = sizeExpr;
        }

        public ReinterpretCastExpr(Token keyword, RippleType castToType, Expression convertedExpr) : 
            this(keyword, castToType, convertedExpr, null)
        {

        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitReinterpretCast(this);
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitReinterpretCast(this);
        }
    }
}
