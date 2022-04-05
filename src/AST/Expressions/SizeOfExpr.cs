using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    class SizeOfExpr : Expression
    {
        public enum ArgType { Expr, Type }
        public Token Keyword;
        public readonly ArgType ArgumentType;
        public Expression Expr;
        public RippleType Type;

        public SizeOfExpr(Token keyword, Expression expr) 
        {
            Keyword = keyword;
            Expr = expr;
            Type = null;
            ArgumentType = ArgType.Expr;
        }

        public SizeOfExpr(Token keyword, RippleType type)
        {
            Keyword = keyword;
            Expr = null;
            Type = type;
            ArgumentType = ArgType.Type;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitSizeOf(this);
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitSizeOf(this);
        }
    }
}
