using System;
using System.Collections.Generic;
using System.Text;

namespace Ripple.AST
{
    abstract class Expression
    {
        public abstract T Accept<T>(IExpressionVisitor<T> visitor);
        public abstract void Accept(IExpressionVisitor visitor);
    }
}
