using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Raucse.Extensions;
using Raucse.Extensions.Nullables;

namespace Ripple.AST.Utils
{
    public class AstPrinter : IStatementVisitor, IExpressionVisitor
    {
        private int m_Index = 0;
        private readonly string m_Seperator;
        private readonly Action<string> PrinterFunc;

        public AstPrinter(string seperator)
        {
            m_Seperator = seperator;
            PrinterFunc = text => Console.Write(text);
        }

        public AstPrinter(string seperator, Action<string> printerFunc) : this(seperator)
        {
            PrinterFunc = printerFunc;
        }

        public void PrintAst(Expression expr)
        {
            expr.Accept(this);
            m_Index = 0;
        }

        public void PrintAst(Statement statement)
        {
            statement.Accept(this);
            m_Index = 0;
        }

        public void VisitExprStmt(ExprStmt exprStmt)
        {
            PrintLine("Expression Statement: ");
            TabRight();
            exprStmt.Expr.Accept(this);
            TabLeft();
        }

        public void VisitBlockStmt(BlockStmt blockStmt)
        {
            PrintLine("Block Statement:");
            TabRight();
            foreach (Statement statement in blockStmt.Statements)
                statement.Accept(this);
            TabLeft();
        }

        public void VisitIfStmt(IfStmt ifStmt)
        {
            PrintLine("If Statement");
            TabRight();

            PrintLine("Condition:");
            TabRight();
            ifStmt.Expr.Accept(this);
            TabLeft();

            PrintLine("Main Branch:");
            TabRight();
            ifStmt.Body.Accept(this);
            TabLeft();

            if(ifStmt.ElseToken.HasValue)
            {
                PrintLine("Else Branch");
                TabRight();
                ifStmt.ElseBody.Match(body => body.Accept(this));
                TabLeft();
            }

            TabLeft();
        }

        public void VisitForStmt(ForStmt forStmt)
        {
            PrintLine("For Statement");
            TabRight();

            if(forStmt.Init.HasValue())
            {
                PrintLine("Initalization:");
                TabRight();
                forStmt.Init.Value.Accept(this);
                TabLeft();
            }

            if(forStmt.Condition.HasValue())
            {
                PrintLine("Condition:");
                TabRight();
                forStmt.Condition.Value.Accept(this);
                TabLeft();
            }

            if(forStmt.Iter.HasValue())
            {
                PrintLine("Iterator:");
                TabRight();
                forStmt.Iter.Value.Accept(this);
                TabLeft();
            }

            PrintLine("Body:");
            TabRight();
            forStmt.Body.Accept(this);
            TabLeft();

            TabLeft();
        }

        public void VisitVarDecl(VarDecl varDecl)
        {
            if (varDecl.UnsafeToken.HasValue)
                PrintLine("Unsafe Variable Declaration:");
            else
                PrintLine("Variable Declaration:");

            TabRight();
            PrintLine("Type: " + TypeNamePrinter.PrintType(varDecl.Type));

            string names = "";
            for(int i = 0; i < varDecl.VarNames.Count; i++)
            {
                if (i != 0)
                    names += ", ";
                names += varDecl.VarNames[i];
            }

            PrintLine("Var Names: " + names);

            varDecl.Expr.Match(ok =>
            {
                PrintLine("Initializer: ");
                TabRight();
                ok.Accept(this);
                TabLeft();
            });

            TabLeft();
        }

        public void VisitReturnStmt(ReturnStmt returnStmt)
        {
            PrintLine("Return Statement:");
            if(returnStmt.Expr.HasValue())
            {
                TabRight();
                returnStmt.Expr.Value.Accept(this);
                TabLeft();
            }
        }

        public void VisitParameters(Parameters parameters)
        {
            PrintLine("Parameters: ");
            TabRight();
            foreach (var pair in parameters.ParamList)
                PrintLine(TypeNamePrinter.PrintType(pair.First) + " " + pair.Second.Text);
            TabLeft();
        }

        public void VisitFuncDecl(FuncDecl funcDecl)
        {
            if (funcDecl.UnsafeToken.HasValue)
                PrintLine("Unsafe Function Declaration:");
            else
                PrintLine("Function Declaration:");

            TabRight();
            funcDecl.Param.Accept(this);
            PrintLine("Return type: " + TypeNamePrinter.PrintType(funcDecl.ReturnType));
            funcDecl.WhereClause.Match(w => w.Accept(this));
            funcDecl.Body.Accept(this);
            TabLeft();
        }

        public void VisitFileStmt(FileStmt fileStmt)
        {
            PrintLine("File: " + System.IO.Path.GetFileName(fileStmt.RelativePath));
            TabRight();
            foreach (Statement statement in fileStmt.Statements)
                statement.Accept(this);
            TabLeft();
        }

        public void VisitWhileStmt(WhileStmt whileStmt)
        {
            PrintLine("While Loop:");
            TabRight();

            PrintLine("Condition:");
            TabRight();
            whileStmt.Condition.Accept(this);
            TabLeft();

            PrintLine("Body:");
            TabRight();
            whileStmt.Body.Accept(this);
            TabLeft();

            TabLeft();
        }

        public void VisitContinueStmt(ContinueStmt continueStmt)
        {
            PrintLine("Continue Statement");
        }

        public void VisitBreakStmt(BreakStmt breakStmt)
        {
            PrintLine("Break Statement");
        }

        public void VisitExternalFuncDecl(ExternalFuncDecl externalFuncDecl)
        {
            PrintLine("External Function Declaration:");
            TabRight();
            externalFuncDecl.Parameters.Accept(this);
            PrintLine("Return type: " + TypeNamePrinter.PrintType(externalFuncDecl.ReturnType));
            TabLeft();
        }

        public void VisitGenericParameters(GenericParameters genericParameters)
        {
            PrintLine("Generic Parameters:");
            TabRight();
            foreach (Token t in genericParameters.Lifetimes)
                PrintLine(t.Text);
            TabLeft();
        }

        public void VisitWhereClause(WhereClause whereClause)
        {
            PrintLine("Where: ");
            TabRight();
            whereClause.Expression.Accept(this);
            TabLeft();
        }

        public void VisitUnsafeBlock(UnsafeBlock unsafeBlock)
        {
            PrintLine("Unsafe Block:");
            TabRight();
            foreach (Statement statement in unsafeBlock.Statements)
                statement.Accept(this);
            TabLeft();
        }

        public void VisitConstructorDecl(ConstructorDecl constructorDecl)
        {
            if(constructorDecl.UnsafeToken is Token)
            {
                PrintLine("Unsafe Constructor Declaration:");
            }
            else
            {
                PrintLine("Constructor Declaration:");
            }
            TabRight();
            constructorDecl.Parameters.Accept(this);
            constructorDecl.Body.Accept(this);
            TabLeft();
        }

        public void VisitDestructorDecl(DestructorDecl destructorDecl)
        {
            if (destructorDecl.UnsafeToken is Token)
            {
                PrintLine("Unsafe Destructor Declaration:");
            }
            else
            {
                PrintLine("Destructor Declaration:");
            }

            destructorDecl.Body.Accept(this);
        }

        public void VisitThisFunctionParameter(ThisFunctionParameter thisFunctionParameter)
        {
            string str = "this";
            str += thisFunctionParameter.MutToken.Match(ok => " " + ok.Text, () => "");
            str += thisFunctionParameter.RefToken.Match(ok => ok.Text, () => "");
            str += thisFunctionParameter.LifetimeToken.Match(ok => ok.Text, () => "");
            PrintLine(str);
        }

        public void VisitMemberFunctionParameters(MemberFunctionParameters memberFunctionParameters)
        {
            PrintLine("Member Function Parameters:");
            TabRight();
            memberFunctionParameters.ThisParameter.Match(ok => ok.Accept(this));
            foreach(var (type, name) in memberFunctionParameters.ParamList)
            {
                 PrintLine(TypeNamePrinter.PrintType(type) + " " + name.Text);
            }
            TabLeft();
        }

        public void VisitMemberFunctionDecl(MemberFunctionDecl memberFunctionDecl)
        {
            PrintLine("Member Function Declaration:");
            TabRight();
            memberFunctionDecl.Parameters.Accept(this);
            memberFunctionDecl.Body.Accept(this);
            TabLeft();
        }

        public void VisitMemberDecl(MemberDecl memberDecl)
        {
            PrintLine("Member Decl:");
            TabRight();
            if(memberDecl.VisibilityToken is Token visibility)
                PrintLine($"Visibility: {visibility}");

            memberDecl.Declaration.Accept(this);
            TabLeft();
        }

        public void VisitClassDecl(ClassDecl classDecl)
        {
            PrintLine("Class Declaration:");
            TabRight();
            foreach (var member in classDecl.Members)
                member.Accept(this);
            TabLeft();
        }

        public void VisitExternClassMemberDecl(ExternClassMemberDecl externClassMemberDecl)
        {
            PrintLine($"{TypeNamePrinter.PrintType(externClassMemberDecl.Type)} {externClassMemberDecl.Name.Text}");
        }

        public void VisitExternClassDecl(ExternClassDecl externClassDecl)
        {
            PrintLine($"External Class Declaration: {externClassDecl.Name.Text}");
            TabRight();
            externClassDecl.Members.ForEach(m => m.Accept(this));
            TabLeft();
        }

        public void VisitProgramStmt(ProgramStmt program)
        {
            PrintLine("Program:");
            TabRight();
            foreach (FileStmt file in program.Files)
                file.Accept(this);
            TabLeft();
        }

        public void VisitBinary(Binary binary)
        {
            PrintLine("Binary: " + binary.Op.Text);
            TabRight();
            binary.Left.Accept(this);
            binary.Right.Accept(this);
            TabLeft();
        }

        public void VisitCall(Call call)
        {
            PrintLine("Call:");
            TabRight();
            PrintLine("Callee:");

            TabRight();
            call.Callee.Accept(this);
            TabLeft();

            PrintLine("Arguments:");
            TabRight();
            foreach (Expression expr in call.Args)
                expr.Accept(this);
            TabLeft();
            TabLeft();
        }

        public void VisitSizeOf(SizeOf sizeOf)
        {
            PrintLine("SizeOf: " + TypeNamePrinter.PrintType(sizeOf.Type));
        }

        public void VisitGrouping(Grouping grouping)
        {
            PrintLine("Grouping");
            TabRight();
            grouping.Expr.Accept(this);
            TabLeft();
        }

        public void VisitLiteral(Literal literal)
        {
            PrintLine("Literal: " + literal.Val.Text);
        }

        public void VisitUnary(Unary unary)
        {
            PrintLine("Unary: " + unary.Op.Text);
            TabRight();
            unary.Expr.Accept(this);
            TabLeft();
        }

        public void VisitIdentifier(Identifier variable)
        {
            PrintLine("Identifier: " + variable.Name.Text);
        }

        public void VisitIndex(Index index)
        {
            PrintLine("Index:");
            TabRight();
            PrintLine("Indexed:");

            TabRight();
            index.Indexed.Accept(this);
            TabLeft();

            PrintLine("Argument:");
            TabRight();
            index.Argument.Accept(this);
            TabLeft();
            TabLeft();
        }

        public void VisitCast(Cast cast)
        {
            PrintLine("Cast: " + TypeNamePrinter.PrintType(cast.TypeToCastTo));
            TabRight();
            cast.Castee.Accept(this);
            TabLeft();
        }

        public void VisitInitializerList(InitializerList initializerList)
        {
            PrintLine("Initializer List:");
            TabRight();
            foreach (Expression expression in initializerList.Expressions)
                expression.Accept(this);
            TabLeft();
        }

        public void VisitMemberAccess(MemberAccess memberAccess)
        {
            PrintLine("Member Access:");
            TabRight();
            memberAccess.Expression.Accept(this);
            PrintLine($"Member: {memberAccess.MemberName.Text}");
            TabLeft();
        }

        private void TabRight() { m_Index++; }
        private void TabLeft() { m_Index--; }

        private void PrintLine(string text) { PrinterFunc(GetOffset() + text + "\n"); }

        private string GetOffset()
        {
            string s = "";
            for(int i = 0; i < m_Index; i++)
                s += m_Seperator;

            return s;
        }
    }
}
