using System;
using System.Collections.Generic;
using System.Text;

namespace Ripple.AST
{
    class ASTPrinter : IASTVisitor<string>
    {
        private int m_IndentCount = 0;
        private readonly string m_IndentSeperator;

        public static string PrintTree(AbstractSyntaxTree ast, string seperator)
        {
            List<string> statements = ast.Accept(new ASTPrinter(seperator));
            string tree = "";
            foreach(string s in statements)
            {
                tree += s;
            }

            return tree;
        }

        public ASTPrinter(string seperator = "\t")
        {
            m_IndentSeperator = seperator;
            m_IndentCount = 0;
        }

        public string VisitAssignment(AssignmentExpr assignment)
        {
            string sExpr = GetOffset() + "Assignment Expression: Name " + assignment.Name.Lexeme;
            m_IndentCount++;
            sExpr += "\n" + assignment.Value.Accept(this);
            m_IndentCount--;
            return sExpr;
        }

        public string VisitBinary(BinaryExpr binary)
        {
            string sExpr = GetOffset() + "Binary Expression: operator " + binary.Operator.Lexeme;
            m_IndentCount++;
            sExpr += "\n" + binary.Left.Accept(this) + binary.Right.Accept(this);
            m_IndentCount--;
            return sExpr;
        }

        public string VisitGrouping(GroupingExpr grouping)
        {
            string sExpr = GetOffset() + "Grouped Expression: \n";
            m_IndentCount++;
            sExpr += grouping.GroupedExpression.Accept(this);
            m_IndentCount--;
            return sExpr;
        }

        public string VisitLiteral(LiteralExpr literal)
        {
            string sExpr = GetOffset() + "Literal: " + literal.Value.Lexeme + "\n";
            return sExpr;
        }

        public string VisitNew(NewExpr newExpr)
        {
            string sNew = GetOffset() + "New expression:\n";
            m_IndentCount++;
            sNew += GetOffset() + "Type: " + newExpr.Type.Accept(this) + "\n";
            sNew += GetOffset() + "Arguments: \n";

            m_IndentCount++;
            foreach (Expression expr in newExpr.Arguments)
                sNew += expr.Accept(this);
            m_IndentCount--;

            m_IndentCount--;
            return sNew;
        }

        public string VisitNewArray(NewArrayExpr newArrayExpr)
        {
            string sNewArray = GetOffset() + "New array expression:\n";
            m_IndentCount++;
            sNewArray += GetOffset() + "Type: " + newArrayExpr.ArrayType.Accept(this) + "\n";

            if(newArrayExpr.DefultValueExpr != null)
            {
                sNewArray += GetOffset() + "Defalut array value:\n";
                m_IndentCount++;
                sNewArray += newArrayExpr.DefultValueExpr.Accept(this);
                m_IndentCount--;
            }

            if (newArrayExpr.SizeExpr != null)
            {
                sNewArray += GetOffset() + "Array size:\n";
                m_IndentCount++;
                sNewArray += newArrayExpr.DefultValueExpr.Accept(this);
                m_IndentCount--;
            }

            if (newArrayExpr.InitializerArrayArgs.Count > 0)
            {
                sNewArray += GetOffset() + "Initializer arguments: \n";

                m_IndentCount++;
                foreach (Expression expr in newArrayExpr.InitializerArrayArgs)
                    sNewArray += expr.Accept(this);
                m_IndentCount--;
            }
            

            m_IndentCount--;
            return sNewArray;
        }

        public string VisitCast(CastExpr castExpr)
        {
            string sExpr = GetOffset() + "Casting expression: casting type: " + castExpr.Type.Accept(this);
            m_IndentCount++;
            sExpr += "\n" + castExpr.Right.Accept(this);
            m_IndentCount--;
            return sExpr;
        }

        public string VisitUnary(UnaryExpr unary)
        {
            string sExpr = GetOffset() + "Unary Expression: operator " + unary.Operator.Lexeme;
            m_IndentCount++;
            sExpr += "\n" + unary.Right.Accept(this);
            m_IndentCount--;
            return sExpr;
        }

        public string VisitIdentifier(IdentifierExpr variable)
        {
            string sExpr = GetOffset() + "Identifier: " + variable.Name.Lexeme + "\n";
            return sExpr;
        }

        public string VisitVarDeclaration(VariableDecl variable)
        {
            string sExpr = GetOffset() + "Variable declaration: " + variable.Type.Accept(this) + " " + variable.Name.Lexeme;
            
            if (variable.Initializer != null)
            {
                m_IndentCount++;
                sExpr += "\n" + variable.Initializer.Accept(this);
                m_IndentCount--;
            }
            
            return sExpr;
        }

        public string VisitFuncDeclaration(FunctionDecl funcDeclaration)
        {
            string sFunc = GetOffset() + "Func declaration: " + funcDeclaration.Name.Lexeme + "\n";
            m_IndentCount++;
            sFunc += GetOffset() + "Return type: " + funcDeclaration.ReturnType.Accept(this) + "\n";

            sFunc += GetOffset() + "Parameters: ";
            foreach (FunctionParameter parameter in funcDeclaration.Parameters)
                sFunc += "type: " + parameter.Type.Accept(this) + ", name: " + parameter.Name.Lexeme + "; ";
            sFunc = sFunc.Remove(sFunc.Length - 2);

            sFunc += "\n";

            sFunc += funcDeclaration.Body.Accept(this);

            m_IndentCount--;
            return sFunc;
        }

        public string VisitClassDeclaration(ClassDecl classDecl)
        {
            string sClass = GetOffset() + "Class declaration: Name:" + classDecl.Name.Lexeme;
            sClass += classDecl.IsDerived ? "; Base: " + classDecl.Base.Value.Lexeme + "\n" : "\n";

            m_IndentCount++;
            sClass += GetOffset() + "Is static: " + classDecl.IsStatic.ToString() + "\n";

            sClass += GetOffset() + "Members:\n";

            m_IndentCount++;
            foreach (MemberDeclarationStmt memberDeclaration in classDecl.MemberDeclarations)
                sClass += memberDeclaration.Accept(this);
            m_IndentCount--;

            m_IndentCount--;

            return sClass;
        }

        public string VisitConstructorDecl(ConstructorDecl constructorDecl)
        {
            string sConstructor = GetOffset() + "Constructor declaration:\n";
            m_IndentCount++;

            sConstructor += GetOffset() + "Parameters: ";
            foreach (FunctionParameter parameter in constructorDecl.Parameters)
                sConstructor += "type: " + parameter.Type.Accept(this) + ", name: " + parameter.Name.Lexeme + "; ";
            sConstructor = sConstructor.Remove(sConstructor.Length - 2);

            sConstructor += "\n";

            sConstructor += constructorDecl.Body.Accept(this);

            m_IndentCount--;
            return sConstructor;
        }

        public string VisitMemberDeclaration(MemberDeclarationStmt memberDeclaration)
        {
            string sMember = GetOffset() + "Class member declaration:\n";

            m_IndentCount++;
            sMember += GetOffset() + "Attributes: ";

            foreach (var attrib in memberDeclaration.Attributes)
                sMember += attrib.Type.ToString() + ", ";
            sMember = sMember.Remove(sMember.Length - 2);
            sMember += '\n';

            sMember += GetOffset() + "Declaration: \n";
            m_IndentCount++;
            sMember += memberDeclaration.Member.Accept(this);
            m_IndentCount--;

            m_IndentCount--;

            return sMember;
        }

        public string VisitExpressionStmt(ExpressionStmt expressionStmt)
        {
            string sExpr = GetOffset() + "Expression Statement: ";
            m_IndentCount++;
            sExpr += "\n" + expressionStmt.Expr.Accept(this);
            m_IndentCount--;
            return sExpr;
        }

        public string VisitBlock(BlockStmt block)
        {
            string sBlock = GetOffset() + "Block statement: \n";
            m_IndentCount++;
            foreach (Statement s in block.Statements)
                sBlock += s.Accept(this);
            m_IndentCount--;
            return sBlock;
        }

        public string VisitCall(CallExpr call)
        {
            string sCall = GetOffset() + "Call expression: \n";

            m_IndentCount++;
            sCall += GetOffset() + "Callee:\n";
            m_IndentCount++;
            sCall += call.Callee.Accept(this);
            m_IndentCount--;
            
            sCall += GetOffset() + "Parameters: \n";
            m_IndentCount++;
            foreach (Expression e in call.Parameters)
                sCall += e.Accept(this);
            m_IndentCount--;

            m_IndentCount--;

            return sCall;
        }

        public string VisitIfStmt(IfStmt ifStmt)
        {
            string sIf = GetOffset() + "If statement:\n";
            m_IndentCount++;

            sIf += GetOffset() + "Condition: \n";
            m_IndentCount++;
            sIf += ifStmt.Condition.Accept(this);
            m_IndentCount--;

            sIf += GetOffset() + "Then branch: \n";
            m_IndentCount++;
            sIf += ifStmt.ThenBranch.Accept(this);
            m_IndentCount--;


            if(ifStmt.ElseBranch != null)
            {
                sIf += GetOffset() + "Else branch: \n";
                m_IndentCount++;
                sIf += ifStmt.ElseBranch.Accept(this);
                m_IndentCount--;
            }
            m_IndentCount--;
            return sIf;
        }

        public string VisitWhileLoop(WhileLoopStmt whileLoop)
        {
            string sWhile = GetOffset() + "While loop:\n";
            m_IndentCount++;


            sWhile += GetOffset() + "Condition: \n";
            m_IndentCount++;
            sWhile += whileLoop.Condition.Accept(this);
            m_IndentCount--;


            sWhile += GetOffset() + "Body: \n";
            m_IndentCount++;
            sWhile += whileLoop.Body.Accept(this);
            m_IndentCount--;

            m_IndentCount--;
            return sWhile;
        }

        public string VisitContinueStmt(ContinueStmt continueStmt)
        {
            return GetOffset() + "Continue statement \n";
        }

        public string VisitBreakStmt(BreakStmt breakStmt)
        {
            return GetOffset() + "Break statement \n";
        }

        public string VisitReturnStmt(ReturnStmt returnStmt)
        {
            string sReturn = GetOffset() + "Return statment: \n";
            

            if(returnStmt.ReturnExpression != null)
            {
                m_IndentCount++;
                sReturn += GetOffset() + "Expression: \n";

                m_IndentCount++;
                sReturn += returnStmt.ReturnExpression.Accept(this);
                m_IndentCount--;

                m_IndentCount--;
            }
            return sReturn;
        }

        public string VisitForLoop(ForLoopStmt forLoop)
        {
            string sFor = GetOffset() + "For loop:\n";
            m_IndentCount++;
            if (forLoop.Initializer != null)
            {
                sFor += GetOffset() + "Initializer: \n";
                m_IndentCount++;
                sFor += forLoop.Initializer.Accept(this);
                m_IndentCount--;
            }

            if(forLoop.Condition != null)
            {
                sFor += GetOffset() + "Condition: \n";
                m_IndentCount++;
                sFor += forLoop.Condition.Accept(this);
                m_IndentCount--;
            }

            if (forLoop.Incrementer != null)
            {
                sFor += GetOffset() + "Incrementer: \n";
                m_IndentCount++;
                sFor += forLoop.Incrementer.Accept(this);
                m_IndentCount--;
            }

            sFor += GetOffset() + "Body: \n";
            m_IndentCount++;
            sFor += forLoop.Body.Accept(this);
            m_IndentCount--;

            m_IndentCount--;
            return sFor;
        }

        public string VisitGet(GetExpr get)
        {
            string sGet = GetOffset() + "Get: Name:" + get.Name.Lexeme + "\n";
            m_IndentCount++;
            sGet += GetOffset() + "Object:\n";
            m_IndentCount++;
            sGet += get.Object.Accept(this);
            m_IndentCount--;
            m_IndentCount--;
            return sGet;
        }

        public string VisitIndex(IndexExpr indexExpr)
        {
            string sIndex = GetOffset() + "Indexer: \n";
            m_IndentCount++;

            sIndex += GetOffset() + "Arguments:\n";
            m_IndentCount++;
            foreach (var arg in indexExpr.Arguments)
                sIndex += arg.Accept(this);
            m_IndentCount--;

            sIndex += GetOffset() + "Indexee:\n";
            m_IndentCount++;
            sIndex += indexExpr.Indexee.Accept(this);
            m_IndentCount--;

            m_IndentCount--;
            return sIndex;
        }

        public string VisitBasicType(BasicType basicType)
        {
            string sType = basicType.Name.Lexeme;
            sType += basicType.IsReference ? "&" : "";
            sType += basicType.IsNullable ? "?" : "";
            return sType;
        }

        public string VisitArrayType(ArrayType arrayType)
        {
            string sArr = arrayType.Type.Accept(this);
            sArr += "[";
            for (int i = 0; i < arrayType.Dimentions - 1; i++)
                sArr += ",";
            sArr += "]&";
            if (arrayType.IsNullable)
                sArr += "?";
            return sArr;
        }

        public string VisitFuncRefType(FuncPointerType funcRef)
        {
            string sFuncRef = funcRef.ReturnType.Accept(this);
            sFuncRef += "(";

            for(int i = 0; i < funcRef.Parameters.Count; i++)
            {
                if(i == funcRef.Parameters.Count - 1)
                {
                    sFuncRef += funcRef.Parameters[i].Accept(this);
                    break;
                }

                sFuncRef += funcRef.Parameters[i].Accept(this) + ", ";
            }

            sFuncRef += ")&";
            if (funcRef.IsNullable)
                sFuncRef += "?";
            return sFuncRef;
        }

        private string GetOffset()
        {
            string offset = "";
            for (int i = 0; i < m_IndentCount; i++)
            {
                offset += m_IndentSeperator;
            }

            return offset;
        }

        public string VisitSizeOf(SizeOfExpr sizeOfExpr)
        {
            throw new NotImplementedException();
        }

        public string VisitReinterpretCast(ReinterpretCastExpr reinterpretCastExpr)
        {
            throw new NotImplementedException();
        }
    }
}
