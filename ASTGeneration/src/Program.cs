using System;
using System.Collections.Generic;

namespace ASTGeneration
{
    struct Token { }

    class Program
    {
        static void Main(string[] args)
        {
            //RunTests();
            GenerateRippleAst();
        }

        private static void RunTests()
        {
            AstGenerator.Generate("C:\\dev\\Ripple\\ASTGeneration\\src\\Tests", "ASTGeneration.Tests", "Expression", new List<string>
            {
                "Literal : string LiteralValue",
                "Binary : Expression Left; char Op; Expression Right",
                "Unary : Expression Operand; char Op"
            }, new List<string>());
        }

        private static void GenerateRippleAst()
        {
            List<string> additionalUsings = new List<string>() { "System.Collections.Generic", "Ripple.Lexing", "Ripple.Parsing" };

            //Expressions
            AstGenerator.Generate("C:\\dev\\Ripple\\Ripple\\src\\AST\\Expressions", "Ripple.AST", "Expression", new List<string>()
            {
                "Literal : Token Val",
                "Grouping : Token LeftParen; Expression Expr; Token RightParen",
                "Call : Token Identifier; Token OpenParen; List<Expression> Args; Token CloseParen",
                "Unary : Token Op; Expression Expr",
                "Binary : Expression Left; Token Op; Expression Right",
                "Identifier : Token Name",
            }, additionalUsings);

            //Statements
            AstGenerator.Generate("C:\\dev\\Ripple\\Ripple\\src\\AST\\Statements", "Ripple.AST", "Statement", new List<string>()
            {
                "ExprStmt : Expression Expr; Token SemiColin",
                "BlockStmt : Token OpenBrace; List<Statement> Statements; Token CloseBrace",
                "IfStmt : Token IfTok; Token OpenParen; Expression Expr; Token CloseParen; Statement Body",
                "ForStmt : Token ForTok; Token OpenParen; Statement Init; Expression Condition; Expression Iter; Token CloseParen; Statement Body",
                "VarDecl : Token TypeName; List<Token> VarNames; Token Equels; Expression Expr; Token SemiColin",
                "ReturnStmt : Token ReturnTok; Expression Expr; Token SemiColin",

                "Parameters : Token OpenParen; List<(Token,Token)> ParamList; Token CloseParen",
                "FuncDecl : Token FuncTok; Token Name; Parameters Param; Token Arrow; Token ReturnType; BlockStmt Body",

                "FileStmt : List<Statement> Statements; Token EOFTok",
            }, additionalUsings);

            // Types:
            AstGenerator.Generate("C:\\dev\\Ripple\\Ripple\\src\\AST\\Types", "Ripple.AST", "TypeName", new List<string>
            {
                "GroupedType : Token OpenParen; TypeName GroupedType; Token CloseParen",
                "PointerType : TypeName BaseType; Token Star",
                "ReferenceType : TypeName BaseType; Token Ampersand",
                "FuncPtr : Token OpenParen; List<TypeName> Parameters; Token CloseParen; Token Arrow; TypeName ReturnType",
            }, additionalUsings);
        }
    }
}
