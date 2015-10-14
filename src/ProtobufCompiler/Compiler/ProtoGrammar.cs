using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ProtobufCompiler.Compiler
{
    /// <summary>
    /// Here we'll define some helpers 
    /// </summary>
    internal static class ProtoGrammar
    {
        /// <summary>
        /// Provides support for handling EOL on Windows and Unix
        /// </summary>
        internal static char LineFeed = '\n';

        /// <summary>
        /// Provides support for handling EOL on Windows and Mac
        /// </summary>
        internal static char CarriageReturn = '\r';

        /// <summary>
        /// Characters which may be placed beside another character and not be considered the same token. 
        /// </summary>
        internal static readonly IList<char> InlineTokens = new List<char>{ ';', '{', '}', '=', ',', '<', '>', '[', ']', '(', ')' };

        /// <summary>
        /// Characters which will invoke a look behind to delimit the start or end of a comment. 
        /// </summary>
        internal static readonly char[] Comment = {'/', '*'};

        /// <summary>
        /// Identify Numeric Tokens
        /// </summary>
        internal static readonly Regex IsNumeric = new Regex("^(" +
                /*Hex*/ @"0x[0-9a-f]+" + "|" +
                /*Oct*/ @"0[0-7]*" + "|" +
                /*Dec*/ @"((?!0)|[-+]|(?=0+\.))(\d*\.)?\d+(e\d+)?" +
                ")$", RegexOptions.Compiled);

        internal static readonly IList<string> IdTable = new List<string>
        {
            "syntax",
            "import",
            "package",
            "option",
            "enum",
            "message",
            "oneof",
            "map"
        };

        internal static readonly IList<string> BlockDefinitions = new List<string>
        {
            "enum",
            "message",
            "service",
            "/*"
        };

        internal static readonly IList<string> LineDefinitions = new List<string>
        {
            "syntax",
            "import",
            "package",
            "option",
            "//"
        };

        internal static readonly IList<string> FieldDefinitions = new List<string>
        {
            "double",
            "float",
            "int32",
            "int64",
            "uint32",
            "uint64",
            "sint32",
            "sint64",
            "fixed32",
            "fixed64",
            "sfixed32",
            "sfixed64",
            "bool",
            "string",
            "bytes"
        };

        internal static TokenType GetType(string lexeme)
        {

            if (IdTable.Contains(lexeme))
            {
                return TokenType.Id;
            }
            if (IsNumeric.IsMatch(lexeme))
            {
                return TokenType.Numeric;
            }
            if (lexeme.Length == 1 && InlineTokens.Contains(lexeme[0]))
            {
                return TokenType.Control;
            }
            return TokenType.String;
        }

    }
}
