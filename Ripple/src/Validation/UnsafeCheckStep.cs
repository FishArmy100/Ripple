using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST.Utils;
using Ripple.AST;
using Ripple.AST.Info;
using Ripple.Lexing;

namespace Ripple.Validation
{
    class UnsafeCheckStep : ASTWalkerBase
    {
        public readonly List<ValidationError> Errors = new List<ValidationError>();
        private bool m_IsInUnsafe = false;
        private bool m_IsInFunction = false;

        private readonly ASTInfo m_AST;
        private readonly LocalVariableStack m_VariableStack = new LocalVariableStack();

        public UnsafeCheckStep(ASTInfo astInfo)
        {
            m_AST = astInfo;
            astInfo.AST.Accept(this);
        }

        public override void VisitVarDecl(VarDecl varDecl)
        {
            UpdateUnsafe(varDecl.UnsafeToken.HasValue, () =>
            {
                CheckType(varDecl.Type);
                base.VisitVarDecl(varDecl);

                if(m_IsInFunction)
                {
                    foreach (VariableInfo variableInfo in VariableInfo.FromVarDecl(varDecl))
                        m_VariableStack.AddVariable(variableInfo);
                }
            });
        }

        public override void VisitFuncDecl(FuncDecl funcDecl)
        {
            m_IsInFunction = true;
            UpdateUnsafe(funcDecl.UnsafeToken.HasValue, () =>
            {
                CheckType(funcDecl.ReturnType);

                m_VariableStack.PushScope();
                foreach ((TypeName type, Token name) in funcDecl.Param.ParamList)
                    m_VariableStack.AddVariable(new VariableInfo(name, TypeInfo.FromASTType(type), false));

                base.VisitFuncDecl(funcDecl);

                m_VariableStack.PopScope();
            });
            m_IsInFunction = false;
        }

        public override void VisitExternalFuncDecl(ExternalFuncDecl externalFuncDecl)
        {
            UpdateUnsafe(true, () => 
            {
                CheckType(externalFuncDecl.ReturnType);
                base.VisitExternalFuncDecl(externalFuncDecl);
            });
        }

        public override void VisitUnsafeBlock(UnsafeBlock unsafeBlock)
        {
            UpdateUnsafe(true, () => 
            {
                base.VisitUnsafeBlock(unsafeBlock);
            });
        }

        public override void VisitParameters(Parameters parameters)
        {
            foreach(var param in parameters.ParamList)
            {
                CheckType(param.Item1);
            }
        }

        public override void VisitForStmt(ForStmt forStmt)
        {
            m_VariableStack.PushScope();
            forStmt.Init.Match(ok => ok.Accept(this));
            forStmt.Condition.Match(ok => ok.Accept(this));
            forStmt.Iter.Match(ok => ok.Accept(this));

            m_VariableStack.PushScope();
            forStmt.Body.Accept(this);
            m_VariableStack.PopScope();

            m_VariableStack.PopScope();
        }

        public override void VisitBlockStmt(BlockStmt blockStmt)
        {
            m_VariableStack.PushScope();
            base.VisitBlockStmt(blockStmt);
            m_VariableStack.PopScope();
        }

        public override void VisitWhileStmt(WhileStmt whileStmt)
        {
            m_VariableStack.PushScope();
            base.VisitWhileStmt(whileStmt);
            m_VariableStack.PopScope();
        }

        public override void VisitIfStmt(IfStmt ifStmt)
        {
            m_VariableStack.PushScope();
            ifStmt.Body.Accept(this);
            m_VariableStack.PopScope();

            m_VariableStack.PushScope();
            ifStmt.ElseBody.Match(body => body.Accept(this));
            m_VariableStack.PopScope();
        }

        public override void VisitCast(Cast cast)
        {
            TypeInfo info = TypeInfo.FromASTType(cast.TypeToCastTo);
            if (!m_IsInUnsafe && info.IsUnsafe())
            {
                string typeName = TypeNamePrinter.PrintType(cast.TypeToCastTo);
                AddError(cast.AsToken, "Can't cast to unsafe type: " + typeName + ", in a safe context.");
            }

            base.VisitCast(cast);
        }

        public override void VisitCall(Call call)
        {
            if(call.Callee is Identifier id)
            {
                string name = id.Name.Text;
                if(!m_VariableStack.ContainsVariable(name) && !m_AST.GlobalVariables.ContainsKey(name))
                {
                    List<TypeInfo> args = new List<TypeInfo>();
                    foreach(Expression arg in call.Args)
                    {
                        bool failed = false;

                        TypeInfo.FromExpression(m_AST, m_VariableStack, arg, new Utils.Option<TypeInfo>()).Match(
                            ok => args.Add(ok),
                            fail => failed = true);

                        if (failed)
                        {
                            base.VisitCall(call);
                            return;
                        }
                    }

                    if (m_AST.Functions.TryGetFunction(name, args, out FunctionInfo info))
                    {
                        if (!m_IsInUnsafe && info.IsUnsafe)
                            AddError(id.Name, "Use of unsafe funciton: " + name + " in a safe context.");
                    }
                }
            }

            base.VisitCall(call);
        }

        public override void VisitIdentifier(Identifier identifier)
        {
            string name = identifier.Name.Text;
            if(m_VariableStack.TryGetVariable(name, out VariableInfo info))
            {
                if (!m_IsInUnsafe && info.IsUnsafe)
                    AddError(info.NameToken, "Cannot refer to unsafe variable: " + name + " in a safe context.");
            }
            else if(m_AST.GlobalVariables.TryGetValue(name, out info))
            {
                if (!m_IsInUnsafe && info.IsUnsafe)
                    AddError(info.NameToken, "Cannot refer to unsafe variable: " + name + " in a safe context.");
            }
        }

        private void UpdateUnsafe(bool isUnsafe, Action func)
        {
            bool oldContext = m_IsInUnsafe;
            m_IsInUnsafe = m_IsInUnsafe || isUnsafe;
            func();
            m_IsInUnsafe = oldContext;
        }

        private void CheckType(TypeName typeName)
        {
            TypeInfo returnType = TypeInfo.FromASTType(typeName);
            if (returnType.IsUnsafe() && !m_IsInUnsafe)
            {
                Token token = returnType.GetPrimaries()[0].Name;
                string message = "Unsafe type: " + TypeNamePrinter.PrintType(typeName) + ", is unsafe and not in an unsafe context.";
                AddError(token, message);
            }
        }

        private void AddError(Token token, string message)
        {
            Errors.Add(new ValidationError(message, token));
        }
    }
}
