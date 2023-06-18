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

        public static Result<ConstructorInfo, List<ValidationError>> FromASTConstructor(ConstructorDecl constructorDecl, IReadOnlyList<string> primaries, string declaringClass, IReadOnlyList<Token> classLifetimes, MemberVisibility visibility)
        {
            List<ValidationError> errors = new List<ValidationError>();

            if (constructorDecl.Identifier.Text != declaringClass)
                errors.Add(new InvalidConstructorNameError(constructorDecl.Identifier.Location, declaringClass, constructorDecl.Identifier.Text));

            //var lifetimes = CheckLifetimes(constructorDecl.GenericParameters, classLifetimes);

            throw new NotImplementedException();
        }

        private static Pair<List<Token>, List<ValidationError>> CheckLifetimes(Option<GenericParameters> genericParameters, IReadOnlyList<string> predeclaredLifetimes)
        {
            if (!genericParameters.HasValue())
                return new Pair<List<Token>, List<ValidationError>>(new List<Token>(), new List<ValidationError>());

            List<Token> lifetimes = new List<Token>();
            List<ValidationError> errors = new List<ValidationError>();

            // makes sure all lifetimes have not been previously defined
            foreach(Token lifetime in genericParameters.Value.Lifetimes)
            {
                if (predeclaredLifetimes.Contains(lifetime.Text))
                    errors.Add(new DefinitionError.Lifetime(lifetime.Location, true, lifetime.Text));
                else
                    lifetimes.Add(lifetime);
            }

            return new Pair<List<Token>, List<ValidationError>>(lifetimes, errors);
        }

        private static bool WhereLifetimesUsed(IReadOnlyList<string> lifetimes)
        {
            throw new NotImplementedException();
        }

        private static Pair<List<ParameterInfo>, List<ValidationError>> GetParameters(Parameters parameters, IReadOnlyList<string> primaryTypes, IReadOnlyList<string> classLifetimes, IReadOnlyList<string> constructorLifetimes, SafetyContext safetyContext)
        {
            List<string> allLifetimes = new List<string>();
            allLifetimes.AddRange(classLifetimes);
            allLifetimes.AddRange(constructorLifetimes);

            List<ParameterInfo> parameterInfos = new List<ParameterInfo>();
            List<ValidationError> errors = new List<ValidationError>();

            foreach(var (type, name) in parameters.ParamList)
            {
                var result = TypeInfoUtils.FromASTType(type, primaryTypes, allLifetimes, safetyContext, true);
                result.Match(
                    ok =>
                    {
                        parameterInfos.Add(new ParameterInfo(name, ok));
                    },
                    fail =>
                    {
                        errors.AddRange(fail);
                    });
            }

            return new Pair<List<ParameterInfo>, List<ValidationError>>(parameterInfos, errors);
        }
    }
}
