using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Utils;
using Ripple.AST.Utils;
using Ripple.AST.Info;

namespace Ripple.Validation
{
    static class Validator
    {
        public static Result<ASTInfo, List<ValidationError>> ValidateAst(ProgramStmt programStmt)
        {
            ASTInfo info = GenerateASTInfo(programStmt);
            List<ValidationError> errors = new List<ValidationError>();

            errors.AddRange(info.Errors.ConvertAll(e => new ValidationError(e.Message, e.Token)));

            ValidatorHelperVisitor visitor = new ValidatorHelperVisitor(info);

            errors.AddRange(visitor.Errors);

            if (errors.Count > 0)
                return new Result<ASTInfo, List<ValidationError>>(errors);

            return info;
        }

        private static ASTInfo GenerateASTInfo(ProgramStmt programStmt)
        {
            List<string> primaries = RippleBuiltins.GetPrimitives();
            FunctionList functions = RippleBuiltins.GetBuiltInFunctions();
            return new ASTInfo(programStmt, primaries, functions, RippleBuiltins.GetBuiltInOperators());
        } 
    }
}
