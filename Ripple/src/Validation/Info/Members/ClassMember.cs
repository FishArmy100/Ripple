using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Raucse;
using Ripple.AST;
using Ripple.Validation.Errors;

namespace Ripple.Validation.Info.Members
{
    public abstract class ClassMember
    {
        public readonly MemberVisibility Visibility;

        protected ClassMember(MemberVisibility visibility)
        {
            Visibility = visibility;
        }
    }
}
