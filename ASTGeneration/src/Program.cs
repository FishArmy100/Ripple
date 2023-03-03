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
            GenerateCAST();
        }

        private static void RunTests()
        {
            AstGenerator.Generate("C:\\dev\\Ripple\\ASTGeneration\\src\\Tests", "ASTGeneration.Tests", "Expression", new List<string>
            {
                "Test : string a; string b; string c; string d; string e; string f; string g; string h; string i",
            }, new List<string>());
        }

        private static void GenerateCAST()
		{
            List<string> additionalUsings = new List<string>() { "System.Collections.Generic", "Ripple.Utils" };

            AstGenerator.Generate("C:\\dev\\Ripple\\Ripple\\src\\Transpiling\\C_AST\\Types", "Ripple.Transpiling.C_AST", "CType", new List<string>()
            {
                "CBasicType : string Name; bool IsConst",
                "CPointer : CType BaseType; bool IsConst",
                "CArray : CType BaseType; Option<int> Size",
                "CFuncPtr : CType Returned; List<CType> Parameters"
            }, additionalUsings);

            AstGenerator.Generate("C:\\dev\\Ripple\\Ripple\\src\\Transpiling\\C_AST\\Expressions", "Ripple.Transpiling.C_AST", "CExpression", new List<string>() 
            {
                "CBinary : CExpression Left; CBinaryOperator Op; CExpression Right",
                "CUnary : CExpression Expression; CUnaryOperator Op",
                "CIndex : CExpression Indexee; CExpression Argument",
                "CCall : CExpression Callee; List<CExpression> Arguments",
                "CCast : CExpression Castee; CType Type",
                "CIdentifier : string Id",
                "CSizeOf : CType Type",
                "C" +
                "Literal : object Value; CLiteralType Type"
            }, additionalUsings);

            AstGenerator.Generate("C:\\dev\\Ripple\\Ripple\\src\\Transpiling\\C_AST\\Statements", "Ripple.Transpiling.C_AST", "CStatement", new List<string>()
            {
                "ExprStmt : CExpression Expression", 
                "IfStmt : CExpression Condition",
                "WhileStmt : CExpression Condition",
                "ForStmt : VarDecl Initalizer; CExpression Condition; CExpression Iterator",
                "BlockStmt : List<CStatement> Statements",
                "VarDecl : CType Type; string Name; Option<CExpression> Initializer",
                "ReturnStmt : CExpression Expression",
                "BreakStmt : ",
                "ContinueStmt : ",
                "FuncParam : CType Type; string Name",
                "FuncDecl : CType Returned; string Name; List<FuncParam> Parameters; BlockStmt Body",
                "StructMember : CType Type; string Name",
                "StructDecl : string Name; List<StructMember> Members",
                "TypeDefStmt : CType Type; string Name",
                "IncludeStmt : string File",
                "FileStmt : List<IncludeStmt> Includes; List<CStatement> Statements; CFileType FileType",
                "ProgramStmt : List<FileStmt> Files"
            }, additionalUsings);
        }

        private static void GenerateRippleAst()
        {
            List<string> additionalUsings = new List<string>() { "System.Collections.Generic", "Ripple.Lexing", "Ripple.Parsing", "Ripple.Utils" };

            // Expressions
            AstGenerator.Generate("C:\\dev\\Ripple\\Ripple\\src\\AST\\Expressions", "Ripple.AST", "Expression", new List<string>()
            {
                "Literal : Token Val",
                "Grouping : Token LeftParen; Expression Expr; Token RightParen",
                "Call : Expression Callee; Token OpenParen; List<Expression> Args; Token CloseParen",
                "Index : Expression Indexed; Token OpenBracket; Expression Argument; Token CloseBracket",
                "Cast : Expression Castee; Token AsToken; TypeName TypeToCastTo",
                "Unary : Token Op; Expression Expr",
                "Binary : Expression Left; Token Op; Expression Right",
                "Identifier : Token Name",
                "TypeExpression : Token Name; Token GreaterThan; List<Token> Lifetimes; Token LessThan",
                "InitializerList : Token OpenBrace; List<Expression> Expressions; Token CloseBrace",
                "SizeOf : Token SizeofToken; Token LessThan; TypeName Type; Token GreaterThan; Token OpenParen; Token CloseParen",
            }, additionalUsings);

            // Statements
            AstGenerator.Generate("C:\\dev\\Ripple\\Ripple\\src\\AST\\Statements", "Ripple.AST", "Statement", new List<string>()
            {
                "ExprStmt : Expression Expr; Token SemiColin",
                "BlockStmt : Token OpenBrace; List<Statement> Statements; Token CloseBrace",
                "IfStmt : Token IfTok; Token OpenParen; Expression Expr; Token CloseParen; Statement Body; Token? ElseToken; Option<Statement> ElseBody",
                "ForStmt : Token ForTok; Token OpenParen; Option<Statement> Init; Option<Expression> Condition; Option<Expression> Iter; Token CloseParen; Statement Body",
                "WhileStmt : Token WhileToken; Token OpenParen; Expression Condition; Token CloseParen; Statement Body",
                "VarDecl : Token? UnsafeToken; TypeName Type; List<Token> VarNames; Token Equels; Expression Expr; Token SemiColin",
                "ReturnStmt : Token ReturnTok; Option<Expression> Expr; Token SemiColin",

                "ContinueStmt : Token ContinueToken; Token SemiColon",
                "BreakStmt : Token BreakToken; Token SemiColon",

                "Parameters : Token OpenParen; List<(TypeName,Token)> ParamList; Token CloseParen",
                "GenericParameters : Token LessThan; List<Token> Lifetimes; Token GreaterThan",

                "WhereClause : Token WhereToken; Expression Expression",
                "UnsafeBlock : Token UnsafeToken; Token OpenBrace; List<Statement> Statements; Token CloseBrace",

                "FuncDecl : Token? UnsafeToken; Token FuncTok; Token Name; Option<GenericParameters> GenericParams; Parameters Param; Token Arrow; TypeName ReturnType; Option<WhereClause> WhereClause; BlockStmt Body",
                "ExternalFuncDecl : Token ExternToken; Token Specifier; Token FuncToken; Token Name; Parameters Parameters; Token Arrow; TypeName ReturnType; Token SemiColon",

                "FileStmt : List<Statement> Statements; string FilePath; Token EOFTok",
                "ProgramStmt : List<FileStmt> Files"
            }, additionalUsings);

            // Types:
            AstGenerator.Generate("C:\\dev\\Ripple\\Ripple\\src\\AST\\Types", "Ripple.AST", "TypeName", new List<string>
            {
                "BasicType : Token? MutToken; Token Identifier",
                "GroupedType : Token OpenParen; TypeName Type; Token CloseParen",
                "PointerType : TypeName BaseType; Token? MutToken; Token Star",
                "ReferenceType : TypeName BaseType; Token? MutToken; Token Ampersand; Token? Lifetime",
                "ArrayType : TypeName BaseType; Token? MutToken; Token OpenBracket; Token Size; Token CloseBracket",
                "FuncPtr : Token? MutToken; Token FuncToken; Option<List<Token>> Lifetimes; Token OpenParen; List<TypeName> Parameters; Token CloseParen; Token Arrow; TypeName ReturnType",
            }, additionalUsings);

            // TypeInfo:
            AstGenerator.Generate("C:\\dev\\Ripple\\Ripple\\src\\Validation\\Info\\Types", "Ripple.Validation.Info.Types", "TypeInfo", new List<string>
            {
                "BasicTypeInfo : bool IsMutable; string Name",
                "PointerInfo : bool IsMutable; TypeInfo Contained",
                "ReferenceInfo : bool IsMutable; TypeInfo Contained; Option<ReferenceLifetime> Lifetime",
                "ArrayInfo : bool IsMutable; TypeInfo Contained; int Size",
                "FuncPtrInfo : bool IsMutable; int FunctionIndex; int LifetimeCount; List<TypeInfo> Parameters; TypeInfo Returned"
            }, additionalUsings);
        }
    }
}
