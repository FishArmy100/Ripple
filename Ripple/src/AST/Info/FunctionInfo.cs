using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Utils;

namespace Ripple.AST.Info
{
    class FunctionInfo
    {
        public readonly bool IsUnsafe;
        public readonly Token NameToken;
        public readonly List<ParameterInfo> Parameters;
        public readonly List<Token> Lifetimes;
        public readonly TypeInfo ReturnType;

        public string Name => NameToken.Text;

        public FunctionInfo(bool isUnsafe, Token name, List<Token> lifetimes, List<ParameterInfo> parameters, TypeInfo returnType)
        {
            IsUnsafe = isUnsafe;
            NameToken = name;
            Lifetimes = lifetimes;
            Parameters = parameters;
            ReturnType = returnType;
        }

        public static Result<FunctionInfo, List<ASTInfoError>> FromASTFunction(FuncDecl funcDecl, List<PrimaryTypeInfo> primaries)
        {
            Token funcName = funcDecl.Name;
            SafetyContext safetyContext = new SafetyContext(!funcDecl.UnsafeToken.HasValue);

            List<ASTInfoError> errors = new List<ASTInfoError>();
            Func<ReferenceType, AmbiguousTypeException> errorGenerator = 
                (r) => new AmbiguousTypeException("Lifetimes in function parameters cannot be inferred yet", r.Ampersand);

            List<ParameterInfo> parameters = new List<ParameterInfo>();

            List<Token> lifetimes = funcDecl.GenericParams.Match(
                ok => ok.Lifetimes,
                () => new List<Token>());

            foreach ((var type, var name) in funcDecl.Param.ParamList)
            { 
                var result = TypeInfo.FromASTType(type, primaries, lifetimes, safetyContext, errorGenerator);
                result.ToResult().Match(ok =>
                {
                    if (parameters.Any(p => p.Name == name.Text))
                        errors.Add(new ASTInfoError("Parameter with same name '" + name.Text + "' already exists.", name));
                    else
                        parameters.Add(new ParameterInfo(name, ok));
                },
                error =>
                {
                    errors.AddRange(error);
                });
            }

            Option<TypeInfo> returned = TypeInfo.FromASTType(funcDecl.ReturnType, primaries, lifetimes, safetyContext, errorGenerator).ToResult().Match(ok =>
            {
                return ok;
            },
            error =>
            {
                errors.AddRange(error);
                return new Option<TypeInfo>();
            });

            List<LifetimeInfo> lifetimeInfos = lifetimes.ConvertAll(l => new LifetimeInfo(l));
            List<TypeInfo> parameterTypes = parameters.ConvertAll(p => p.Type);
            CheckFunctionLifetimes(lifetimeInfos, parameterTypes, returned, ref errors);

            if (errors.Count > 0)
                return errors;

            return new FunctionInfo(!safetyContext.IsSafe, funcName, lifetimes, parameters, returned.Value);
        }

        public static Result<FunctionInfo, List<ASTInfoError>> FromASTExternalFunction(ExternalFuncDecl funcDecl, List<PrimaryTypeInfo> primaries)
        {
            List<ASTInfoError> errors = new List<ASTInfoError>();
            List<ParameterInfo> parameters = new List<ParameterInfo>();
            SafetyContext safetyContext = new SafetyContext(false);

            foreach ((var type, var name) in funcDecl.Parameters.ParamList)
            {
                var result = TypeInfo.FromASTType(type, primaries, new List<Token>(),  safetyContext);
                result.ToResult().Match(ok =>
                {
                    CheckExternalFunctionType(ok, ref errors, ref parameters, name);
                },
                error =>
                {
                    errors.AddRange(error);
                });
            }

            var returned = TypeInfo.FromASTType(funcDecl.ReturnType, primaries, new List<Token>(), safetyContext);
            returned.ToResult().Match(ok =>
            {
                CheckExternalFunctionType(ok, ref errors, ref parameters, funcDecl.Name);
            },
            error =>
            {
                errors.AddRange(error);
            });

            if (errors.Count > 0)
                return errors;

            return new FunctionInfo(true, funcDecl.Name, new List<Token>(), parameters, returned.Type.Value);
        }

        private static void CheckFunctionLifetimes(List<LifetimeInfo> lifetimes, List<TypeInfo> parameters, Option<TypeInfo> returned, ref List<ASTInfoError> errors)
        {
            List<LifetimeInfo> usedParameterLifetimes = new List<LifetimeInfo>();
            foreach(TypeInfo param in parameters)
            {
                param.Walk(t =>
                {
                    if (t is TypeInfo.Reference r)
                        usedParameterLifetimes.Add(r.Lifetime);
                });
            }

            usedParameterLifetimes = usedParameterLifetimes.Distinct().ToList();

            List<LifetimeInfo> validParameterLifetimes = new List<LifetimeInfo>();
            foreach(LifetimeInfo info in usedParameterLifetimes)
            {
                if (lifetimes.Contains(info))
                    validParameterLifetimes.Add(info);
                else
                    errors.Add(new ASTInfoError("Lifetime '" + info.LifetimeToken.Value.Text + "' has not been defined.", info.LifetimeToken.Value));
            }

            if(returned.HasValue())
            {
                List<LifetimeInfo> returnedTypeLifetimes = new List<LifetimeInfo>();
                returned.Value.Walk(t =>
                {
                    if (t is TypeInfo.Reference r && !returnedTypeLifetimes.Contains(r.Lifetime))
                        returnedTypeLifetimes.Add(r.Lifetime);
                });

                foreach (LifetimeInfo info in returnedTypeLifetimes)
                {
                    if (!validParameterLifetimes.Contains(info))
                        errors.Add(new ASTInfoError("Lifetime'" + info.LifetimeToken.Value.Text + "' has not been defined and or used in a parameter.", info.LifetimeToken.Value));
                }
            }

            foreach(LifetimeInfo info in lifetimes)
            {
                if(!validParameterLifetimes.Contains(info))
                {
                    errors.Add(new ASTInfoError("Lifetime '" + info.LifetimeToken.Value.Text + "' was not used in a parameter.", info.LifetimeToken.Value));
                }
            }
        }

        private static void CheckExternalFunctionType(TypeInfo ok, ref List<ASTInfoError> errors, ref List<ParameterInfo> parameters, Token name)
        {
            bool hasReference = false;
            ok.Walk(t =>
            {
                if (t is TypeInfo.Reference)
                    hasReference = true;
            });

            if (!hasReference)
                parameters.Add(new ParameterInfo(name, ok));
            else
                errors.Add(new ASTInfoError("An external function cannot have a reference as a parameter or as a return type.", name));
        }
    }
}
