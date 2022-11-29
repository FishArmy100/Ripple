using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.AST;

namespace Ripple.AST.Info
{
    class VariableInfo
    {
        public readonly Token NameToken;
        public readonly TypeInfo Type;
        public readonly bool IsUnsafe;

        public VariableInfo(Token nameToken, TypeInfo type, bool isUnsafe)
        {
            NameToken = nameToken;
            Type = type;
            IsUnsafe = isUnsafe;
        }

        public static List<VariableInfo> FromVarDecl(VarDecl varDecl)
        {
            TypeInfo type = TypeInfo.FromASTType(varDecl.Type);
            List<VariableInfo> infos = new List<VariableInfo>();
            foreach(Token name in varDecl.VarNames)
                infos.Add(new VariableInfo(name, type, varDecl.UnsafeToken.HasValue));

            return infos;
        }

        public string Name => NameToken.Text;
    }
}
