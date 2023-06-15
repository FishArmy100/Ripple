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

        protected static Pair<List<Token>, List<ValidationError>> CheckLifetimes(Option<GenericParameters> genericParameters, IReadOnlyList<Token> classLifetimes)
        {
            return genericParameters.Match(
                ok =>
                {
                    List<ValidationError> errors = new List<ValidationError>();
                    var lifetimes = ok.Lifetimes.Select(l =>
                    {
                        if (classLifetimes.Contains(l))
                        {
                            errors.Add(new DefinitionError.Lifetime(l.Location, true, l.Text));
                            return new Option<Token>();
                        }
                        return new Option<Token>(l);
                    }).AllValids();

                    return new Pair<List<Token>, List<ValidationError>>(lifetimes, errors);
                },
                () =>
                {
                    return new Pair<List<Token>, List<ValidationError>>(new List<Token>(), new List<ValidationError>());
                });
        }
    }
}
