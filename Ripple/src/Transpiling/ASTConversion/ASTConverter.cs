using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Transpiling.C_AST;
using Ripple.AST;
using Ripple.Transpiling.ASTConversion.SimplifiedTypes;
using Ripple.Utils;

namespace Ripple.Transpiling.ASTConversion
{
	static class ASTConverter
	{
		public static CProgram ConvertAST(ProgramStmt program)
		{
			return null;
		}

		public static CType ConvertToCType(TypeName name, ref CArrayRegistry registry)
		{
			SimplifiedType simplified = SimplifiedTypeGenerator.Generate(name);
			return simplified.Accept(new TypeNameToCTypeConverterVisitor(registry));
		}

		public static Pair<CExpression, List<CVarDecl>> ConvertToCExpression(Expression expression, string varPrefix, ref CArrayRegistry registry)
		{
			
		}

		private class ExpressionToCExpressionConverterVisitor : IExpressionVisitor<Pair<ReturnedValue, CExpression>>
		{
			public readonly List<CVarDecl> PromotedTempararyVariables = new List<CVarDecl>();
			private CArrayRegistry m_Regsitry;

			public ExpressionToCExpressionConverterVisitor(CArrayRegistry regsitry)
			{
				m_Regsitry = regsitry;
			}

			public Pair<ReturnedValue, CExpression> VisitBinary(Binary binary)
			{
				CExpression left = binary.Left.Accept(this).Second;
				CExpression right = binary.Right.Accept(this).Second;
				CBinaryOperator op = RippleToCConversionUtils.ToBinary(binary.Op.Type);
				return new Pair<ReturnedValue, CExpression>(new ReturnedValue(true), new CBinary(left, op, right));
			}

			public Pair<ReturnedValue, CExpression> VisitCall(Call call)
			{
				List<CExpression> args = call.Args.Select(a => a.Accept(this)).Select(a => a.Second).ToList();
				CExpression callee = call.Callee.Accept(this).Second;
				CCall ccall = new CCall(callee, args);
				return new Pair<ReturnedValue, CExpression>(new ReturnedValue(true), ccall);
			}

			public Pair<ReturnedValue, CExpression> VisitCast(Cast cast)
			{
				CType type = ConvertToCType(cast.TypeToCastTo, ref m_Regsitry);
				CExpression castee = cast.Castee.Accept(this).Second;
				CCast ccast = new CCast(castee, type);
				return new Pair<ReturnedValue, CExpression>(new ReturnedValue(true), ccast);
			}

			public Pair<ReturnedValue, CExpression> VisitGrouping(Grouping grouping)
			{
				return grouping.Expr.Accept(this);
			}

			public Pair<ReturnedValue, CExpression> VisitIdentifier(Identifier identifier)
			{
				CIdentifier id = new CIdentifier(identifier.Name.Text);
				return new Pair<ReturnedValue, CExpression>(new ReturnedValue(false), id);
			}

			public Pair<ReturnedValue, CExpression> VisitIndex(AST.Index index)
			{
				CExpression arg = index.Argument.Accept(this).Second;
				CExpression indexee = index.Indexed.Accept(this).Second;
				CIndex cIndex = new CIndex(indexee, arg);
				return new Pair<ReturnedValue, CExpression>(new ReturnedValue(false), cIndex);
			}

			public Pair<ReturnedValue, CExpression> VisitInitializerList(InitializerList initializerList)
			{
				throw new NotImplementedException();
			}

			public Pair<ReturnedValue, CExpression> VisitLiteral(Literal literal)
			{
				CLiteral cLiteral = RippleToCConversionUtils.ToLiteral(literal);
				return new Pair<ReturnedValue, CExpression>(new ReturnedValue(true), cLiteral);
			}

			public Pair<ReturnedValue, CExpression> VisitSizeOf(SizeOf sizeOf)
			{
				CType type = ConvertToCType(sizeOf.Type, ref m_Regsitry);
				CSizeOf cSizeOf = new CSizeOf(type);
				return new Pair<ReturnedValue, CExpression>(new ReturnedValue(true), cSizeOf);
			}

			public Pair<ReturnedValue, CExpression> VisitTypeExpression(TypeExpression typeExpression)
			{
				throw new NotImplementedException();
			}

			public Pair<ReturnedValue, CExpression> VisitUnary(Unary unary)
			{
				throw new NotImplementedException();
			}
		}

		private readonly struct ReturnedValue
		{
			public readonly bool IsTemparary;

			public ReturnedValue(bool isTemp)
			{
				IsTemparary = isTemp;
			}
		}

		private class TypeNameToCTypeConverterVisitor : ISimplifiedTypeVisitor<CType>
		{
			private readonly CArrayRegistry m_Registry;

			public TypeNameToCTypeConverterVisitor(CArrayRegistry registry)
			{
				m_Registry = registry;
			}

			public CType VisitSArray(SArray sArray)
			{
				return new CBasicType(m_Registry.GetArrayAlias(sArray).Name, false); 
			}

			public CType VisitSBasicType(SBasicType sBasicType)
			{
				return new CBasicType(sBasicType.Name, false);
			}

			public CType VisitSFuncPtr(SFuncPtr sFuncPtr)
			{
				List<CType> parameters = sFuncPtr.Parameters.Select(p => p.Accept(this)).ToList();
				CType returned = sFuncPtr.Returned.Accept(this);
				return new CFuncPtr(returned, parameters);
			}

			public CType VisitSPointer(SPointer sPointer)
			{
				return new CPointer(sPointer.Contained.Accept(this), false);
			}

			public CType VisitSReference(SReference sReference)
			{
				return new CPointer(sReference.Contained.Accept(this), false);
			}
		}
	}
}
