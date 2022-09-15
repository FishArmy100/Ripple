using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.Lexing;

namespace Ripple.Transpiling
{
    class TranspilerVisitor : IStatementVisitor<CStatement>, IExpressionVisitor<CExpression>
    {
        private readonly string m_FileName;

        public TranspilerVisitor(string fileName)
        {
            m_FileName = fileName;
        }

        public string Transpile(FileStmt fileStmt)
        {
            CStatement.Package package = fileStmt.Accept(this) as CStatement.Package;
            return package.ConvertToCCode(0);
        }

        public CStatement VisitExprStmt(ExprStmt exprStmt)
        {
            CExpression expression = exprStmt.Expr.Accept(this);
            return new CStatement.ExprStmt(expression);
        }

        public CStatement VisitBlockStmt(BlockStmt blockStmt)
        {
            List<CStatement> statements = new List<CStatement>();
            foreach (Statement statement in blockStmt.Statements)
                statements.Add(statement.Accept(this));

            return new CStatement.Block(statements);
        }

        public CStatement VisitIfStmt(IfStmt ifStmt)
        {
            CExpression condition = ifStmt.Expr.Accept(this);
            CStatement body = ifStmt.Body.Accept(this);
            return new CStatement.If(condition, body);
        }

        public CStatement VisitForStmt(ForStmt forStmt)
        {
            CStatement.Var initalizer = null;
            if (forStmt.Init is VarDecl v)
                initalizer = v.Accept(this) as CStatement.Var;


            CExpression condition = null;
            if(forStmt.Condition is not null)
                condition = forStmt.Condition.Accept(this);

            CExpression iterator = null;
            if (forStmt.Iter is not null)
                iterator = forStmt.Iter.Accept(this);

            CStatement body = forStmt.Body.Accept(this);
            
            return new CStatement.For(initalizer, condition, iterator, body);
        }

        public CStatement VisitVarDecl(VarDecl varDecl)
        {
            string typeName = varDecl.TypeName.Text;
            List<string> varNames = varDecl.VarNames.ConvertAll(t => t.Text);
            CExpression initalizer = varDecl.Expr.Accept(this);

            return new CStatement.Var(typeName, varNames, initalizer);
        }

        public CStatement VisitReturnStmt(ReturnStmt returnStmt)
        {
            return new CStatement.Return(returnStmt.Expr.Accept(this));
        }

        public CStatement VisitParameters(Parameters parameters) // Unused
        {
            throw new NotImplementedException();
        }

        public CStatement VisitFuncDecl(FuncDecl funcDecl)
        {
            string funcName = funcDecl.Name.Text;
            string returnType = funcDecl.ReturnType.Text;
            List<(string, string)> parameters = funcDecl.Param.ParamList.ConvertAll(p => (p.Item1.Text, p.Item2.Text));
            CStatement.Block body = funcDecl.Body.Accept(this) as CStatement.Block;

            return new CStatement.Func(returnType, funcName, parameters, body);
        }

        public CStatement VisitFileStmt(FileStmt fileStmt)
        {
            List<CStatement.Func> functions = new List<CStatement.Func>();
            List<CStatement.Var> variables = new List<CStatement.Var>();

            foreach(Statement statement in fileStmt.Statements)
            {
                CStatement cStatement = statement.Accept(this);
                switch (cStatement)
                {
                    case CStatement.Func f:
                        functions.Add(f);
                        break;
                    case CStatement.Var v:
                        variables.Add(v);
                        break;
                    default:
                        throw new InvalidCastException("Translated statement must be a declaration");
                }
            }

            return new CStatement.Package(m_FileName, functions, variables, new List<string>());
        }

        public CExpression VisitBinary(Binary binary)
        {
            CExpression left = binary.Left.Accept(this);
            CExpression right = binary.Right.Accept(this);
            CBinaryOperator op = ConvertBinaryOperator(binary.Op.Type);
            return new CExpression.Binary(left, op, right);
        }

        public CExpression VisitCall(Call call)
        {
            List<CExpression> args = new List<CExpression>();

            foreach (Expression arg in call.Args)
                args.Add(arg.Accept(this));

            return new CExpression.Call(args, call.Identifier.Text);
        }

        public CExpression VisitGrouping(Grouping grouping)
        {
            return new CExpression.Grouping(grouping.Expr.Accept(this));
        }

        public CExpression VisitIdentifier(Identifier identifier)
        {
            return CExpression.Value.FromIdentifier(identifier.Name.Text);
        }

        public CExpression VisitLiteral(Literal literal)
        {
            string text = literal.Val.Text;
            TokenType type = literal.Val.Type;
            return type switch
            {
                TokenType.IntagerLiteral => CExpression.Value.FromInt(int.Parse(text)),
                TokenType.FloatLiteral => CExpression.Value.FromFloat(float.Parse(text)),
                TokenType.True => CExpression.Value.FromBool(true),
                TokenType.False => CExpression.Value.FromBool(false),
                _ => throw new ArgumentException("Token is not a literal")
            };
        }

        public CExpression VisitUnary(Unary unary)
        {
            CExpression operand = unary.Expr.Accept(this);
            CUnaryOperator op = ConvertUnaryOperator(unary.Op.Type);
            return new CExpression.Unary(operand, op);
        }

        private static CBinaryOperator ConvertBinaryOperator(TokenType type)
        {
            return type switch
            {
                TokenType.Plus => CBinaryOperator.Plus,
                TokenType.Minus => CBinaryOperator.Minus,
                TokenType.Star => CBinaryOperator.Times,
                TokenType.Slash => CBinaryOperator.Divide,
                TokenType.Equal => CBinaryOperator.Assign,
                TokenType.EqualEqual => CBinaryOperator.EqualEqual,
                TokenType.BangEqual => CBinaryOperator.BangEqual,
                TokenType.GreaterThan => CBinaryOperator.GreaterThan,
                TokenType.GreaterThanEqual => CBinaryOperator.GreaterThanEqual,
                TokenType.LessThan => CBinaryOperator.LessThan,
                TokenType.LessThanEqual => CBinaryOperator.LessThanEqual,
                TokenType.Mod => CBinaryOperator.Mod,
                _ => throw new ArgumentException("Cannot convert operator")
            };
        }

        private static CUnaryOperator ConvertUnaryOperator(TokenType type)
        {
            return type switch
            {
                TokenType.Minus => CUnaryOperator.Negate,
                TokenType.Bang => CUnaryOperator.Bang,
                _ => throw new ArgumentException("Cannot convert operator")
            };
        }
    }
}
