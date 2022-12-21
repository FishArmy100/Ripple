using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST.Utils;
using Ripple.AST.Info;
using Ripple.AST;
using Ripple.Lexing;

namespace Ripple.Validation
{
    class LifetimeCheckStep : ASTWalkerBase
    {
        public readonly List<ValidationError> Errors = new List<ValidationError>();

        public LifetimeCheckStep(ASTInfo info)
        {
            info.AST.Accept(this);
        }

        public override void VisitFuncDecl(FuncDecl funcDecl)
        {
            FunctionInfo info = new FunctionInfo(funcDecl);
            if (!CheckFunctionForReferencesWithoutLifetimes(info))
                return;

            List<LifetimeInfo> declaredLifetimes = info.Lifetimes;
            List<LifetimeInfo> usedLifetimes = GetUsedLifetimes(info);

            List<LifetimeInfo> validUsedLifetimes = new List<LifetimeInfo>();
            foreach(LifetimeInfo lifetime in usedLifetimes)
            {
                if (!declaredLifetimes.Contains(lifetime))
                    AddError("Use of undeclared lifetime "/* + lifetime.Lifetime.Text + "."*/, new Token());
                else
                    validUsedLifetimes.Add(lifetime);
            }

            foreach(LifetimeInfo liftime in declaredLifetimes)
            {
                if (!validUsedLifetimes.Contains(liftime))
                    AddError("Declared lifetime: "/* + liftime.Lifetime.Text + " is not used"*/, new Token());
            }
        }

        private static List<LifetimeInfo> GetUsedLifetimes(FunctionInfo info)
        {
            List<LifetimeInfo> liftimes = 
                info.Parameters
                .ConvertAll(p => GetReferences(p.Type))
                .SelectMany(r => r)
                .ToList()
                .ConvertAll(r => r.Lifetime.Value);

            List<LifetimeInfo> returnedLifetimes = GetReferences(info.ReturnType).ConvertAll(r => r.Lifetime.Value);
            liftimes.AddRange(returnedLifetimes);

            return liftimes.Distinct().ToList();
        }

        private static List<TypeInfo.Reference> GetReferences(TypeInfo info)
        {
            List<TypeInfo.Reference> references = new List<TypeInfo.Reference>();
            info.Walk(t =>
            {
                if (t is TypeInfo.Reference r)
                    references.Add(r);
            });
            return references;
        }

        private bool CheckFunctionForReferencesWithoutLifetimes(FunctionInfo info)
        {
            bool foundError = false;

            foreach (TypeInfo param in info.Parameters.ConvertAll(p => p.Type))
            {
                param.Walk(p =>
                {
                    if (p is TypeInfo.Reference r)
                    {
                        if (!r.Lifetime.HasValue())
                        {
                            AddError("Currently, a parameter reference must have a lifetime, sorry, will fix it lator :)", r.GetPrimaries()[0].Name);
                            foundError = true;
                        }
                    }
                });
            }

            info.ReturnType.Walk(p =>
            {
                if (p is TypeInfo.Reference r)
                {
                    if (!r.Lifetime.HasValue())
                    {
                        AddError("Currently, a returned reference must have a lifetime, sorry, will fix it lator :)", r.GetPrimaries()[0].Name);
                        foundError = true;
                    }
                }
            });

            return !foundError;
        }

        public override void VisitExternalFuncDecl(ExternalFuncDecl externalFuncDecl)
        {
            FunctionInfo info = new FunctionInfo(externalFuncDecl);
            foreach(TypeInfo param in info.Parameters.ConvertAll(p => p.Type))
            {
                param.Walk(p =>
                {
                    if (p is TypeInfo.Reference r)
                        AddError("Reference type cannot be in a 'extern' function declaration.", r.GetPrimaries()[0].Name);
                });
            }

            info.ReturnType.Walk(p =>
            {
                if (p is TypeInfo.Reference r)
                    AddError("Reference type cannot be in a 'extern' function declaration.", r.GetPrimaries()[0].Name);
            });
        }

        private void AddError(string message, Token token)
        {
            Errors.Add(new ValidationError(message, token));
        }
    }
}
