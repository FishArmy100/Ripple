using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.Validation.Info.Statements;
using Ripple.Validation.Info.Expressions;
using Ripple.Validation.Info.Types;
using Ripple.Lexing;
using Ripple.Utils;
using Ripple.AST.Utils;
using Raucse;
using Raucse.Extensions;
using Ripple.Validation.Errors;
using Ripple.Core;

namespace Ripple.Validation.Info
{
    class StatementChecker : IStatementVisitor<Result<TypedStatement, List<ValidationError>>>
    {
        private IReadOnlyList<string> m_PrimaryTypes => m_ASTData.PrimaryTypes;

        private bool m_IsGlobal = true;
        private bool m_IsUnsafe = false;

        private Stack<bool> m_LoopsStack = new Stack<bool>();
        private Stack<List<bool>> m_BlocksReturn = new Stack<List<bool>>();
        private readonly LocalVariableStack m_VariableStack = new LocalVariableStack();
        private readonly Stack<List<Token>> m_CurrentLifetimes = new Stack<List<Token>>();
        private TypeInfo m_CurrentReturnType = null;

        private readonly ASTData m_ASTData;
        private readonly Linker m_Linker;

        public StatementChecker(ASTData data, Linker linker)
		{
            m_ASTData = data;
            m_Linker = linker;

            m_BlocksReturn.Push(new List<bool>());
		}

		public Result<TypedStatement, List<ValidationError>> VisitExprStmt(ExprStmt exprStmt)
		{
            return ExpressionChecker.CheckExpression(exprStmt.Expr, m_ASTData, m_VariableStack, GetSafetyContext(), GetActiveLifetimesList()).Match(
                ok => new Result<TypedStatement, List<ValidationError>>(new TypedExprStmt(ok.Second)),
                fail => fail);
        }

		public Result<TypedStatement, List<ValidationError>> VisitBlockStmt(BlockStmt blockStmt)
		{
            var statements = UpdateUnsafe(m_IsUnsafe, () =>
            {
                m_VariableStack.PushScope();
                var results = blockStmt.Statements.Select(s => s.Accept(this)).AggregateResults();
                m_VariableStack.PopScope();
                return results;
            });

            return statements.Match(
                ok => new TypedBlockStmt(ok),
                fail => new Result<TypedStatement, List<ValidationError>>(fail));
        }

		public Result<TypedStatement, List<ValidationError>> VisitIfStmt(IfStmt ifStmt)
		{
            var conditon = CheckCondition(ifStmt.Expr, ifStmt.IfTok);
            m_VariableStack.PushScope();
            m_BlocksReturn.Push(new List<bool>());

            var body = ifStmt.Body.Accept(this);
            bool ifBlockReturns = m_BlocksReturn.Peek().Any(v => v);

            m_BlocksReturn.Push(new List<bool>());
            m_VariableStack.PopScope();

            bool elseBlockReturns = false;

            var elseBody = ifStmt.ElseBody.Match(ok =>
            {
                m_VariableStack.PushScope();
                m_BlocksReturn.Push(new List<bool>());

                var elseBody = ok.Accept(this);
                elseBlockReturns = m_BlocksReturn.Peek().Any(v => v);

                m_BlocksReturn.Push(new List<bool>());
                m_VariableStack.PopScope();
                return elseBody;

            }, () => new Option<Result<TypedStatement, List<ValidationError>>>());

            m_BlocksReturn.Peek().Add(ifBlockReturns && elseBlockReturns);

            List<ValidationError> errors = new List<ValidationError>();
            conditon.Match(ok => { }, fail => errors.AddRange(fail));
            body.Match(ok => { }, fail => errors.AddRange(fail));
            elseBody.Match(ok => ok.Match(ok => { }, fail => errors.AddRange(fail)));

            if (errors.Any())
                return errors;

            return new TypedIfStmt(conditon.Value, body.Value, elseBody.Match(ok => ok.Value, () => default));
        }

		public Result<TypedStatement, List<ValidationError>> VisitForStmt(ForStmt forStmt)
		{
            m_BlocksReturn.Peek().Add(false);
            m_VariableStack.PushScope();
            var init = forStmt.Init.Match(ok => ok.Accept(this), () => new Option<Result<TypedStatement, List<ValidationError>>>());
            var condition = forStmt.Condition.Match(
                ok => CheckCondition(ok, forStmt.ForTok), 
                () => new Option<Result<TypedExpression, List<ValidationError>>>());

            var iter = forStmt.Iter.Match(ok => 
            {
                return ExpressionChecker.CheckExpression(ok, m_ASTData, m_VariableStack, GetSafetyContext(), GetActiveLifetimesList())
                    .Match(
                        ok => new Result<TypedExpression, List<ValidationError>>(ok.Second), 
                        fail => new Result<TypedExpression, List<ValidationError>>(fail));
                
            }, () => new Option<Result<TypedExpression, List<ValidationError>>>());

            var body = forStmt.Body.Accept(this);

            m_VariableStack.PopScope();

            // basically just checks weather or not they are all errors
            List<ValidationError> errors = new List<ValidationError>();
            init.Match(ok => ok.GetErrorOption().Match(e => errors.AddRange(e)));
            condition.Match(ok => ok.GetErrorOption().Match(e => errors.AddRange(e)));
            iter.Match(ok => ok.GetErrorOption().Match(e => errors.AddRange(e)));
            body.GetErrorOption().Match(e => errors.AddRange(e));

            if (errors.Any())
                return errors;

            return new TypedForStmt(
                init.Match(ok => ok.Value, () => default),
                condition.Match(ok => ok.Value, () => default),
                iter.Match(ok => ok.Value, () => default),
                body.Value);
        }

		public Result<TypedStatement, List<ValidationError>> VisitWhileStmt(WhileStmt whileStmt)
		{
            m_BlocksReturn.Peek().Add(false);
            var condition = CheckCondition(whileStmt.Condition, whileStmt.WhileToken);
            m_VariableStack.PushScope();

            var body = whileStmt.Body.Accept(this);

            m_VariableStack.PopScope();

            if(condition.IsError() || body.IsError())
            {
                List<ValidationError> errors = new List<ValidationError>();
                errors.AddRange(condition.GetErrorOption().MatchOrConstruct(ok => ok));
                errors.AddRange(body.GetErrorOption().MatchOrConstruct(ok => ok));
                return errors;
            }

            return new TypedWhileStmt(condition.Value, body.Value);
        }

		public Result<TypedStatement, List<ValidationError>> VisitVarDecl(VarDecl varDecl)
		{
            ExpressionCheckerVisitor visitor = new ExpressionCheckerVisitor(m_ASTData, m_VariableStack, GetActiveLifetimesList(), GetSafetyContext());
            var result = VariableInfo.FromVarDecl(varDecl, visitor, m_PrimaryTypes, GetActiveLifetimesList(), m_VariableStack.CurrentLifetime, GetSafetyContext());

            if (!m_IsGlobal)
            {
                return FromVarDecl(result);
            }
            else
            {
                return FromVarDecl(result).Match(
                    ok => ok, 
                    fail => new Result<TypedStatement, List<ValidationError>>(new List<ValidationError>())); // errors will be caught by Global Variable Finder
            }
        }

		public Result<TypedStatement, List<ValidationError>> VisitReturnStmt(ReturnStmt returnStmt)
		{
            m_BlocksReturn.Peek().Add(true);

            return returnStmt.Expr.Match(ok =>
            {
                return ExpressionChecker.CheckExpression(ok, m_ASTData, m_VariableStack, GetSafetyContext(), GetActiveLifetimesList()).Match(
                ok =>
                {
                    if (!ok.First.Type.Equals(m_CurrentReturnType))
                    {
                        SourceLocation location = returnStmt.ReturnTok.Location + returnStmt.Expr.Value.GetLocation() + returnStmt.SemiColin.Location;
                        return CreateError<TypedStatement>(new InvalidReturnTypeError(location, ok.First.Type, m_CurrentReturnType));
                    }

                    TypedReturnStmt typedReturn = new TypedReturnStmt(ok.Second);
                    return new Result<TypedStatement, List<ValidationError>>(typedReturn);
                },
                fail =>
                {
                    return fail;
                });
            },
            () =>
            {
                if (!m_CurrentReturnType.Equals(RipplePrimitives.Void))
                {
                    return CreateError<TypedStatement>(new InvalidReturnTypeError(returnStmt.ReturnTok.Location + returnStmt.SemiColin.Location, RipplePrimitives.Void, m_CurrentReturnType));
                }

                TypedReturnStmt success = new TypedReturnStmt(new Option<TypedExpression>());
                return new Result<TypedStatement, List<ValidationError>>(success);
            });
        }

		public Result<TypedStatement, List<ValidationError>> VisitContinueStmt(ContinueStmt continueStmt)
		{
            if (!IsInLoop())
            {
                return CreateError<TypedStatement>(new ContinueStatementError(continueStmt.ContinueToken.Location));
            }

            return new TypedContinueStmt();
        }

		public Result<TypedStatement, List<ValidationError>> VisitBreakStmt(BreakStmt breakStmt)
		{
			if(!IsInLoop())
            {
                return CreateError<TypedStatement>(new BreakStatementError(breakStmt.BreakToken.Location));
            }

            return new TypedBreakStmt();
		}

		public Result<TypedStatement, List<ValidationError>> VisitParameters(Parameters parameters)
		{
			throw new NotImplementedException(); // Should never get called
		}

		public Result<TypedStatement, List<ValidationError>> VisitGenericParameters(GenericParameters genericParameters)
		{
			throw new NotImplementedException(); // should never get called
		}

		public Result<TypedStatement, List<ValidationError>> VisitWhereClause(WhereClause whereClause)
		{
			throw new NotImplementedException();
		}

		public Result<TypedStatement, List<ValidationError>> VisitUnsafeBlock(UnsafeBlock unsafeBlock)
		{
            var statements = UpdateUnsafe(true, () =>
            {
                m_VariableStack.PushScope();
                var results = unsafeBlock.Statements.Select(s => s.Accept(this)).AggregateResults();
                m_VariableStack.PopScope();
                return results;
            });

            return statements.Match(
                ok => new TypedUnsafeBlock(ok),
                fail => new Result<TypedStatement, List<ValidationError>>(fail));
        }

		public Result<TypedStatement, List<ValidationError>> VisitFuncDecl(FuncDecl funcDecl)
		{
            var bodyResult = UpdateFunctionData(funcDecl);

            var declResult = FunctionInfo.FromASTFunction(funcDecl, m_PrimaryTypes).Match(
                ok =>
                {
                    return new Result<FunctionInfo, List<ValidationError>>(ok);
                },
                fail =>
                {
                    return new Result<FunctionInfo, List<ValidationError>>(new List<ValidationError>()); // errors already found by function finder
                });

            if(declResult.IsError() && bodyResult.IsError())
			{
                List<ValidationError> validationErrors = new List<ValidationError>();
                validationErrors.AddRange(declResult.Error);
                validationErrors.AddRange(bodyResult.Error);
                return validationErrors;
			}

            if (declResult.IsError())
                return declResult.Error;

            if (bodyResult.IsError())
                return bodyResult.Error;

            return new TypedFuncDecl(declResult.Value, bodyResult.Value);
        }

		public Result<TypedStatement, List<ValidationError>> VisitExternalFuncDecl(ExternalFuncDecl externalFuncDecl)
		{
            return FunctionInfo.FromASTExternalFunction(externalFuncDecl, m_PrimaryTypes).Match(
                ok =>
                {
                    Option<string> file = m_Linker.TryGetHeader(ok);
                    if (!file.HasValue())
                        return CreateError<TypedStatement>(new ExternFunctionLinkerError(ok.NameToken.Location, ok.Name, ok.FunctionType));

                    return new Result<TypedStatement, List<ValidationError>>(new TypedExternalFuncDecl(ok, file.Value));
                },
                fail =>
                {
                    return new Result<TypedStatement, List<ValidationError>>(new List<ValidationError>()); // error already found by function finder
                });
		}

		public Result<TypedStatement, List<ValidationError>> VisitFileStmt(FileStmt fileStmt)
		{
            List<TypedStatement> statements = new List<TypedStatement>();
            List<ValidationError> errors = new List<ValidationError>();
            foreach (var result in fileStmt.Statements.Select(f => f.Accept(this)))
            {
                result.Match(
                    ok => statements.Add(ok),
                    fail => errors.AddRange(fail));
            }

            if (errors.Any())
                return errors;

            return new TypedFileStmt(statements, fileStmt.RelativePath);
        }

		public Result<TypedStatement, List<ValidationError>> VisitProgramStmt(ProgramStmt programStmt)
		{
            return programStmt.Files
                .Select(f => f.Accept(this))
                .AggregateResults()
                .Match(
                    ok => new TypedProgramStmt(ok.Cast<TypedFileStmt>().ToList(), programStmt.Path),
                    fail => new Result<TypedStatement, List<ValidationError>>(fail));
		}

        private Result<TypedStatement, List<ValidationError>> FromVarDecl(Result<Pair<List<VariableInfo>, TypedExpression>, List<ValidationError>> result)
        {
            return result.Match(ok =>
            {
                List<ValidationError> errors = new List<ValidationError>();
                foreach (VariableInfo variable in ok.First)
                {
                    if (!m_VariableStack.TryAddVariable(variable))
                    {
                        ValidationError error = new DefinitionError.Variable(variable.NameToken.Location, true, variable.Name);
                        errors.Add(error);
                    }
                }

                if (errors.Any())
                {
                    return errors;
                }
                else
                {
                    TypeInfo type = ok.First[0].Type;
                    TypedVarDecl typedVar = new TypedVarDecl(ok.First[0].IsUnsafe, type, ok.First[0].IsMutable, ok.First.Select(v => v.Name).ToList(), ok.Second);
                    return new Result<TypedStatement, List<ValidationError>>(typedVar);
                }
            },
            fail =>
            {
                return fail;
            });
        }

        private List<string> GetActiveLifetimesList()
        {
            return m_CurrentLifetimes.SelectMany(l => l).ToList().ConvertAll(t => t.Text);
        }

        private Result<TypedExpression, List<ValidationError>> CheckCondition(Expression condition, Token errorToken)
        {
            var result = ExpressionChecker.CheckExpression(condition, m_ASTData, m_VariableStack, GetSafetyContext(), GetActiveLifetimesList());
            return result.Match(ok =>
            {
                if (ok.First.Type.Equals(RipplePrimitives.Bool))
                    return new Result<TypedExpression, List<ValidationError>>(ok.Second);
                return CreateError<TypedExpression>(new ConditionalValueError(errorToken.Location));
            },
            fail => fail);
        }

        private Result<TypedBlockStmt, List<ValidationError>> UpdateFunctionData(FuncDecl decl)
        {
            List<string> functionLifetimes = decl.GenericParams
                .MatchOrConstruct(ok => ok.Lifetimes.ConvertAll(l => l.Text));

            m_CurrentReturnType = TypeInfoUtils
                .FromASTType(decl.ReturnType, m_PrimaryTypes, functionLifetimes, GetSafetyContext())
                .ToOption()
                .Match(ok => ok, () => null);

            m_IsGlobal = false;
            m_VariableStack.PushScope();
            m_BlocksReturn.Push(new List<bool>());

            Result<TypedBlockStmt, List<ValidationError>> result = null;

            UpdateUnsafe(decl.UnsafeToken.HasValue, () =>
            {
                foreach ((var type, var name) in decl.Param.ParamList)
                {
                    VariableInfo.FromFunctionParameter(type, name, m_VariableStack.CurrentLifetime, m_PrimaryTypes, functionLifetimes, GetSafetyContext()).ToOption().Match(
                        ok =>
                        {
                            m_VariableStack.TryAddVariable(ok);
                        });
                }

                result = decl.Body.Accept(this).Match(
                    ok => new Result<TypedBlockStmt, List<ValidationError>>((TypedBlockStmt)ok),
                    fail => fail);
            });

            if (!m_CurrentReturnType.Equals(RipplePrimitives.Void) && !m_BlocksReturn.Peek().Any(v => v))
            {
                SourceLocation location = decl.FuncTok.Location + decl.ReturnType.GetLocation();
                return CreateError<TypedBlockStmt>(new NotAllCodePathsReturnAValueError(location));
            }
            m_BlocksReturn.Pop();

            m_VariableStack.PopScope();
            m_IsGlobal = true;
            m_CurrentReturnType = null;

            return result;
        }

        private SafetyContext GetSafetyContext()
        {
            return new SafetyContext(!m_IsUnsafe);
        }

        private bool IsInLoop() => m_LoopsStack.Any(b => b);

        private T UpdateUnsafe<T>(bool isUnsafe, Func<T> func)
        {
            bool oldUnsafe = m_IsUnsafe;
            m_IsUnsafe = isUnsafe;
            T val = func();
            m_IsUnsafe = oldUnsafe;
            return val;
        }
        private void UpdateUnsafe(bool isUnsafe, Action func)
        {
            bool oldUnsafe = m_IsUnsafe;
            m_IsUnsafe = isUnsafe;
            func();
            m_IsUnsafe = oldUnsafe;
        }

        private Result<T, List<ValidationError>> CreateError<T>(ValidationError error)
        {
            return new Result<T, List<ValidationError>>(new List<ValidationError> { error });
        }
    }
}
