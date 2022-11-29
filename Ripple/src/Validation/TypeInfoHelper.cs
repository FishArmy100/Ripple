using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST.Info;
using Ripple.AST;
using Ripple.Utils;
using Ripple.Lexing;

namespace Ripple.Validation
{
    static class TypeInfoHelper
    {
        public struct Error
        {
            public Token Token;
            public string Message;

            public Error(Token token, string message)
            {
                Token = token;
                Message = message;
            }
        }

        public static Result<TypeInfo, Error> GetTypeOfExpression(ASTInfo ast, Expression expr)
        {
            throw new NotImplementedException();
        }

        private class TypeOfExpressionVisitor : IExpressionVisitor<TypeInfo>
        {
            public TypeInfo VisitBinary(Binary binary)
            {
                throw new NotImplementedException();
            }

            public TypeInfo VisitCall(Call call)
            {
                throw new NotImplementedException();
            }

            public TypeInfo VisitCast(Cast cast)
            {
                throw new NotImplementedException();
            }

            public TypeInfo VisitGrouping(Grouping grouping)
            {
                throw new NotImplementedException();
            }

            public TypeInfo VisitIdentifier(Identifier identifier)
            {
                throw new NotImplementedException();
            }

            public TypeInfo VisitIndex(AST.Index index)
            {
                throw new NotImplementedException();
            }

            public TypeInfo VisitInitializerList(InitializerList initializerList)
            {
                throw new NotImplementedException();
            }

            public TypeInfo VisitLiteral(Literal literal)
            {
                throw new NotImplementedException();
            }

            public TypeInfo VisitSizeOf(SizeOf sizeOf)
            {
                throw new NotImplementedException();
            }

            public TypeInfo VisitTypeExpression(TypeExpression typeExpression)
            {
                throw new NotImplementedException();
            }

            public TypeInfo VisitUnary(Unary unary)
            {
                throw new NotImplementedException();
            }
        }
    }
}
