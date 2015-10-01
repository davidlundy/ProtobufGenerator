using Xunit;
using Sprache;

namespace ProtobufCompiler.Tests
{
    public class LexicalTests
    {
        private readonly ProtoGrammar _sys;

        public LexicalTests()
        {
            _sys = new ProtoGrammar();
        }

        [Theory]
        [InlineData("0")]
        [InlineData("9")]
        [InlineData("A")]
        [InlineData("F")]
        [InlineData("a")]
        [InlineData("f")]
        // hexDigit     = "0" … "9" | "A" … "F" | "a" … "f"
        public void HexDigit_is_0to9_and_AtoF_CaseInsensitive(string input)
        {
            var output = _sys.HexDigit.Parse(input);
            Assert.Equal(input[0], output);
        }

        [Theory]
        [InlineData("G")]
        [InlineData("h")]
        [InlineData("*")]
        public void HexDigit_is_0to9_and_AtoF_CaseInsensitive_Throws(string input)
        {
            Assert.Throws<ParseException>(() => _sys.HexDigit.Parse(input));
        }

        [Theory]
        [InlineData("0")]
        [InlineData("7")]
        // octalDigit   = "0" … "7"
        public void OctalDigit_is_0to7(string input)
        {
            var output = _sys.OctalDigit.Parse(input);
            Assert.Equal(input[0], output);
        }

        [Theory]
        [InlineData("8")]
        [InlineData("A")]
        public void OctalDigit_is_0to7_Throws(string input)
        {
            Assert.Throws<ParseException>(() => _sys.OctalDigit.Parse(input));
        }

        [Theory]
        [InlineData("Aab_z34 ", "Aab_z34")]
        [InlineData("A ", "A")]
        [InlineData("A asdf", "A")]
        [InlineData("Aab%z34 ", "Aab")]
        // ident = letter { letter | unicodeDigit | "_" }
        public void Identifier_is_Letter_Then_LetterNumberOrUnderscore(string input, string expectedResult)
        {
            Assert.Equal(expectedResult, _sys.Identifier.Parse(input));
        }

        [Theory]
        [InlineData("3ab_z34 ")]
        [InlineData("*ab%z34 ")]
        public void Identifier_is_Letter_Then_LetterNumberOrUnderscore_Throws(string input)
        {
            Assert.Throws<ParseException>(() => _sys.Identifier.Parse(input));
        }

        [Theory]
        [InlineData(".Aab_z34", ".Aab_z34")]
        [InlineData(".A", ".A")]
        // fullIdent = ident {"." ident}
        public void DotSeparatedIdentifier_is_Dot_and_Identifier(string input, string expected)
        {
            Assert.Equal(expected, _sys.DotSeparatedIdentifier.Parse(input));
        }

        [Theory]
        [InlineData("Aab_z34", "Aab_z34")]
        [InlineData("Aab_z34.E4_2", "Aab_z34.E4_2")]
        [InlineData("Foo.Bar.Baz", "Foo.Bar.Baz")]
        // fullIdent = ident {"." ident}
        public void FullIdentifier_is_Identifier_Plus_Multiple_DotSeperated_Identifiers(string input, string expected)
        {
            Assert.Equal(expected, _sys.FullIdentifier.Parse(input));
        }

        [Theory]
        [InlineData(".IdentA.IdentB.MessageName", ".IdentA.IdentB.MessageName")]
        [InlineData("IdentA.IdentB.MessageName", "IdentA.IdentB.MessageName")]
        [InlineData(".MessageName", ".MessageName")]
        [InlineData("MessageName", "MessageName")]
        // messageType = ["."] {ident "."} messageName
        public void MessageType_is_OptionalDot_Repeated_DotSeperatedIdentifier_and_Identifier(string input, string expected)
        {
            Assert.Equal(expected, _sys.MessageType.Parse(input));
        }

        [Theory]
        [InlineData(".")]
        // messageType = ["."] {ident "."} messageName
        public void MessageType_Throws_when_MissingIdentifier(string input)
        {
            Assert.Throws<ParseException>(() => _sys.MessageType.Parse(input));

        }

        [Theory]
        [InlineData(".IdentA.IdentB.EnumName", ".IdentA.IdentB.EnumName")]
        [InlineData("IdentA.IdentB.EnumName", "IdentA.IdentB.EnumName")]
        [InlineData(".EnumName", ".EnumName")]
        [InlineData("EnumName", "EnumName")]
        // enumType = ["."] {ident "."} enumName
        public void EnumType_is_OptionalDot_Repeated_DotSeperatedIdentifier_and_Identifier(string input, string expected)
        {
            Assert.Equal(expected, _sys.EnumType.Parse(input));
        }

        [Theory]
        [InlineData(".")]
        // enumType = ["."] {ident "."} enumName
        public void EnumType_Throws_when_noIdentifier(string input)
        {
            Assert.Throws<ParseException>(() => _sys.EnumType.Parse(input));
        }

        [Theory]
        [InlineData("Aab_z34 ", "Aab_z34")]
        // groupName = capital { letter | unicodeDigit | "_" }
        public void GroupName_is_CapLetter_then_OptionalSeries_of_Letter_Digit_Underscore(string input, string expected)
        {
            Assert.Equal(expected, _sys.GroupName.Parse(input));
        }

        [Theory]
        [InlineData("aab_z34 ")]
        // groupName = capital { letter | unicodeDigit | "_" }
        public void GroupName_Throws_when_noStartingUpperCase(string input)
        {
            Assert.Throws<ParseException>(() => _sys.GroupName.Parse(input));
        }

        [Theory]
        [InlineData("1", "1")]
        [InlineData("19", "19")]
        [InlineData("07", "07")]
        [InlineData("0x9", "0x9")]
        // intLit     = decimalLit | octalLit | hexLit
        public void IntegerLiteral_is_Decimal_Octal_or_Hex_Literal(string input, string expected)
        {
            Assert.Equal(expected, _sys.IntegerLiteral.Parse(input));
        }

        [Theory]
        [InlineData("0x9", "0x9")]
        [InlineData("0X9", "0X9")]
        [InlineData("0x96FA", "0x96FA")]
        // hexLit     = "0" ( "x" | "X" ) hexDigit { hexDigit } 
        public void HexLiteral_is_0_XignoreCase_HexDigit_Then_ZeroOrMany_HexDigits(string input, string expected)
        {
            Assert.Equal(expected, _sys.HexLiteral.Parse(input));
        }

        [Theory]
        [InlineData("0x96G")]
        [InlineData("0x")]
        // hexLit     = "0" ( "x" | "X" ) hexDigit { hexDigit } 
        public void HexLiteral_ThrowsOn_NonHexChar_or_NoHexChar(string input)
        {
            Assert.Throws<ParseException>(() => _sys.HexLiteral.Parse(input));
        }

        [Theory]
        [InlineData("e+12", "e+12")]
        [InlineData("e-12", "e-12")]
        [InlineData("E+12", "E+12")]
        [InlineData("e12", "e+12")]
        // exponent  = ( "e" | "E" ) [ "+" | "-" ]decimals 
        public void Exponent_is_EignoreCase_then_Optional_Plus_or_Minus_then_Decimals(string input, string expected)
        {
            Assert.Equal(expected, _sys.Exponent.Parse(input));
        }

        [Theory]
        [InlineData("e-a")]
        [InlineData("G+12")]
        // exponent  = ( "e" | "E" ) [ "+" | "-" ]decimals 
        public void Exponent_Throws_on_NonNumericExp_or_NonEBase(string input)
        {
            Assert.Throws<ParseException>(() => _sys.Exponent.Parse(input));
        }

        [Theory]
        [InlineData("1.2", "1.2")]
        [InlineData("1.2e+10", "1.2e+10")]
        [InlineData("1.2e10", "1.2e+10")]
        // decimals "." [ decimals ] [ exponent ] | decimals exponent | "."decimals [exponent ]
        public void FloatLiteral_is_Digits_dot_OptionalDigits_OptionalExp_or_Decimals_and_Exp_or_dot_and_decimals_and_ExponentOpt(string input, string expected)
        {
            Assert.Equal(expected, _sys.FloatLiteral.Parse(input));
        }

        [Theory]
        [InlineData("1.2e10g")]
        // decimals "." [ decimals ] [ exponent ] | decimals exponent | "."decimals [exponent ]
        public void FloatLiteral_Throws_on_LetterInNumeric(string input)
        {
            Assert.Throws<ParseException>(() => _sys.FloatLiteral.Parse(input));
        }

        [Theory]
        [InlineData("true", "true")]
        [InlineData("false", "false")]
        // boolLit = "true" | "false"
        public void BooleanLiteral_can_be_True_or_False(string input, string expected)
        {
            Assert.Equal(expected, _sys.BooleanLiteral.Parse(input));
        }

        [Theory]
        [InlineData("TRUE")]
        [InlineData("True")]
        [InlineData("treu")]
        // boolLit = "true" | "false"
        public void BooleanLiteral_Throws_on_Bad_Casing_or_Spelling(string input)
        {
            Assert.Throws<ParseException>(() => _sys.BooleanLiteral.Parse(input));
        }

        [Theory]
        [InlineData(";", ";")]
        // emptyStatement = ";"
        public void EmptyStatement_is_a_SemiColon(string input, string expected)
        {
            Assert.Equal(expected, _sys.EmptyStatement.Parse(input));
        }

        [Theory]
        [InlineData("")]
        // emptyStatement = ";"
        public void EmptyStatement_isnt_Empty_Rly(string input)
        {
            Assert.Throws<ParseException>(() => _sys.EmptyStatement.Parse(input));
        }

        [Theory]
        [InlineData("'", '\'')]
        [InlineData("\"", '\"')]
        // Quote = '"' or "'"
        public void Quote_is_Single_Or_Double_Quote(string input, char expected)
        {
            Assert.Equal(expected, _sys.Quote.Parse(input));
        }

        [Theory]
        [InlineData(";")]
        // Quote = '"' or "'"
        public void Quote_Throws_on_NonQuote(string input)
        {
            Assert.Throws<ParseException>(() => _sys.Quote.Parse(input));
        }

        [Theory]
        [InlineData("\\x09", "\\x09")]
        [InlineData("\\x0F", "\\x0F")]
        [InlineData("\\X0F", "\\X0F")]
        // hexEscape = `\` ("x" | "X") hexDigit hexDigit
        public void HexEscape_is_Backslash_XignoreCase_Two_HexDigits(string input, string expected)
        {
            Assert.Equal(expected, _sys.HexEscape.Parse(input));
        }

        [Theory]
        [InlineData("X0F")]
        // hexEscape = `\` ("x" | "X") hexDigit hexDigit
        public void HexEscape_Throws_when_NotEscaped(string input)
        {
            Assert.Throws<ParseException>(() => _sys.HexEscape.Parse(input));
        }

        [Theory]
        [InlineData("\\435", "\\435")]
        // octEscape = `\` octalDigit octalDigit octalDigit
        public void OctEscape_is_Backslash_Three_OctalDigits(string input, string expected)
        {
            Assert.Equal(expected, _sys.OctalEscape.Parse(input));
        }

        [Theory]
        [InlineData("435")]
        [InlineData("\\439")]
        [InlineData("\\43A")]
        // octEscape = `\` octalDigit octalDigit octalDigit
        public void OctEscape_Throws_when_InvalidOctal_or_NotEscaped(string input)
        {
            Assert.Throws<ParseException>(() => _sys.OctalEscape.Parse(input));
        }

        [Theory]
        [InlineData("\\a", "\\a")]
        [InlineData("\\\\", "\\\\")]
        [InlineData("\\\"", "\\\"")]
        // charEscape = `\` ( "a" | "b" | "f" | "n" | "r" | "t" | "v" | `\` | "'" | `"` )
        public void CharEscape_is_Backslash_abfnrtw_backslash_or_single_or_double_Quote(string input, string expected)
        {
            Assert.Equal(expected, _sys.CharEscape.Parse(input));
        }

        [Theory]
        [InlineData("a\\")]
        // charEscape = `\` ( "a" | "b" | "f" | "n" | "r" | "t" | "v" | `\` | "'" | `"` )
        public void CharEscape_Throws_on_NonEscaped(string input)
        {
            Assert.Throws<ParseException>(() => _sys.CharEscape.Parse(input));
        }

        [Theory]
        [InlineData("\\a", "\\a")]
        [InlineData("\\\\", "\\\\")]
        [InlineData("\\\"", "\\\"")]
        [InlineData("\\435", "\\435")]
        [InlineData("\\x09", "\\x09")]
        [InlineData("a", "a")]
        // charValue = hexEscape | octEscape | charEscape | /[^\0\n\\]/
        public void CharValue_is_Either_HexEscape_OctEscape_CharEscape_OrStringFirstChar(string input, string expected)
        {
            Assert.Equal(expected, _sys.CharValue.Parse(input));
        }

        [Theory]
        [InlineData("\"astring\"", "astring")]
        [InlineData("'astring'", "astring")]
        // strLit = ("`" { charValue } "`") |  (`"` { charValue } `"`)
        public void StringLiteral_is_oneOrMore_CharValue_between_Double_or_Single_Quotes(string input, string expected)
        {
            Assert.Equal(expected, _sys.StringLiteral.Parse(input));
        }

        [Theory]
        [InlineData("'astring")]
        [InlineData("astring'")]
        // strLit = ("`" { charValue } "`") |  (`"` { charValue } `"`)
        public void StringLiteral_Throws_when_MissingQuote(string input)
        {
            Assert.Throws<ParseException>(() => _sys.StringLiteral.Parse(input));
        }

        [Theory]
        [InlineData("32", 32)]
        [InlineData("0", 0)]
        // fieldNumber = intLit
        public void FieldNumber_is_IntegerLiteral(string input, int expected)
        {
            Assert.Equal(expected, _sys.FieldNumber.Parse(input));
        }
    }
}
