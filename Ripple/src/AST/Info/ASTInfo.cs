using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils;

namespace Ripple.AST.Info
{
    class ASTInfo
    {
        public readonly List<PrimaryTypeInfo> PrimaryTypes;
        public readonly List<TypeInfo> CompositTypes;
        public readonly List<ASTInfoError> Errors;

        public readonly ProgramStmt AST;

        public ASTInfo(ProgramStmt ast, List<PrimaryTypeInfo> additionalTypes)
        {
            PrimaryTypes = additionalTypes;

            ASTInfoGenerationHelper helper = new ASTInfoGenerationHelper(ast, PrimaryTypes);
            CompositTypes = helper.CompositTypes;
            Errors = helper.Errors;

            AST = ast;
        }

        private class ASTInfoGenerationHelper : AstWalkerBase
        {
            private readonly List<PrimaryTypeInfo> m_Primaries;
            public List<TypeInfo> CompositTypes { get; private set; } = new List<TypeInfo>();
            public List<ASTInfoError> Errors { get; private set; } = new List<ASTInfoError>();

            public ASTInfoGenerationHelper(ProgramStmt ast, List<PrimaryTypeInfo> primaries)
            {
                m_Primaries = primaries;
                ast.Accept(this);
            }

            private Option<TypeInfo> CheckType(TypeName typeName)
            {
                TypeInfo info = TypeInfo.FromASTType(typeName);
                List<PrimaryTypeInfo> primaries = info.GetPrimaryTypes();

                bool isValidType = true;
                foreach(PrimaryTypeInfo primary in primaries)
                {
                    if(!m_Primaries.Contains(primary))
                    {
                        isValidType = false;
                        Errors.Add(new ASTInfoError("Undefined type: " + primary.Name.Text, primary.Name));
                    }
                }

                if (isValidType && !CompositTypes.Contains(info))
                {
                    CompositTypes.Add(info);
                    return info;
                }

                return new Option<TypeInfo>();
            }

            public override void VisitFuncDecl(FuncDecl funcDecl)
            {
                CheckType(funcDecl.ReturnType);
                foreach (TypeName typeName in funcDecl.Param.ParamList.ConvertAll(p => p.Item1))
                    CheckType(typeName);
                base.VisitFuncDecl(funcDecl);
            }

            public override void VisitExternalFuncDecl(ExternalFuncDecl externalFuncDecl)
            {
                CheckType(externalFuncDecl.ReturnType);
                foreach (TypeName typeName in externalFuncDecl.Parameters.ParamList.ConvertAll(p => p.Item1))
                    CheckType(typeName);
                base.VisitExternalFuncDecl(externalFuncDecl);
            }

            public override void VisitVarDecl(VarDecl varDecl)
            {
                CheckType(varDecl.Type);
                base.VisitVarDecl(varDecl);
            }
        }
    }
}
