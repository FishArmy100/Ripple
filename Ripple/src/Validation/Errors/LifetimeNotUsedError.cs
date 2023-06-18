using Ripple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;

namespace Ripple.Validation.Errors
{
    class LifetimeNotUsedError : ValidationError
    {
        public readonly Token LifetimeToken;
        public LifetimeNotUsedError(SourceLocation location, Token lifetimeToken) : base(location)
        {
            LifetimeToken = lifetimeToken;
        }

        public override string GetMessage()
        {
            return $"Lifetime {LifetimeToken.Text} is not used.";
        }
    }
}
