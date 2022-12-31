using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils;

namespace Ripple.AST.Info
{
    class TypeInfoCreationResult
    {
        public readonly Option<TypeInfo> Type;
        public readonly Option<ASTInfoError> NoLifetimeError;
        public readonly List<ASTInfoError> InvalidTypeErrors;

        public TypeInfoCreationResult(Option<TypeInfo> type, Option<ASTInfoError> noLifetimeError, List<ASTInfoError> invalidTypeErrors)
        {
            Type = type;
            NoLifetimeError = noLifetimeError;
            InvalidTypeErrors = invalidTypeErrors;
        }

        public Result<TypeInfo, List<ASTInfoError>> ToResult()
        {
            return Type.Match(ok =>
            {
                return new Result<TypeInfo, List<ASTInfoError>>(ok);
            },
            () =>
            {
                List<ASTInfoError> errors = new List<ASTInfoError>();
                NoLifetimeError.Match(ok => errors.Add(ok));
                errors.AddRange(InvalidTypeErrors);
                return new Result<TypeInfo, List<ASTInfoError>>(errors);
            });
        }
    }
}
