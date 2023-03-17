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

namespace Ripple.Validation.Info
{
    class StatementChecker : IStatementVisitor<Result<TypedStatement, List<ValidationError>>>
    {
        private IReadOnlyList<string> m_PrimaryTypes => m_ASTData.PrimaryTypes;
        private FunctionList m_Functions => m_ASTData.Functions;
        private IReadOnlyDictionary<string, VariableInfo> m_GlobalVariables => m_ASTData.GlobalVariables;
        private OperatorEvaluatorLibrary m_OperatorLibrary => m_ASTData.OperatorLibrary;

        private bool m_IsGlobal = true;
        private bool m_IsUnsafe = false;
        private Stack<List<bool>> m_BlocksReturn = new Stack<List<bool>>();
        private readonly LocalVariableStack m_VariableStack = new LocalVariableStack();
        private readonly Stack<List<Token>> m_CurrentLifetimes = new Stack<List<Token>>();
        private TypeInfo m_CurrentReturnType = null;

        private readonly ASTData m_ASTData;

        public StatementChecker(ASTData data)
		{
            m_ASTData = data;
		}

		public Result<TypedStatement, List<ValidationError>> VisitExprStmt(ExprStmt exprStmt)
		{
			throw new NotImplementedException();
		}

		public Result<TypedStatement, List<ValidationError>> VisitBlockStmt(BlockStmt blockStmt)
		{
			throw new NotImplementedException();
		}

		public Result<TypedStatement, List<ValidationError>> VisitIfStmt(IfStmt ifStmt)
		{
			throw new NotImplementedException();
		}

		public Result<TypedStatement, List<ValidationError>> VisitForStmt(ForStmt forStmt)
		{
			throw new NotImplementedException();
		}

		public Result<TypedStatement, List<ValidationError>> VisitWhileStmt(WhileStmt whileStmt)
		{
			throw new NotImplementedException();
		}

		public Result<TypedStatement, List<ValidationError>> VisitVarDecl(VarDecl varDecl)
		{
			throw new NotImplementedException();
		}

		public Result<TypedStatement, List<ValidationError>> VisitReturnStmt(ReturnStmt returnStmt)
		{
			throw new NotImplementedException();
		}

		public Result<TypedStatement, List<ValidationError>> VisitContinueStmt(ContinueStmt continueStmt)
		{
			throw new NotImplementedException();
		}

		public Result<TypedStatement, List<ValidationError>> VisitBreakStmt(BreakStmt breakStmt)
		{
			
		}

		public Result<TypedStatement, List<ValidationError>> VisitParameters(Parameters parameters)
		{
			throw new NotImplementedException();
		}

		public Result<TypedStatement, List<ValidationError>> VisitGenericParameters(GenericParameters genericParameters)
		{
			throw new NotImplementedException();
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
            var bodyResult = UpdateFunctionData(funcDecl, () =>
            {
                funcDecl.Body.Accept(this);
            });

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
                    return new Result<TypedStatement, List<ValidationError>>(new TypedExternalFuncDecl(ok));
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

            return new TypedFileStmt(statements, fileStmt.FilePath);
        }

		public Result<TypedStatement, List<ValidationError>> VisitProgramStmt(ProgramStmt programStmt)
		{
            return programStmt.Files
                .Select(f => f.Accept(this))
                .AggregateResults()
                .Match(
                    ok => new TypedProgramStmt(ok.Cast<TypedFileStmt>().ToList()),
                    fail => new Result<TypedStatement, List<ValidationError>>(fail));
		}

        private List<string> GetActiveLifetimesList()
        {
            return m_CurrentLifetimes.SelectMany(l => l).ToList().ConvertAll(t => t.Text);
        }

        private Option<List<ValidationError>> CheckCondition(Expression condition, Token errorToken)
        {
            var result = ExpressionChecker.CheckExpression(condition, m_ASTData, m_VariableStack, GetSafetyContext(), GetActiveLifetimesList());
            return result.Match(ok =>
            {
                if (ok.First.Type.EqualsWithoutFirstMutable(RipplePrimitives.Bool))
                    return new ();
                return CreateError("Conditional expression must evaluate to a bool.", errorToken);
            },
            fail => fail.Select(e => new ValidationError(e.Message, e.Token)).ToList());
        }

        private Result<TypedBlockStmt, List<ValidationError>> UpdateFunctionData(FuncDecl decl, Action func)
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

                func();
            });

            if (!m_CurrentReturnType.Equals(RipplePrimitives.Void) && !m_BlocksReturn.Peek().Any(v => v))
            {
                return CreateError("Not all code paths return a value.", decl.FuncTok);
            }
            m_BlocksReturn.Pop();

            m_VariableStack.PopScope();
            m_IsGlobal = true;
            m_CurrentReturnType = null;

            return decl.Body.Accept(this).Match(
                ok => new Result<TypedBlockStmt, List<ValidationError>>((TypedBlockStmt)ok), 
                fail => fail);
        }

        private SafetyContext GetSafetyContext()
        {
            return new SafetyContext(!m_IsUnsafe);
        }

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

        private List<ValidationError> CreateError(string text, Token token)
		{
            return new List<ValidationError> { new ValidationError(text, token) };
		}
    }
}
