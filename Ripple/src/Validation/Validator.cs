﻿using System;
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

            if (info.Errors.Count > 0)
                return info.Errors.ConvertAll(e => new ValidationError(e.Message, e.Token));

            return info;
        }

        private static ASTInfo GenerateASTInfo(ProgramStmt programStmt)
        {
            List<PrimaryTypeInfo> primaries = RippleBuiltins.GetPrimitives();
            FunctionList functions = RippleBuiltins.GetBuiltInFunctions();
            return new ASTInfo(programStmt, primaries, functions,  RippleBuiltins.GetBuiltInOperators());
        } 
    }
}
