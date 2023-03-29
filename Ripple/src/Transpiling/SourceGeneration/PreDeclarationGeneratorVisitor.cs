using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Transpiling.C_AST;
using Ripple.Validation.Info.Statements;
using Ripple.Transpiling.ASTConversion;

namespace Ripple.Transpiling.SourceGeneration
{
    class PreDeclarationGeneratorVisitor : ITypedStatementVisitor<PreDeclarationData>
    {
        public PreDeclarationData VisitTypedBlockStmt(TypedBlockStmt typedBlockStmt)
        {
            throw new NotImplementedException();
        }

        public PreDeclarationData VisitTypedBreakStmt(TypedBreakStmt typedBreakStmt)
        {
            throw new NotImplementedException();
        }

        public PreDeclarationData VisitTypedContinueStmt(TypedContinueStmt typedContinueStmt)
        {
            throw new NotImplementedException();
        }

        public PreDeclarationData VisitTypedExprStmt(TypedExprStmt typedExprStmt)
        {
            throw new NotImplementedException();
        }

        public PreDeclarationData VisitTypedExternalFuncDecl(TypedExternalFuncDecl typedExternalFuncDecl)
        {
            return new PreDeclarationData(typedExternalFuncDecl.Header);
        }

        public PreDeclarationData VisitTypedFileStmt(TypedFileStmt typedFileStmt)
        {
            return PreDeclarationData.Merge(typedFileStmt.Statements.Select(s => s.Accept(this)));
        }

        public PreDeclarationData VisitTypedForStmt(TypedForStmt typedForStmt)
        {
            throw new NotImplementedException();
        }

        public PreDeclarationData VisitTypedFuncDecl(TypedFuncDecl typedFuncDecl)
        {
            return new PreDeclarationData(typedFuncDecl.Info);
        }

        public PreDeclarationData VisitTypedIfStmt(TypedIfStmt typedIfStmt)
        {
            throw new NotImplementedException();
        }

        public PreDeclarationData VisitTypedProgramStmt(TypedProgramStmt typedProgramStmt)
        {
            return PreDeclarationData.Merge(typedProgramStmt.Files.Select(s => s.Accept(this)));
        }

        public PreDeclarationData VisitTypedReturnStmt(TypedReturnStmt typedReturnStmt)
        {
            throw new NotImplementedException();
        }

        public PreDeclarationData VisitTypedUnsafeBlock(TypedUnsafeBlock typedUnsafeBlock)
        {
            throw new NotImplementedException();
        }

        public PreDeclarationData VisitTypedVarDecl(TypedVarDecl typedVarDecl)
        {
            return new PreDeclarationData(typedVarDecl);
        }

        public PreDeclarationData VisitTypedWhileStmt(TypedWhileStmt typedWhileStmt)
        {
            throw new NotImplementedException();
        }
    }
}
