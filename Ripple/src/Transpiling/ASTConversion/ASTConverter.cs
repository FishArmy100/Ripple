using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Transpiling.C_AST;
using Ripple.AST;
using Ripple.Transpiling.ASTConversion.SimplifiedTypes;
using Ripple.Utils;
using Ripple.Transpiling.SourceGeneration;

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
			return simplified.Accept(new SimplifiedTypeToCTypeConverterVisitor(registry));
		}

		public static CType ConvertToCType(SimplifiedType type, ref CArrayRegistry registry)
        {
			return type.Accept(new SimplifiedTypeToCTypeConverterVisitor(registry));
        }

		public static Pair<CExpression, List<CVarDecl>> ConvertToCExpression(Expression expression, string varPrefix, ref CArrayRegistry registry)
		{
			 
		}

		private class ExpressionToCExpressionConverterVisitor : IExpressionVisitor<Pair<ReturnedValue, CExpression>>
		{
			public readonly List<CVarDecl> PromotedTempararyVariables = new List<CVarDecl>();
			public readonly ListStack<KeyValuePair<string, SimplifiedType>> m_Variables;
			public readonly Dictionary<string, SimplifiedFunctionInfo> m_Functions;
			private CArrayRegistry m_Regsitry;

            public ExpressionToCExpressionConverterVisitor(ListStack<KeyValuePair<string, SimplifiedType>> variables, Dictionary<string, SimplifiedFunctionInfo> functions, CArrayRegistry regsitry)
            {
                m_Variables = variables;
                m_Functions = functions;
                m_Regsitry = regsitry;
            }

            public Pair<ReturnedValue, CExpression> VisitBinary(Binary binary)
            {
				var left = binary.Left.Accept(this);
				var right = binary.Right.Accept(this);
				CBinaryOperator op = RippleToCConversionUtils.ToBinary(binary.Op.Type);
				CBinary cBinary = new CBinary(left.Second, op, right.Second);

				if (left.First.Type is SPointer ptr1)
                {
					ReturnedValue value = new ReturnedValue(ValueType.Temp, ptr1);
					return new Pair<ReturnedValue, CExpression>(value, cBinary);
                }
				else if(right.First.Type is SPointer ptr2)
                {
					ReturnedValue value = new ReturnedValue(ValueType.Temp, ptr2);
					return new Pair<ReturnedValue, CExpression>(value, cBinary);
				}

				ReturnedValue returned = new ReturnedValue(ValueType.Temp, left.First.Type);
				return new Pair<ReturnedValue, CExpression>(returned, cBinary);
			}

            public Pair<ReturnedValue, CExpression> VisitCall(Call call)
            {
				var callee = call.Callee.Accept(this);
				if(callee.First.Type is SFuncPtr ptr)
                {
					var args = call.Args.Select(a => a.Accept(this));
					CCall cCall = new CCall(callee.Second, args.Select(a => a.Second).ToList());
					ReturnedValue returned = new ReturnedValue(ValueType.Temp, ptr.Returned);
					return new Pair<ReturnedValue, CExpression>(returned, cCall);
                }

				throw new TranspilingException("Called type is not of type function pointer");
            }

            public Pair<ReturnedValue, CExpression> VisitCast(Cast cast)
            {
				var castee = cast.Castee.Accept(this);
				SimplifiedType type = SimplifiedTypeGenerator.Generate(cast.TypeToCastTo);
				CCast cCast = new CCast(castee.Second, ConvertToCType(type, ref m_Regsitry));
				ReturnedValue returned = new ReturnedValue(ValueType.Temp, type);
				return new Pair<ReturnedValue, CExpression>(returned, cCast);
            }

            public Pair<ReturnedValue, CExpression> VisitGrouping(Grouping grouping)
            {
				return grouping.Expr.Accept(this);
            }

            public Pair<ReturnedValue, CExpression> VisitIdentifier(Identifier identifier)
            {
				string idName = identifier.Name.Text;
                foreach(var (name, type) in m_Variables)
                {
					if(identifier.Name.Text == name)
                    {
						ReturnedValue value = new ReturnedValue(ValueType.Var, type);
						CIdentifier id = new CIdentifier(name);
						return new Pair<ReturnedValue, CExpression>(value, id);
                    }
                }

				if(m_Functions.TryGetValue())
            }

            public Pair<ReturnedValue, CExpression> VisitIndex(AST.Index index)
            {
                throw new NotImplementedException();
            }

            public Pair<ReturnedValue, CExpression> VisitInitializerList(InitializerList initializerList)
            {
                throw new NotImplementedException();
            }

            public Pair<ReturnedValue, CExpression> VisitLiteral(Literal literal)
            {
                throw new NotImplementedException();
            }

            public Pair<ReturnedValue, CExpression> VisitSizeOf(SizeOf sizeOf)
            {
                throw new NotImplementedException();
            }

            public Pair<ReturnedValue, CExpression> VisitTypeExpression(TypeExpression typeExpression)
            {
                throw new NotImplementedException();
            }

            public Pair<ReturnedValue, CExpression> VisitUnary(Unary unary)
            {
                throw new NotImplementedException();
            }

            private CVarDecl GenerateCVarDecl(int id, string prefix, CExpression initalizer)
			{
				string name = $"{prefix}_temp_{id}";
				return new CVarDecl(new CBasicType(CKeywords.AUTO, false), name, initalizer);
			}
		}

		private enum ValueType
        {
			Temp,
			Var
        }

		private readonly struct ReturnedValue
		{
			public readonly ValueType ValueType;
			public readonly SimplifiedType Type;

            public ReturnedValue(ValueType isTemparary, SimplifiedType type)
            {
                ValueType = isTemparary;
                Type = type;
            }
        }

		private class SimplifiedTypeToCTypeConverterVisitor : ISimplifiedTypeVisitor<CType>
		{
			private readonly CArrayRegistry m_Registry;

			public SimplifiedTypeToCTypeConverterVisitor(CArrayRegistry registry)
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
