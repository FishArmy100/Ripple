using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    struct TypeName
    {
        public readonly string Name;

        public TypeName(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Attempts to convert a literal token type to a type name
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typeName"></param>
        /// <returns>false if the token type is not a literal, identifiers dont count as literals</returns>
        public static bool FromTokenTypeLiteral(TokenType type, out TypeName typeName)
        {
            typeName = new TypeName();

            switch(type)
            {
                case TokenType.True:
                    typeName = new TypeName("bool");
                    return true;
                case TokenType.False:
                    typeName = new TypeName("bool");
                    return true;
                case TokenType.Int:
                    typeName = new TypeName("int");
                    return true;
                case TokenType.Uint:
                    typeName = new TypeName("uint");
                    return true;
                case TokenType.Float:
                    typeName = new TypeName("float");
                    return true;
                case TokenType.Char:
                    typeName = new TypeName("char");
                    return true;
                case TokenType.String:
                    typeName = new TypeName("string");
                    return true;
                default:
                    return false;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            return obj is TypeName name &&
                   Name == name.Name;
        }

        public static bool operator==(TypeName left, TypeName right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TypeName left, TypeName right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
