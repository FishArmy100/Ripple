using System;
using System.Collections.Generic;
using Raucse;

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
            AstGenerator.Generate("C:\\dev\\Ripple\\ASTGeneration\\src\\Tests", "ASTGeneration.Tests", "TestBase", 
                "", new List<string>
                {
                    "Test1 : int X; int Y",
                    "Test2 : int Z; int W"
                }, new List<string>() { "System.Collections.Generic" });
        }

        private static void GenerateCAST()
		{
            List<string> additionalUsings = new List<string>() { "System.Collections.Generic", "Raucse", "System", "System.Linq" };

            AstGenerator.Generate("C:\\dev\\Ripple\\Ripple\\src\\Transpiling\\C_AST\\Types", "Ripple.Transpiling.C_AST", "CType", "", new List<string>()
            {
                "CBasicType : string Name; bool IsConst; bool IsStruct",
                "CPointer : CType BaseType; bool IsConst",
                "CArray : CType BaseType; Option<int> Size",
                "CFuncPtr : CType Returned; List<CType> Parameters"
            }, additionalUsings);

            AstGenerator.Generate("C:\\dev\\Ripple\\Ripple\\src\\Transpiling\\C_AST\\Expressions", "Ripple.Transpiling.C_AST", "CExpression", "", new List<string>() 
            {
                "CBinary : CExpression Left; CBinaryOperator Op; CExpression Right",
                "CUnary : CExpression Expression; CUnaryOperator Op",
                "CIndex : CExpression Indexee; CExpression Argument",
                "CCall : CExpression Callee; List<CExpression> Arguments",
                "CCast : CExpression Castee; CType Type",
                "CIdentifier : string Id",
                "CSizeOf : CType Type",
                "CMemberAccess : CExpression Expression; string Identifier",
                "CLiteral : object Value; CLiteralType Type",
                "CCompoundLiteral : CType Type; CInitalizerList Initalizer",
                "CInitalizerList : List<CExpression> Expressions"
            }, additionalUsings);

            AstGenerator.Generate("C:\\dev\\Ripple\\Ripple\\src\\Transpiling\\C_AST\\Statements", "Ripple.Transpiling.C_AST", "CStatement", "", new List<string>()
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
                "CStructMember : CType Type; string Name",
                "CStructDef : string Name; List<CStructMember> Members",
                "CStructDecl : string Name",
                "CTypeDefStmt : CType Type; string Name",
                "CIncludeStmt : string File",
                "CFileStmt : List<CIncludeStmt> Includes; List<CStatement> Statements; string RelativePath; CFileType FileType",
            }, additionalUsings);
        }

        private static void GenerateRippleAst()
        {
            List<string> additionalUsings = new List<string>() { "System.Collections.Generic", "Ripple.Lexing", "Ripple.Parsing", "Raucse", "System", "System.Linq" };

            // Expressions
            AstGenerator.Generate("C:\\dev\\Ripple\\Ripple\\src\\AST\\Expressions", "Ripple.AST", "Expression", "", new List<string>()
            {
                "Literal : Token Val",
                "Grouping : Token LeftParen; Expression Expr; Token RightParen",
                "Call : Expression Callee; Token OpenParen; List<Expression> Args; Token CloseParen",
                "Index : Expression Indexed; Token OpenBracket; Expression Argument; Token CloseBracket",
                "Cast : Expression Castee; Token AsToken; TypeName TypeToCastTo",
                "Unary : Token Op; Expression Expr",
                "Binary : Expression Left; Token Op; Expression Right",
                "Identifier : Token Name",
                "InitializerList : Token OpenBrace; List<Expression> Expressions; Token CloseBrace",
                "MemberAccess : Expression Expression; Token Dot; Token MemberName",
                "SizeOf : Token SizeofToken; Token LessThan; TypeName Type; Token GreaterThan; Token OpenParen; Token CloseParen",
            }, additionalUsings);

            // Statements
            AstGenerator.Generate("C:\\dev\\Ripple\\Ripple\\src\\AST\\Statements", "Ripple.AST", "Statement", "", new List<string>()
            {
                "ExprStmt : Expression Expr; Token SemiColin",
                "BlockStmt : Token OpenBrace; List<Statement> Statements; Token CloseBrace",
                "IfStmt : Token IfTok; Token OpenParen; Expression Expr; Token CloseParen; Statement Body; Token? ElseToken; Option<Statement> ElseBody",
                "ForStmt : Token ForTok; Token OpenParen; Option<Statement> Init; Option<Expression> Condition; Option<Expression> Iter; Token CloseParen; Statement Body",
                "WhileStmt : Token WhileToken; Token OpenParen; Expression Condition; Token CloseParen; Statement Body",
                "VarDecl : Token? UnsafeToken; TypeName Type; Token? MutToken; List<Token> VarNames; Token? Equels; Option<Expression> Expr; Token SemiColin",
                "ReturnStmt : Token ReturnTok; Option<Expression> Expr; Token SemiColin",

                "ContinueStmt : Token ContinueToken; Token SemiColon",
                "BreakStmt : Token BreakToken; Token SemiColon",

                "Parameters : Token OpenParen; List<Pair<TypeName,Token>> ParamList; Token CloseParen",
                "GenericParameters : Token LessThan; List<Token> Lifetimes; Token GreaterThan",

                "WhereClause : Token WhereToken; Expression Expression",
                "UnsafeBlock : Token UnsafeToken; Token OpenBrace; List<Statement> Statements; Token CloseBrace",

                "FuncDecl : Token? UnsafeToken; Token FuncTok; Token Name; Option<GenericParameters> GenericParams; Parameters Param; Token Arrow; TypeName ReturnType; Option<WhereClause> WhereClause; BlockStmt Body",
                "ExternalFuncDecl : Token? UnsafeToken; Token ExternToken; Token Specifier; Token FuncToken; Token Name; Parameters Parameters; Token Arrow; TypeName ReturnType; Token SemiColon",

                "ConstructorDecl : Token? UnsafeToken; Token Identifier; Option<GenericParameters> GenericParameters; Parameters Parameters; BlockStmt Body",
                "DestructorDecl : Token? UnsafeToken; Token TildaToken; Token Identifier; Token OpenParen; Token CloseParen; BlockStmt Body",

                "ThisFunctionParameter : Token ThisToken; Token? MutToken; Token? RefToken; Token? LifetimeToken",
                "MemberFunctionParameters : Token OpenParen; Option<ThisFunctionParameter> ThisParameter; List<Pair<TypeName,Token>> ParamList; Token CloseParen",
                "MemberFunctionDecl : Token? UnsafeToken; Token FuncToken; Token NameToken; Option<GenericParameters> GenericParameters; MemberFunctionParameters Parameters; Token Arrow; TypeName ReturnType; BlockStmt Body",

                "MemberDecl : Token? VisibilityToken; Statement Declaration",
                "ClassDecl : Token? UnsafeToken; Token ClassToken; Token Name; Option<GenericParameters> GenericParameters; Token OpenBrace; List<MemberDecl> Members; Token CloseBrace",

                "FileStmt : List<Statement> Statements; string RelativePath; Token EOFTok",
                "ProgramStmt : List<FileStmt> Files; string Path"
            }, additionalUsings);

            // Types:
            AstGenerator.Generate("C:\\dev\\Ripple\\Ripple\\src\\AST\\Types", "Ripple.AST", "TypeName", "", new List<string>
            {
                "BasicType : Token Identifier",
                "GroupedType : Token OpenParen; TypeName Type; Token CloseParen",
                "PointerType : TypeName BaseType; Token? MutToken; Token Star",
                "ReferenceType : TypeName BaseType; Token? MutToken; Token Ampersand; Token? Lifetime",
                "ArrayType : TypeName BaseType; Token OpenBracket; Token Size; Token CloseBracket",
                "FuncPtr : Token FuncToken; Option<List<Token>> Lifetimes; Token OpenParen; List<TypeName> Parameters; Token CloseParen; Token Arrow; TypeName ReturnType",
            }, additionalUsings);
        }

        private static void GenerateTypedAST()
        {
            List<string> additionalUsings = new List<string>()
            {
                "System.Collections.Generic",
                "Raucse",
                "Ripple.Validation",
                "Ripple.Validation.Info.Types",
                "Ripple.Validation.Info",
                "Ripple.Validation.Info.Expressions",
                "Ripple.Lexing",
                "System",
                "System.Linq",
            };

            AstGenerator.Generate("C:\\dev\\Ripple\\Ripple\\src\\Validation\\Info\\Statements", "Ripple.Validation.Info.Statements", "TypedStatement",
                "", new List<string>
                {
                    "TypedExprStmt : TypedExpression Expression",
                    "TypedBlockStmt : List<TypedStatement> Statements",
                    "TypedIfStmt : TypedExpression Condition; TypedStatement Body; Option<TypedStatement> ElseBody",
                    "TypedForStmt : Option<TypedStatement> Initalizer; Option<TypedExpression> Condition; Option<TypedExpression> Iterator; TypedStatement Body",
                    "TypedWhileStmt : TypedExpression Condition; TypedStatement Body",
                    "TypedVarDecl : bool IsUnsafe; TypeInfo Type; bool IsMutable; List<string> VariableNames; TypedExpression Initalizer",
                    "TypedReturnStmt : Option<TypedExpression> Expression",

                    "TypedContinueStmt",
                    "TypedBreakStmt",

                    "TypedUnsafeBlock : List<TypedStatement> Statements",

                    "TypedFuncDecl : FunctionInfo Info; TypedBlockStmt Body",
                    "TypedExternalFuncDecl : FunctionInfo Info; string Header",

                    "TypedFileStmt : List<TypedStatement> Statements; string RelativePath",
                    "TypedProgramStmt : List<TypedFileStmt> Files; string Path"
                }, additionalUsings);

            // TypedExpression
            AstGenerator.Generate("C:\\dev\\Ripple\\Ripple\\src\\Validation\\Info\\Expressions", "Ripple.Validation.Info.Expressions", "TypedExpression", 
                "TypeInfo Returned", new List<string>
                {
                    "TypedIdentifier : string Name; Either<FunctionInfo,VariableInfo> Value",
                    "TypedInitalizerList : List<TypedExpression> Expressions",
                    "TypedLiteral : string Value; TokenType Type",
                    "TypedSizeOf : TypeInfo SizedType",
                    "TypedCall : TypedExpression Callee; List<TypedExpression> Arguments",
                    "TypedIndex : TypedExpression Indexee; TypedExpression Argument",
                    "TypedCast : TypedExpression Casted; TypeInfo TypeToCastTo",
                    "TypedBinary : TypedExpression Left; TokenType Op; TypedExpression Right",
                    "TypedUnary : TypedExpression Operand; TokenType Op"
                }, additionalUsings);

            // TypeInfo:
            AstGenerator.Generate("C:\\dev\\Ripple\\Ripple\\src\\Validation\\Info\\Types", "Ripple.Validation.Info.Types", "TypeInfo", "", new List<string>
            {
                "BasicTypeInfo : string Name",
                "PointerInfo : bool IsMutable; TypeInfo Contained",
                "ReferenceInfo : bool IsMutable; TypeInfo Contained; Option<ReferenceLifetime> Lifetime",
                "ArrayInfo : TypeInfo Contained; int Size",
                "FuncPtrInfo : int FunctionIndex; int LifetimeCount; List<TypeInfo> Parameters; TypeInfo Returned"
            }, additionalUsings);
        }
    }
}
