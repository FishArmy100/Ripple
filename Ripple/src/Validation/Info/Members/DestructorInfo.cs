using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Raucse;
using Ripple.Validation.Errors;
using Ripple.AST;
using Ripple.Core;

namespace Ripple.Validation.Info.Members
{
    public class DestructorInfo
    {
        public readonly MemberVisibility Visibility;
        public readonly Token NameToken;
        public readonly bool IsUnsafe;

        private DestructorInfo(MemberVisibility visibility, Token nameToken, bool isUnsafe)
        {
            Visibility = visibility;
            NameToken = nameToken;
            IsUnsafe = isUnsafe;
        }

        public static Result<DestructorInfo, ValidationError> FromASTDestructor(DestructorDecl destructorDecl, string className, MemberVisibility visibility)
        {
            if (destructorDecl.Identifier.Text != className)
            {
                SourceLocation location = destructorDecl.TildaToken.Location + destructorDecl.Identifier.Location;
                return new InvalidDestructorNameError(location, className, destructorDecl.Identifier.Text);
            }

            return new DestructorInfo(visibility, destructorDecl.Identifier, destructorDecl.UnsafeToken.HasValue);

        }
    }
}
