using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    struct TypeCheckerError
    {
        public readonly string Message;

        public TypeCheckerError(string message)
        {
            Message = message;
        }
    }
}
