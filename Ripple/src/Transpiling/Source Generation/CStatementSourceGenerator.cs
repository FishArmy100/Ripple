using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Transpiling.C_AST;

namespace Ripple.Transpiling.Source_Generation
{
	static class CStatementSourceGenerator
	{
		public static string GenerateSource(CStatement statement)
		{
			CSourceBuilder builder = new CSourceBuilder();
			CStatementSourceGeneratorVisitor visitor = new CStatementSourceGeneratorVisitor();
			statement.Accept(visitor, builder);
			return builder.ToString();
		}

		private class CStatementSourceGeneratorVisitor : ICStatementVisitorWithArg<CSourceBuilder>
		{
			public void VisitCBlockStmt(CBlockStmt blockStmt, CSourceBuilder builder)
			{
				builder.BeginBlock();
				foreach (CStatement statement in blockStmt.Statements)
					statement.Accept(this, builder);
				builder.EndBlock();
			}

			public void VisitCBreakStmt(CBreakStmt breakStmt, CSourceBuilder builder)
			{
				builder.AppendLine($"{CKeywords.BREAK};");
			}

			public void VisitCContinueStmt(CContinueStmt continueStmt, CSourceBuilder builder)
			{
				builder.AppendLine($"{CKeywords.CONTINUE};");
			}

			public void VisitCExprStmt(CExprStmt exprStmt, CSourceBuilder builder)
			{
				string expression = CExpressionSourceGenerator.GenerateSource(exprStmt.Expression);
				builder.AppendLine($"{expression};");
			}

			public void VisitCFileStmt(CFileStmt fileStmt, CSourceBuilder builder)
			{
				foreach (CIncludeStmt include in fileStmt.Includes)
					include.Accept(this, builder);

				builder.AppendLine();

				foreach (CStatement statement in fileStmt.Statements)
					statement.Accept(this, builder);
			}

			public void VisitCForStmt(CForStmt forStmt, CSourceBuilder builder)
			{
				string src = $"{CKeywords.FOR}(";
				src += forStmt.Initalizer.Match(ok => InlineVarDecl(ok), () => ";");
				src += forStmt.Condition.Match(ok => CExpressionSourceGenerator.GenerateSource(ok), () => "") + ";";
				src += forStmt.Iterator.Match(ok => CExpressionSourceGenerator.GenerateSource(ok), () => "");
				src += ")";

				builder.AppendLine(src);
				TabIfNotBlock(forStmt.Body, builder);
			}

			public void VisitCFuncDecl(CFuncDecl funcDecl, CSourceBuilder builder)
			{
				string b = $"{funcDecl.Name}{GenerateFunctionParameters(funcDecl.Parameters)}";
				string f = CTypeSourceGenerator.GenerateSource(funcDecl.Returned, b);
				builder.AppendLine(f + ";");
			}

			public void VisitCFuncDef(CFuncDef funcDef, CSourceBuilder builder)
			{
				string b = $"{funcDef.Name}{GenerateFunctionParameters(funcDef.Parameters)}";
				string f = CTypeSourceGenerator.GenerateSource(funcDef.Returned, b);
				builder.AppendLine(f);
				funcDef.Body.Accept(this, builder);
			}

			public void VisitCIfStmt(CIfStmt ifStmt, CSourceBuilder builder)
			{
				builder.AppendLine($"{CKeywords.IF}({CExpressionSourceGenerator.GenerateSource(ifStmt.Condition)})");
				TabIfNotBlock(ifStmt.Body, builder);
			}

			public void VisitCIncludeStmt(CIncludeStmt include, CSourceBuilder builder)
			{
				builder.AppendLine($"{CKeywords.HASH_INCLUDE} \"{include.File}\"");
			}

			public void VisitCReturnStmt(CReturnStmt returnStmt, CSourceBuilder builder)
			{
				string expression = returnStmt.Expression.Match(ok => CExpressionSourceGenerator.GenerateSource(ok), () => "");
				builder.AppendLine($"{CKeywords.RETURN} {expression};");
			}
			public void VisitCStructDef(CStructDef cStructDef, CSourceBuilder builder)
			{
				builder.AppendLine($"{CKeywords.STRUCT} {cStructDef.Name}");
				builder.BeginBlock();
				foreach (CStructMember member in cStructDef.Members)
					member.Accept(this, builder);
				builder.EndBlock();
			}

			public void VisitCStructDecl(CStructDecl structDecl, CSourceBuilder builder)
			{
				builder.AppendLine($"{CKeywords.STRUCT} {structDecl.Name}");
			}

			public void VisitCStructMember(CStructMember structMember, CSourceBuilder builder)
			{
				string member = CTypeSourceGenerator.GenerateSource(structMember.Type, structMember.Name);
				string initializer = structMember.Initalizer.Match(ok => " = " + CExpressionSourceGenerator.GenerateSource(ok), () => "");
				builder.AppendLine($"{member}{initializer};");
			}

			public void VisitCTypeDefStmt(CTypeDefStmt typeDefStmt, CSourceBuilder builder)
			{
				builder.AppendLine($"{CKeywords.TYPEDEF} {CTypeSourceGenerator.GenerateSource(typeDefStmt.Type, typeDefStmt.Name)};");
			}

			public void VisitCVarDecl(CVarDecl varDecl, CSourceBuilder builder)
			{
				builder.AppendLine(InlineVarDecl(varDecl));
			}

			public void VisitCWhileStmt(CWhileStmt whileStmt, CSourceBuilder builder)
			{
				builder.AppendLine($"{CKeywords.WHILE}({CExpressionSourceGenerator.GenerateSource(whileStmt.Condition)})");
				TabIfNotBlock(whileStmt.Body, builder);
			}

			private static string GenerateFunctionParameters(List<CFuncParam> parameters)
			{
				return "(" + string.Join(", ", parameters
					.Select(p => CTypeSourceGenerator.GenerateSource(p.Type, p.Name))) + ")";
			}

			private static string InlineVarDecl(CVarDecl varDecl)
			{
				string type = CTypeSourceGenerator.GenerateSource(varDecl.Type, varDecl.Name);
				string expression = varDecl.Initializer.Match(ok => CExpressionSourceGenerator.GenerateSource(ok), () => "");
				return $"{type} = {expression};";
			}

			private void TabIfNotBlock(CStatement statement, CSourceBuilder builder)
			{
				if (statement is CBlockStmt)
				{
					statement.Accept(this, builder);
				}
				else
				{
					builder.TabRight();
					statement.Accept(this, builder);
					builder.TabLeft();
				}
			}
		}
	}
}
