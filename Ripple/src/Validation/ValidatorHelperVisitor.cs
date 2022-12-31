using System;
using System.Collections.Generic;
using System.Linq;
using Ripple.AST;
using Ripple.AST.Info;
using Ripple.AST.Utils;
using Ripple.Utils;
using Ripple.Utils.Extensions;
using Ripple.Lexing;

namespace Ripple.Validation
{
    class ValidatorHelperVisitor : ASTWalkerBase
    {
        public readonly List<ValidationError> Errors = new List<ValidationError>();

        private bool m_IsGlobal = true;
        private bool m_IsUnsafe = false;
        private bool m_PreviouseBlockReturns = false;
        private readonly LocalVariableStack m_VariableStack = new LocalVariableStack();
        private readonly Stack<List< Token>> m_CurrentLifetimes = new Stack<List<Token>>();
        private Option<TypeInfo> m_CurrentReturnType = new Option<TypeInfo>();

        private readonly ASTInfo m_ASTInfo;

        public ValidatorHelperVisitor(ASTInfo astInfo)
        {
            m_ASTInfo = astInfo;
            m_ASTInfo.AST.Accept(this);
        }

        public override void VisitFuncDecl(FuncDecl funcDecl)
        {
            UpdateFunctionData(funcDecl, () =>
            {
                funcDecl.Body.Accept(this);
            });
        }

        public override void VisitBlockStmt(BlockStmt blockStmt)
        {
            m_VariableStack.PushScope();
            base.VisitBlockStmt(blockStmt);
            m_VariableStack.PopScope();
        }

        public override void VisitUnsafeBlock(UnsafeBlock unsafeBlock)
        {
            UpdateUnsafe(true, () =>
            {
                m_VariableStack.PushScope();
                base.VisitUnsafeBlock(unsafeBlock);
                m_VariableStack.PopScope();
            });
        }

        public override void VisitForStmt(ForStmt forStmt)
        {
            m_VariableStack.PushScope();
            forStmt.Init.Match(ok => ok.Accept(this));
            forStmt.Condition.Match(ok => CheckCondition(ok, forStmt.ForTok));

            forStmt.Iter.Match(ok =>
            {
                var result = ValueInfo.FromExpression(ok, m_ASTInfo, m_VariableStack, GetSafetyContext(), GetActiveLifetimesList());
                result.Match(ok => { }, fail => Errors.Add(new ValidationError(fail.Message, fail.Token)));
            });

            forStmt.Body.Accept(this);

            m_VariableStack.PopScope();
        }

        public override void VisitIfStmt(IfStmt ifStmt)
        {
            CheckCondition(ifStmt.Expr, ifStmt.IfTok);
            m_VariableStack.PushScope();
            ifStmt.Body.Accept(this);
            m_VariableStack.PopScope();

            ifStmt.ElseBody.Match(ok =>
            {
                m_VariableStack.PushScope();
                ok.Accept(this);
                m_VariableStack.PopScope();
            });
        }

        public override void VisitWhileStmt(WhileStmt whileStmt)
        {
            CheckCondition(whileStmt.Condition, whileStmt.WhileToken);
            m_VariableStack.PushScope();
            whileStmt.Body.Accept(this);
            m_VariableStack.PopScope();
        }

        public override void VisitVarDecl(VarDecl varDecl)
        {
            if(!m_IsGlobal)
            {
                ValueOfExpressionVisitor visitor = new ValueOfExpressionVisitor(m_ASTInfo, m_VariableStack, GetActiveLifetimesList(), GetSafetyContext());
                var result = VariableInfo.FromVarDecl(varDecl, visitor, m_ASTInfo.PrimaryTypes, GetActiveLifetimesList(), m_VariableStack.CurrentLifetime, GetSafetyContext());
                result.Match(ok =>
                {
                    foreach (VariableInfo variable in ok)
                    {
                        if (!m_VariableStack.TryAddVariable(variable))
                        {
                            AddError("Variable '" + variable.Name + "' has already been defined.", variable.NameToken);
                        }
                    }
                },
                fail =>
                {
                    Errors.AddRange(fail.ConvertAll(e => new ValidationError(e.Message, e.Token)));
                });
            }
        }

        private List<Token> GetActiveLifetimesList()
        {
            return m_CurrentLifetimes.SelectMany(l => l).ToList();
        }

        private void CheckCondition(Expression condition, Token errorToken)
        {
            var result = ValueInfo.FromExpression(condition, m_ASTInfo, m_VariableStack, GetSafetyContext(), GetActiveLifetimesList());
            result.Match(ok =>
            {
                if (!ok.Type.Equals(RipplePrimitives.Bool))
                    AddError("Conditional expression must evaluate to a bool.", errorToken);
            },
            fail => Errors.Add(new ValidationError(fail.Message, fail.Token)));
        }

        private void UpdateFunctionData(FuncDecl decl, Action func)
        {
            List<Token> functionLifetimes = decl.GenericParams.Match(ok => ok.Lifetimes, () => new List<Token>());

            m_CurrentReturnType = TypeInfo.FromASTType(decl.ReturnType, m_ASTInfo.PrimaryTypes, functionLifetimes, GetSafetyContext()).ToResult().ToOption();
            m_IsGlobal = false;
            m_VariableStack.PushScope();

            UpdateUnsafe(decl.UnsafeToken.HasValue, () => 
            {
                foreach((var type, var name) in decl.Param.ParamList)
                {
                    VariableInfo.FromFunctionParameter(type, name, m_VariableStack.CurrentLifetime, m_ASTInfo.PrimaryTypes, functionLifetimes, GetSafetyContext()).ToOption().Match(
                        ok =>
                        {
                            m_VariableStack.TryAddVariable(ok);
                        });
                }

                func();
            });

            m_VariableStack.PopScope();
            m_IsGlobal = true;
            m_CurrentReturnType = new Option<TypeInfo>();
        }

        private SafetyContext GetSafetyContext()
        {
            return new SafetyContext(!m_IsUnsafe);
        }

        private void UpdateUnsafe(bool isUnsafe, Action func)
        {
            bool oldUnsafe = m_IsUnsafe;
            m_IsUnsafe = isUnsafe;
            func();
            m_IsUnsafe = oldUnsafe;
        }

        private void AddError(string message, Token token)
        {
            Errors.Add(new ValidationError(message, token));
        }
    }
}
