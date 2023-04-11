﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Compiling;
using Ripple.Core;

namespace Ripple.Parsing.Errors
{
    public abstract class ParserError : CompilerError
    {
        protected ParserError(SourceLocation location) : base(location)
        {
        }
    }
}
