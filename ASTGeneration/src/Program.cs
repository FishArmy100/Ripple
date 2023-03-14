using System;
using System.Collections.Generic;

namespace ASTGeneration
{
    class Program
    {
        static void Main(string[] args)
        {
            //RunTests();
            GenerateRippleAst();
            GenerateTypedAST();
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
                "CMemberAccess : CExpression Expression; string Identifier",
                "CLiteral : object Value; CLiteralType Type"
            }, additionalUsings);

            AstGenerator.Generate("C:\\dev\\Ripple\\Ripple\\src\\Transpiling\\C_AST\\Statements", "Ripple.Transpiling.C_AST", "CStatement", new List<string>()
            {
                "CExprStmt : CExpression Expression", 
                "CIfStmt : CExpression Condition; CStatement Body; Option<CStatement> ElseBody",
                "CWhileStmt : CExpression Condition; CStatement Body",
                "CForStmt : Option<CVarDecl> Initalizer; Option<CExpression> Condition; Option<CExpression> Iterator; CStatement Body",
                "CBlockStmt : List<CStatement> Statements",
                "CVarDecl : CType Type; string Name; Option<CExpression> Initializer",
                "CReturnStmt : Option<CExpression> Expression",
                "CBreakStmt",
                "CContinueStmt",
                "CFuncDef : CType Returned; string Name; List<CFuncParam> Parameters; CBlockStmt Body",
                "CFuncDecl : CType Returned; string Name; List<CFuncParam> Parameters",
                "CStructMember : CType Type; string Name; Option<CExpression> Initalizer",
                "CStructDef : string Name; List<CStructMember> Members",
                "CStructDecl : string Name",
                "CTypeDefStmt : CType Type; string Name",
                "CIncludeStmt : string File",
                "CFileStmt : List<CIncludeStmt> Includes; List<CStatement> Statements; string RelativePath; CFileType FileType",
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

            // Simplified types
            AstGenerator.Generate("C:\\dev\\Ripple\\Ripple\\src\\Transpiling\\ASTConversion\\SimplifiedTypes", "Ripple.Transpiling.ASTConversion.SimplifiedTypes", "SimplifiedType", new List<string>
            {
                "SBasicType : bool IsMutable; string Name",
                "SPointer : bool IsMutable; SimplifiedType Contained",
                "SReference : bool IsMutable; SimplifiedType Contained",
                "SArray : bool IsMutable; SimplifiedType Contained; int Size",
                "SFuncPtr : bool IsMutable; List<SimplifiedType> Parameters; SimplifiedType Returned"
            }, additionalUsings);
        }

        private static void GenerateTypedAST()
        {
            List<string> additionalUsings = new List<string>() { "System.Collections.Generic", "Ripple.Utils" };

            // TypedExpression
            AstGenerator.Generate("C:\\dev\\Ripple\\Ripple\\src\\Validation\\Info\\TypedExpression", "Ripple.Validation.Info.TypedExpression", "TypedExpression", new List<string>
            { 

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
