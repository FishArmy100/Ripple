using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST.Info;

namespace Ripple.Utils
{
    static class TokenUtils
    {
        /// <summary>
        /// Converts a token type into a AST type based on the information in a AST info
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="info"></param>
        /// <param name="astType"></param>
        /// <returns></returns>
        public static bool TryGetTypeFromTokenType(TokenType tokenType, ASTInfo info, out ASTTypeInfo astType)
        {
            astType = new ASTTypeInfo();

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
