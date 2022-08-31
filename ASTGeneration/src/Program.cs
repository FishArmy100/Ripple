using System;
using System.Collections.Generic;

namespace ASTGeneration
{
    struct Token { }

    class Program
    {
        static void Main(string[] args)
        {
            List<string> additionalUsings = new List<string>() { "System.Collections.Generic", "Ripple.Lexing", "Ripple.Parsing" };

            //Expressions
            AstGenerator.Generate("C:\\dev\\Ripple\\Ripple\\src\\AST\\Expressions", "Ripple.AST", "Expression", new List<string>()
            {
                "Literal : Token Val",
                "Grouping : Token LeftParen; Expression Expr; Token RightParen",
                "Call : Expression Expr; Token OpenParen; List<Expression> Args; Token CloseParen",
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
        }
    }
}
