using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Utils
{
    class TokenUtils
    {
        public static bool TryGetTypeFromTokenType(TokenType tokenType, ASTInfo info, out ASTType astType)
        {
            astType = new ASTType();

            switch(tokenType)
            {
                case TokenType.True:
                case TokenType.False:
                    if (info.TryGetType(RippleKeywords.BOOL_TYPE_NAME, out astType))
                        return true;
                    break;
                case TokenType.CharLiteral:
                    if (info.TryGetType(RippleKeywords.CHAR_TYPE_NAME, out astType))
                        return true;
                    break;
                case TokenType.IntLiteral:
                    if (info.TryGetType(RippleKeywords.INT_TYPE_NAME, out astType))
                        return true;
                    break;
                case TokenType.FloatLiteral:
                    if (info.TryGetType(RippleKeywords.FLOAT_TYPE_NAME, out astType))
                        return true;
                    break;
                case TokenType.StringLiteral:
                    if (info.TryGetType(RippleKeywords.FLOAT_TYPE_NAME, out astType))
                        return true;
                    break;
                default:
                    break;
            }

            return false;
        }
    }
}
