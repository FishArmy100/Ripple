﻿using System;
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
            ASTInfo info = GenASTInfo(programStmt);
            List<ValidationError> errors = GetASTInfoErrors(info);

            UnsafeCheck(ref errors, info);
            BasicCheck(ref errors, info);

            if (errors.Count > 0)
                return new Result<ASTInfo, List<ValidationError>>.Fail(errors);

            return new Result<ASTInfo, List<ValidationError>>.Ok(info);
        }

        private static List<ValidationError> GetASTInfoErrors(ASTInfo info)
        {
            List<ValidationError> errors = new List<ValidationError>();
            errors.AddRange(info.Errors.ConvertAll(e => new ValidationError(e.Message, e.Token)));
            return errors;
        }

        private static ASTInfo GenASTInfo(ProgramStmt programStmt)
        {
            List<PrimaryTypeInfo> builtInTypes = RippleBuiltins.GetPrimitives();
            FunctionList builtInFunctions = RippleBuiltins.GetBuiltInFunctions();
            OperatorLibrary operatorLibrary = RippleBuiltins.GetPrimitiveOperators();

            ASTInfo info = new ASTInfo(programStmt, builtInTypes, builtInFunctions, operatorLibrary);
            return info;
        }

        private static void BasicCheck(ref List<ValidationError> errors, ASTInfo info)
        {
            BasicTypeCheckStep step = new BasicTypeCheckStep(info);
            errors.AddRange(step.Errors);
        }

        private static void UnsafeCheck(ref List<ValidationError> errors, ASTInfo info)
        {
            UnsafeCheckStep step = new UnsafeCheckStep(info);
            errors.AddRange(step.Errors);
        }
    }
}
