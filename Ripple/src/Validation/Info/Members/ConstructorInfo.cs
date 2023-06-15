using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Validation.Info.Functions;
using Ripple.Validation.Errors;
using Raucse;
using Ripple.AST;
using Ripple.Validation.Info.Types;

namespace Ripple.Validation.Info.Members
{
    public class ConstructorInfo : ClassMember
    {
        public readonly Token ClassName;
        public readonly IReadOnlyList<Token> Lifetimes;
        public readonly IReadOnlyList<ParameterInfo> Parameters;

        private ConstructorInfo(MemberVisibility visibility, Token className, IReadOnlyList<ParameterInfo> parameters) : base(visibility)
        {
            ClassName = className;
            Parameters = parameters;
        }

        public static Result<ConstructorInfo, List<ValidationError>> FromASTConstructor(ConstructorDecl constructorDecl, IReadOnlyList<string> primaries, string declaringClass, MemberVisibility visibility)
        {
            List<ValidationError> errors = new List<ValidationError>();

            if (constructorDecl.Identifier.Text != declaringClass)
                errors.Add(new InvalidConstructorNameError(constructorDecl.Identifier.Location, declaringClass, constructorDecl.Identifier.Text));

            List<Token> lifetimes = constructorDecl.GenericParameters.Match(
                        ok => ok.Lifetimes.Select(l => l).ToList(),
                        () => new List<Token>());

            throw new NotImplementedException();
        }
    }
}
