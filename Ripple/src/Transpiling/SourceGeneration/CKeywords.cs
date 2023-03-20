using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Transpiling.SourceGeneration
{
	static class CKeywords
	{
		public const string ALIGNAS = "alignas";
		public const string ALIGNOF = "alignof";
		public const string AUTO = "auto";
		public const string BOOL = "bool";
		public const string BREAK = "break";
		public const string CASE = "case";
		public const string CHAR = "char";
		public const string CONST = "const";
		public const string CONSTEXPR = "constexpr";
		public const string CONTINUE = "continue";
		public const string DEFAULT = "default";
		public const string DO = "do";
		public const string DOUBLE = "double";
		public const string ELSE = "else";
		public const string ENUM = "enum";
		public const string EXTERN = "extern";
		public const string FALSE = "false";
		public const string FLOAT = "float";
		public const string FOR = "for";
		public const string GOTO = "goto";
		public const string IF = "if";
		public const string INLINE = "inline";
		public const string INT = "int";
		public const string LONG = "long";
		public const string NULLPTR = "nullptr";
		public const string REGISTER = "register";
		public const string RESTRICT = "restrict";
		public const string RETURN = "return";
		public const string SHORT = "short";
		public const string SIGNED = "signed";
		public const string SIZEOF = "sizeof";
		public const string STATIC = "static";
		public const string STATIC_ASSSERT = "static_assert";
		public const string SWITCH = "switch";
		public const string STRUCT = "struct";
		public const string THREAD_LOCAL = "thread_local";
		public const string TRUE = "true";
		public const string TYPEDEF = "typedef";
		public const string TYPEOF = "typeof";
		public const string TYPEOF_UNQUAL = "typeof_unqual";
		public const string UNION = "union";
		public const string UNSIGNED = "unsigned";
		public const string VOID = "void";
		public const string VOLATILE = "volatile";
		public const string WHILE = "while";

		public const string HASH_IF = "#if";
		public const string HASH_ELIF = "#elif";
		public const string HASH_ELSE = "#else";

		public const string HASH_IFDEF = "#ifdef";
		public const string HASH_IFNDEF = "#ifndef";
		public const string HASH_ELIFDEF = "#elifdef";
		public const string HASH_ELIFNDEF = "#elifndef";
		public const string HASH_DEFINE = "#define";
		public const string HASH_UNDEF = "#undef";

		public const string HASH_INCLUDE = "#include";
		public const string HASH_EMBED = "#embed";
		public const string HASH_LINE = "#line";
		public const string HASH_ERROR = "#error";
		public const string HASH_WARNING = "#warning";
		public const string HASH_PRAGMA = "#pragma";
		public const string HASH_DEFINED = "#defined";

		public const string HASH_PRAGMA_ONCE = "#pragma once";

		public const string INT_8BIT = "int8_t";
		public const string INT_16BIT = "int16_t";
		public const string INT_32BIT = "int32_t";
		public const string INT_64BIT = "int64_t";

		public const string UINT_8BIT = "uint8_t";
		public const string UINT_16BIT = "uint16_t";
		public const string UINT_32BIT = "uint32_t";
		public const string UINT_64BIT = "uint64_t";

		public const string FLOAT_32BIT = "float";
		public const string FLOAT_64BIT = "double";

		public const string NULL_MACRO = "NULL";

		public const string ARRAY_DATA_NAME = "data";
		public const string TEMP_VAR_PREFIX = "_temp_var"
	}
}
