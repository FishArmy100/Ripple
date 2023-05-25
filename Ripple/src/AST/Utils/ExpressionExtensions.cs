using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Core;

namespace Ripple.AST.Utils
{
    public static class ExpressionExtensions
    {
        public static SourceLocation GetLocation(this Expression expression)
        {
            return expression.Accept(new LocationFinderVisitor());
        }

        public class LocationFinderVisitor : IExpressionVisitor<SourceLocation>
        {
            public SourceLocation VisitBinary(Binary binary)
            {
                return binary.Left.Accept(this) + binary.Right.Accept(this);
            }

            public SourceLocation VisitCall(Call call)
            {
                return call.Callee.Accept(this) + call.OpenParen.Location + call.CloseParen.Location;
            }

            public SourceLocation VisitCast(Cast cast)
            {
                return cast.Castee.Accept(this) + cast.TypeToCastTo.GetLocation();
            }

            public SourceLocation VisitGrouping(Grouping grouping)
            {
                return grouping.LeftParen.Location + grouping.RightParen.Location;
            }

            public SourceLocation VisitIdentifier(Identifier identifier)
            {
                return identifier.Name.Location;
            }

            public SourceLocation VisitIndex(Index index)
            {
                return index.Indexed.Accept(this) + index.OpenBracket.Location + index.CloseBracket.Location;
            }

            public SourceLocation VisitInitializerList(InitializerList initializerList)
            {
                return initializerList.OpenBrace.Location + initializerList.CloseBrace.Location;
            }

            public SourceLocation VisitLiteral(Literal literal)
            {
                return literal.Val.Location;
            }

            public SourceLocation VisitMemberAccess(MemberAccess memberAccess)
            {
                return memberAccess.Expression.Accept(this) + memberAccess.MemberName.Location;
            }

            public SourceLocation VisitSizeOf(SizeOf sizeOf)
            {
                return sizeOf.SizeofToken.Location + sizeOf.CloseParen.Location;
            }

            public SourceLocation VisitTypeExpr(TypeExpr typeExpr)
            {
                return typeExpr.Type.GetLocation();
            }

            public SourceLocation VisitUnary(Unary unary)
            {
                return unary.Op.Location + unary.Expr.GetLocation();
            }
        }
    }
}
