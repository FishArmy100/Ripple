using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Core;
using Ripple.Validation.Info;
using Ripple.Lexing;

namespace Ripple.Validation.Errors
{
    class CannotDicernBetweenLifetimesError : ValidationError
    {
        public readonly Token LifetimeA;
        public readonly Token LifetimeB;

        public CannotDicernBetweenLifetimesError(SourceLocation location, Token lifetimeA, Token lifetimeB) : base(location)
        {
            LifetimeA = lifetimeA;
            LifetimeB = lifetimeB;
        }

        public override string GetMessage() => $"Cannot dicern between lifetime {LifetimeA.Text} and {LifetimeB.Text}.";
    }
}
