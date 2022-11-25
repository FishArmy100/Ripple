using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.AST.Info;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Utils;
using Ripple.AST.Utils;

namespace Ripple.Validation
{
    static class Validator
    {
        public static Result<ASTInfo, List<ValidationError>> ValidateAst(ProgramStmt programStmt)
        {
            List<PrimaryTypeInfo> builtInTypes = RippleBuiltins.GetPrimitives();
            ASTInfo info = new ASTInfo(programStmt, builtInTypes);

            if(info.Errors.Count > 0)
            {
                return new Result<ASTInfo, List<ValidationError>>.Fail(info.Errors
                    .AsEnumerable()
                    .ToList()
                    .ConvertAll(e => new ValidationError(e.Message, e.Token)));
            }

            return new Result<ASTInfo, List<ValidationError>>.Ok(info);
        }
    }
}
