using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Validation.Info.Types;
using Ripple.Lexing;
using Ripple.Validation.Errors;
using Raucse;
using Ripple.AST;

namespace Ripple.Validation.Info.Members
{
    public class MemberVariableInfo
    {
        public readonly MemberVisibility Visibility;
        public readonly TypeInfo Type;
        public readonly Token NameToken;
        public readonly bool IsUnsafe;

        private MemberVariableInfo(MemberVisibility visibility, TypeInfo type, Token nameToken, bool isUnsafe)
        {
            Visibility = visibility;
            Type = type;
            NameToken = nameToken;
            IsUnsafe = isUnsafe;
        }

        public static Result<List<MemberVariableInfo>, List<ValidationError>> FromASTMemberVariable(VarDecl varDecl, IReadOnlyList<string> primaries, IReadOnlyList<Token> declaringTypeLifetimes, MemberVisibility visibility, SafetyContext classContext)
        {
            SafetyContext context = new SafetyContext(!varDecl.UnsafeToken.HasValue && classContext.IsSafe);
            var typeResult = TypeInfoUtils.FromASTType(varDecl.Type, primaries, declaringTypeLifetimes.Select(l => l.Text).ToList(), context, true);

            throw new NotImplementedException();
            //return typeResult.Match(
            //    ok =>
            //    {
                    
            //    },
            //    fail =>
            //    {
            //        return new Result<List<MemberVariableInfo>, List<ValidationError>>(fail);
            //    });
        }
    }
}
