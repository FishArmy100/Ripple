using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Validation.Info.Expressions;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info.Statements;
using Ripple.Transpiling.C_AST;
using Ripple.Transpiling.SourceGeneration;
using Ripple.Utils;
using Ripple.Utils.Extensions;
using Ripple.Lexing;

namespace Ripple.Transpiling.ASTConversion
{
    class ExpressionConverterVisitor : ITypedExpressionVisitor<ExpressionConversionResult>
    {
        private readonly TypeConverterVisitor m_TypeConverterVisitor;
        public string VariableSufix { get; set; }

        public ExpressionConverterVisitor(CArrayRegistry registry, string variableSufix)
        {
            m_TypeConverterVisitor = new TypeConverterVisitor(registry);
            VariableSufix = variableSufix;
        }

        public ExpressionConversionResult VisitTypedBinary(TypedBinary typedBinary)
        {
            CBinaryOperator op = ASTConversionUtils.ToBinary(typedBinary.Op);
            var left = typedBinary.Left.Accept(this);
            var right = typedBinary.Right.Accept(this);

            CType returned = GetReturned(typedBinary);
            CBinary binary = new CBinary(left.Expression, op, right.Expression);
            List<CVarDecl> varDecls = new List<CVarDecl>(left.GeneratedVariables.Concat(right.GeneratedVariables));

            return new ExpressionConversionResult(varDecls, binary, returned, ExpressionValueType.Temp);
        }

        public ExpressionConversionResult VisitTypedCall(TypedCall typedCall)
        {
            var callee = typedCall.Callee.Accept(this);
            var args = typedCall.Arguments.Select(a => a.Accept(this));

            List<CVarDecl> varDecls = callee.GeneratedVariables.ToList();
            varDecls.AddRange(args.Select(a => a.GeneratedVariables).SelectMany(v => v));

            CCall cCall = new CCall(callee.Expression, args.Select(a => a.Expression).ToList());
            CType returned = GetReturned(typedCall);
            return new ExpressionConversionResult(varDecls, cCall, returned, ExpressionValueType.Temp);
        }

        public ExpressionConversionResult VisitTypedCast(TypedCast typedCast)
        {
            var castee = typedCast.Casted.Accept(this);
            CType type = ConvertType(typedCast.TypeToCastTo);
            CType returned = GetReturned(typedCast);
            CCast cCast = new CCast(castee.Expression, type);

            return new ExpressionConversionResult(castee.GeneratedVariables, cCast, returned, ExpressionValueType.Temp);
        }

        public ExpressionConversionResult VisitTypedIdentifier(TypedIdentifier typedIdentifier)
        {
            CIdentifier cIdentifier = new CIdentifier(typedIdentifier.Name);
            CType returned = GetReturned(typedIdentifier);
            return new ExpressionConversionResult(new List<CVarDecl>(), cIdentifier, returned, ExpressionValueType.Var);
        }

        public ExpressionConversionResult VisitTypedIndex(TypedIndex typedIndex)
        {
            var indexee = typedIndex.Indexee.Accept(this);
            var arg = typedIndex.Argument.Accept(this);

            CType returned = GetReturned(typedIndex);

            CIndex cIndex = typedIndex.Indexee.Returned switch
            {
                ArrayInfo => new CIndex(new CMemberAccess(indexee.Expression, CKeywords.ARRAY_DATA_NAME), arg.Expression),
                _ => new CIndex(indexee.Expression, arg.Expression)
            };

            List<CVarDecl> varDecls = new List<CVarDecl>(indexee.GeneratedVariables.Concat(arg.GeneratedVariables));

            return new ExpressionConversionResult(varDecls, cIndex, returned, ExpressionValueType.Var);
        }

        public ExpressionConversionResult VisitTypedInitalizerList(TypedInitalizerList typedInitalizerList)
        {
            var expressions = typedInitalizerList.Expressions.Select(e => e.Accept(this));

            List<CVarDecl> generatedVariables = expressions
                .Select(e => e.GeneratedVariables)
                .SelectMany(v => v)
                .ToList();

            CType returned = GetReturned(typedInitalizerList);
            if(returned is CBasicType b && b.IsStruct)
			{

                CInitalizerList cInitalizerList = new CInitalizerList(expressions.Select(e => e.Expression).ToList());
                CCompoundLiteral compoundLiteral = new CCompoundLiteral(b, cInitalizerList);
                return new ExpressionConversionResult(generatedVariables, compoundLiteral, returned, ExpressionValueType.Temp);
            }
			else
			{
                throw new NotImplementedException();
			}
        }

        public ExpressionConversionResult VisitTypedLiteral(TypedLiteral typedLiteral)
        {
            CLiteral literal = ASTConversionUtils.ToLiteral(typedLiteral);
            CType returned = GetReturned(typedLiteral);
            return new ExpressionConversionResult(new List<CVarDecl>(), literal, returned, ExpressionValueType.Temp);
        }

        public ExpressionConversionResult VisitTypedSizeOf(TypedSizeOf typedSizeOf)
        {
            CType type = ConvertType(typedSizeOf.SizedType);
            CType returned = GetReturned(typedSizeOf);
            CSizeOf cSizeOf = new CSizeOf(type);
            return new ExpressionConversionResult(new List<CVarDecl>(), cSizeOf, returned, ExpressionValueType.Temp);
        }

        public ExpressionConversionResult VisitTypedUnary(TypedUnary typedUnary)
        {
            var operand = typedUnary.Operand.Accept(this);
            CUnaryOperator op = ASTConversionUtils.ToUnary(typedUnary.Op);
            List<CVarDecl> varDecls = new List<CVarDecl>(operand.GeneratedVariables);

            CType returned = GetReturned(typedUnary);

            if (typedUnary.Op.IsType(TokenType.Ampersand, TokenType.RefMut) && operand.ValueType == ExpressionValueType.Temp)
            {
                string varName = $"{CKeywords.TEMP_VAR_PREFIX}_{varDecls.Count}_{VariableSufix}";
                CType type = operand.ExpressionType;
                CVarDecl cVarDecl = new CVarDecl(type, varName, operand.Expression);
                varDecls.Add(cVarDecl);

                CUnary premotedUnary = new CUnary(new CIdentifier(varName), op);
                return new ExpressionConversionResult(varDecls, premotedUnary, returned, ExpressionValueType.Temp);
            }

            CUnary unary = new CUnary(operand.Expression, op);
            return new ExpressionConversionResult(varDecls, unary, returned, ExpressionValueType.Temp);
        }

        private CType GetReturned(TypedExpression expression)
        {
            return ConvertType(expression.Returned);
        }

        private CType ConvertType(TypeInfo type)
        {
            return type.Accept(m_TypeConverterVisitor);
        }
    }
}
