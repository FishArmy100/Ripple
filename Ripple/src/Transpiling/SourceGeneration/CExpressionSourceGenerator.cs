using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Transpiling.C_AST;
using Raucse;
using Raucse.Extensions;

namespace Ripple.Transpiling.SourceGeneration
{
	static class CExpressionSourceGenerator
	{
		public static string GenerateSource(CExpression expression)
		{
			return expression.Accept(new CExpressionSourceGeneratorVisitor());
		}

		private class CExpressionSourceGeneratorVisitor : ICExpressionVisitor<string>
		{
			public string VisitCBinary(CBinary binary)
			{
				string code = "";
				if (binary.Left is CBinary bl)
					code += GroupIfTrue(binary.Left, BinOpPriority(bl.Op) < BinOpPriority(binary.Op));
				else if (binary.Left is CCast)
					code += Group(binary.Left.Accept(this));
				else
					code += binary.Left.Accept(this);

				code += $" {binary.Op.ToCCode()} ";

				if (binary.Right is CBinary br)
					code += GroupIfTrue(binary.Right, BinOpPriority(br.Op) < BinOpPriority(binary.Op));
				else
					code += binary.Right.Accept(this);

				return code;
			}

			public string VisitCCall(CCall call)
			{
				string args = string.Join(", ", call.Arguments.Select(a => a.Accept(this)));
				string callee = GroupIfTrue(call.Callee, call.Callee is CBinary || call.Callee is CCast || call.Callee is CUnary);
				return $"{callee}({args})";
			}

			public string VisitCCast(CCast cast)
			{
				string typeName = CTypeSourceGenerator.GenerateSource(cast.Type, new Option<string>());
				string arg = GroupIfTrue(cast.Castee, cast.Castee is CBinary);
				return $"({typeName}){arg}";
			}

			public string VisitCIdentifier(CIdentifier identifier)
			{
				return identifier.Id;
			}

			public string VisitCIndex(CIndex index)
			{
				string arg = index.Argument.Accept(this);
				string callee = GroupIfTrue(index.Indexee, index.Indexee is CBinary || index.Indexee is CCast || index.Indexee is CUnary);
				return $"{callee}[{arg}]";
			}
			public string VisitCMemberAccess(CMemberAccess cMemberAccess)
			{
				string expr = cMemberAccess.Expression.Accept(this);
				return $"{expr}.{cMemberAccess.Identifier}";
			}

			public string VisitCLiteral(CLiteral literal)
			{
				return literal.Type switch
				{
					CLiteralType.String => $"\"{(string)literal.Value}\"",
					CLiteralType.Intager => $"{(int)literal.Value}",
					CLiteralType.Charactor => $"\'{(char)literal.Value}\'",
					CLiteralType.Float => $"{(int)literal.Value}",
					CLiteralType.True => CKeywords.TRUE,
					CLiteralType.False => CKeywords.FALSE,
					_ => throw new ArgumentException("Unknown literal type " + literal.Type),
				};
			}

			public string VisitCSizeOf(CSizeOf sizeOf)
			{
				string typeName = CTypeSourceGenerator.GenerateSource(sizeOf.Type, new Option<string>());
				return CKeywords.SIZEOF + Group(typeName);
			}

			public string VisitCUnary(CUnary unary)
			{
				string op = unary.Op.ToCCode();
				string operand = GroupIfTrue(unary.Expression, unary.Expression is CBinary || unary.Expression is CCast);
				return op + operand;
			}

			public string VisitCCompoundLiteral(CCompoundLiteral cCompoundLiteral)
			{
				return $"({CTypeSourceGenerator.GenerateSource(cCompoundLiteral.Type, new Option<string>())}){cCompoundLiteral.Initalizer.Accept(this)}";
			}

			public string VisitCInitalizerList(CInitalizerList cInitalizerList)
			{
				return "{" + cInitalizerList.Expressions.Select(e => e.Accept(this)).Concat(", ") + "}";
			}

			private static string Group(string grouped)
			{
				return "(" + grouped + ")";
			}

			private string Group(CExpression expression)
			{
				return Group(expression.Accept(this));
			}

			private string GroupIfTrue(CExpression expression, bool condition)
			{
				if(condition)
				{
					return Group(expression);
				}
				else
				{
					return expression.Accept(this);
				}
			}

			private static int BinOpPriority(CBinaryOperator op)
			{
				return op switch
				{
					CBinaryOperator.Plus => 5,
					CBinaryOperator.Minus => 5,
					CBinaryOperator.Star => 6,
					CBinaryOperator.Divide => 6,
					CBinaryOperator.Assign => 0,
					CBinaryOperator.Mod => 6,
					CBinaryOperator.EqualEqual => 3,
					CBinaryOperator.BangEqual => 3,
					CBinaryOperator.GreaterThan => 4,
					CBinaryOperator.LessThan => 4,
					CBinaryOperator.GreaterThanEqual => 4,
					CBinaryOperator.LessThanEqual => 4,
					CBinaryOperator.AnpersandAnpersand => 2,
					CBinaryOperator.PipePipe => 1,
					_ => throw new ArgumentException("Unknown C binary operator " + op),
				};
			}
		}
	}
}
