using System;
using System.Collections.Generic;
using System.Linq;
using Ripple.AST;
using Ripple.AST.Info;
using Ripple.AST.Utils;
using Ripple.Utils;
using Ripple.Utils.Extensions;
using Ripple.Lexing;
using Ripple.AST.Info.Types;

namespace Ripple.Validation
{
    class ValidatorHelperVisitor : ASTWalkerBase
    {
        public readonly List<ValidationError> Errors = new List<ValidationError>();

        private bool m_IsGlobal = true;
        private bool m_IsUnsafe = false;
        private Stack<List<bool>> m_BlocksReturn = new Stack<List<bool>>();
        private readonly LocalVariableStack m_VariableStack = new LocalVariableStack();
        private readonly Stack<List<Token>> m_CurrentLifetimes = new Stack<List<Token>>();
        private TypeInfo m_CurrentReturnType = null;

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
            m_BlocksReturn.Peek().Add(false);
            m_VariableStack.PushScope();
            forStmt.Init.Match(ok => ok.Accept(this));
            forStmt.Condition.Match(ok => CheckCondition(ok, forStmt.ForTok));

            forStmt.Iter.Match(ok =>
            {
                var result = ValueInfo.FromExpression(ok, m_ASTInfo, m_VariableStack, GetSafetyContext(), GetActiveLifetimesList());
                result.Match(ok => { }, fail => Errors.AddRange(fail.ConvertAll(e => new ValidationError(e.Message, e.Token))));
            });

            forStmt.Body.Accept(this);

            m_VariableStack.PopScope();
        }

        public override void VisitIfStmt(IfStmt ifStmt)
        {
            CheckCondition(ifStmt.Expr, ifStmt.IfTok);
            m_VariableStack.PushScope();
            m_BlocksReturn.Push(new List<bool>());

            ifStmt.Body.Accept(this);
            bool ifBlockReturns = m_BlocksReturn.Peek().Any(v => v);

            m_BlocksReturn.Push(new List<bool>());
            m_VariableStack.PopScope();

            bool elseBlockReturns = false;

            ifStmt.ElseBody.Match(ok =>
            {
                m_VariableStack.PushScope();
                m_BlocksReturn.Push(new List<bool>());

                ok.Accept(this);
                elseBlockReturns = m_BlocksReturn.Peek().Any(v => v);

                m_BlocksReturn.Push(new List<bool>());
                m_VariableStack.PopScope();
            });

            m_BlocksReturn.Peek().Add(ifBlockReturns && elseBlockReturns);
        }

        public override void VisitWhileStmt(WhileStmt whileStmt)
        {
            m_BlocksReturn.Peek().Add(false);
            CheckCondition(whileStmt.Condition, whileStmt.WhileToken);
            m_VariableStack.PushScope();
            whileStmt.Body.Accept(this);
            m_VariableStack.PopScope();
        }

        public override void VisitExprStmt(ExprStmt exprStmt)
        {
            ValueInfo.FromExpression(exprStmt.Expr, m_ASTInfo, m_VariableStack, GetSafetyContext(), GetActiveLifetimesList()).Match(
                ok => { },
                fail => Errors.AddRange(fail.ConvertAll(e => new ValidationError(e.Message, e.Token))));
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

        public override void VisitReturnStmt(ReturnStmt returnStmt)
        {
            m_BlocksReturn.Peek().Add(true);

            returnStmt.Expr.Match(ok =>
            {
                ValueInfo.FromExpression(ok, m_ASTInfo, m_VariableStack, GetSafetyContext(), GetActiveLifetimesList()).Match(
                ok => 
                {
                    if(!ok.Type.Equals(m_CurrentReturnType))
                    {
                        AddError("Cannot return value of type '" + ok.Type + "' from a function that returns '" + m_CurrentReturnType + "'.", returnStmt.ReturnTok);
                    }
                },
                fail => 
                {
                    List<ValidationError> errors = fail.ConvertAll(e => new ValidationError(e.Message, e.Token));
                    Errors.AddRange(errors); 
                });
            },
            () =>
            {
                if (!m_CurrentReturnType.Equals(RipplePrimitives.Void))
                {
                    AddError("Cannot return a expression from a void function.", returnStmt.ReturnTok);
                }
            });
        }

        private List<LifetimeInfo> GetActiveLifetimesList()
        {
            return m_CurrentLifetimes.SelectMany(l => l).ToList().ConvertAll(t => new LifetimeInfo(t));
        }

        private void CheckCondition(Expression condition, Token errorToken)
        {
            var result = ValueInfo.FromExpression(condition, m_ASTInfo, m_VariableStack, GetSafetyContext(), GetActiveLifetimesList());
            result.Match(ok =>
            {
                if (ok.Type.EqualsWithoutFirstMutable(RipplePrimitives.Bool))
                    return;
                AddError("Conditional expression must evaluate to a bool.", errorToken);
            },
            fail => Errors.AddRange(fail.ConvertAll(e => new ValidationError(e.Message, e.Token))));
        }

        private void UpdateFunctionData(FuncDecl decl, Action func)
        {
            List<LifetimeInfo> functionLifetimes = decl.GenericParams.Match(ok => ok.Lifetimes.ConvertAll(l => new LifetimeInfo(l)), () => new List<LifetimeInfo>());

            m_CurrentReturnType = TypeInfoUtils.FromASTType(decl.ReturnType, m_ASTInfo.PrimaryTypes, functionLifetimes, GetSafetyContext())
                .ToOption()
                .Match(ok => ok, () => null);

            m_IsGlobal = false;
            m_VariableStack.PushScope();
            m_BlocksReturn.Push(new List<bool>());

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

            if (!m_CurrentReturnType.Equals(RipplePrimitives.Void) && !m_BlocksReturn.Peek().Any(v => v))
            {
                AddError("Not all code paths return a value.", decl.FuncTok);
            }
            m_BlocksReturn.Pop();

            m_VariableStack.PopScope();
            m_IsGlobal = true;
            m_CurrentReturnType = null;
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
