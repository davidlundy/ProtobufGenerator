using System.Text.RegularExpressions;
using ProtobufCompiler.Compiler;

namespace ProtobufCompiler.Extensions
{
    internal static class ParserExtensions
    {
        internal static bool IsCarriageReturn(this char self)
        {
            return '\r'.Equals(self);
        }

        internal static bool IsLineFeed(this char self)
        {
            return '\n'.Equals(self);
        }

        internal static bool IsForwardSlash(this char self)
        {
            return '/'.Equals(self);
        }

        internal static bool IsAsterisk(this char self)
        {
            return '*'.Equals(self);
        }

        /// <summary>
        /// Characters which may be placed beside another character and not be considered the same token. 
        /// </summary>
        internal static bool IsInlineToken(this char self)
        {
            return ';'.Equals(self) || '{'.Equals(self) || '}'.Equals(self) || '='.Equals(self) ||
                   ','.Equals(self) || '<'.Equals(self) || '>'.Equals(self) || '['.Equals(self) ||
                   ']'.Equals(self) || '('.Equals(self) || ')'.Equals(self);
        }

        private static readonly Regex IsNumericRegex = new Regex("^(" +
                /*Hex*/ @"0x[0-9a-f]+" + "|" +
                /*Oct*/ @"0[0-7]*" + "|" +
                /*Dec*/ @"((?!0)|[-+]|(?=0+\.))(\d*\.)?\d+(e\d+)?" +
                ")$", RegexOptions.Compiled);

        /// <summary>
        /// Returns if the token is 'syntax', 'import', 'package', 'option', 'enum', 'message', 'oneof', or 'map'.
        /// </summary>
        internal static bool IsIdToken(this string self)
        {
            return "syntax".Equals(self) ||  "import".Equals(self) ||  "package".Equals(self) ||  
                "option".Equals(self) ||  "enum".Equals(self) ||  "message".Equals(self) ||  
                "oneof".Equals(self) ||  "map".Equals(self);
        }

        internal static bool IsReservedString(this string self)
        {
            return "enum".Equals(self) || "message".Equals(self) || "oneof".Equals(self) ||
                "map".Equals(self) || "reserved".Equals(self);
        }

        internal static bool IsMapKeyType(this string self)
        {
            return "int32".Equals(self) || "int64".Equals(self) || "uint32".Equals(self) || 
                   "uint64".Equals(self) || "sint32".Equals(self) | "sint64".Equals(self) || 
                   "fixed32".Equals(self) || "fixed64".Equals(self) || "sfixed32".Equals(self) || 
                   "sfixed64".Equals(self) || "bool".Equals(self) || "string".Equals(self);
        }

        /// <summary>
        /// Removes the leading and trailing quotes on a string literal. Practically first and last character. 
        /// Don't use in general case. Specific case to the parser is that we are only dealing with quoted strings
        /// in that format. 
        /// </summary>
        internal static string Unquote(this string self)
        {
            return self.Substring(1, self.Length - 2);
        }

        /// <summary>
        /// Identify Numeric Tokens, including Hex, Octal, and Decimal with exponent
        /// </summary>
        internal static bool IsNumeric(this string self)
        {
            return IsNumericRegex.IsMatch(self);
        }

        internal static bool IsType(this string self)
        {
            return IsMapKeyType(self) || "double".Equals(self) || 
                "float".Equals(self) || "bytes".Equals(self);
        }

        internal static TokenType GetTokenType(this string self)
        {
            if (self.IsIdToken())
            {
                return TokenType.Id;
            }
            if (self.IsNumeric())
            {
                return TokenType.Numeric;
            }
            if (self.Length == 1 && self[0].IsInlineToken())
            {
                return TokenType.Control;
            }
            return TokenType.String;
        }
    }
}
