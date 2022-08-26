﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Lexing
{
    static class RippleKeywords
    {
        public static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
        {
            { "for",        TokenType.For       },
            { "if",         TokenType.If        },
            { "true",       TokenType.True      },
            { "false",      TokenType.False     },
            { "func",       TokenType.Func      },
            { "return",     TokenType.Return    },
        };
    }
}