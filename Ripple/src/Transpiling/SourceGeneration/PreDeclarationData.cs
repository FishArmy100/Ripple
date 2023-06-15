using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Transpiling.C_AST;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Statements;
using Ripple.Transpiling.ASTConversion;
using Ripple.Validation.Info.Functions;

namespace Ripple.Transpiling.SourceGeneration
{
    class PreDeclarationData
    {
        public readonly HashSet<string> Headers;
        public readonly List<FunctionInfo> FunctionPreDeclarations;
        public readonly List<TypedVarDecl> GlobalVariables;

        public PreDeclarationData(HashSet<string> headers, List<FunctionInfo> functionPreDeclarations, List<TypedVarDecl> globalVariables)
        {
            Headers = headers;
            FunctionPreDeclarations = functionPreDeclarations;
            GlobalVariables = globalVariables;
        }

        public PreDeclarationData(string header)
        {
            Headers = new HashSet<string> { header };
            FunctionPreDeclarations = new List<FunctionInfo>();
            GlobalVariables = new List<TypedVarDecl>();
        }

        public PreDeclarationData(FunctionInfo info)
        {
            Headers = new HashSet<string>();
            FunctionPreDeclarations = new List<FunctionInfo> { info };
            GlobalVariables = new List<TypedVarDecl>();
        }

        public PreDeclarationData(TypedVarDecl varDecl)
        {
            Headers = new HashSet<string>();
            FunctionPreDeclarations = new List<FunctionInfo>();
            GlobalVariables = new List<TypedVarDecl> { varDecl };
        }

        public static PreDeclarationData Merge(IEnumerable<PreDeclarationData> datas)
        {
            HashSet<string> headers = datas.Select(d => d.Headers).SelectMany(h => h).ToHashSet();
            List<FunctionInfo> funcDecls = datas.Select(d => d.FunctionPreDeclarations).SelectMany(f => f).ToList();
            List<TypedVarDecl> varDecls = datas.Select(d => d.GlobalVariables).SelectMany(v => v).ToList();
            return new PreDeclarationData(headers, funcDecls, varDecls);
        }

        public CFileStmt GeneratePredefFile(StatementConverterVisitor statementConverter, TypeConverterVisitor typeConverter, string path)
        {
            List<CStatement> statements = new List<CStatement>();

            List<CIncludeStmt> headers = Headers
                .Select(h => new CIncludeStmt(h))
                .ToList();

            var vars = GlobalVariables
                .Select(v => v.Accept(statementConverter))
                .SelectMany(v => v);
            statements.AddRange(vars);

            var funcs = FunctionPreDeclarations
                .Select(f =>
                {
                    CType returned = f.ReturnType.Accept(typeConverter);
                    List<CFuncParam> parameters = f.Parameters.Select(p => new CFuncParam(p.Type.Accept(typeConverter), p.Name)).ToList();
                    string name = f.Name;
                    return new CFuncDecl(returned, name, parameters);
                });
            statements.AddRange(funcs);

            return new CFileStmt(headers, statements, path, CFileType.Header);
        }
    }
}
