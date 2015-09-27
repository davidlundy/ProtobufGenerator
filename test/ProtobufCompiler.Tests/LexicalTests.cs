using NUnit.Framework;
using Sprache;

namespace ProtobufCompiler.Tests
{
    [TestFixture]
    public class LexicalTests
    {
        private ProtoGrammar _sys;

        [SetUp]
        public void Setup()
        {
            _sys = new ProtoGrammar();
        }

        [TestCase("0")]
        [TestCase("9")]
        [TestCase("A")]
        [TestCase("F")]
        [TestCase("a")]
        [TestCase("f")]
        [TestCase("G", ExpectedException = typeof(ParseException))]
        [TestCase("h", ExpectedException = typeof(ParseException))]
        [TestCase("*", ExpectedException = typeof(ParseException))]
        // hexDigit     = "0" … "9" | "A" … "F" | "a" … "f"
        public void Lexical_HexDigit_is_0to9_and_AtoF_CaseInsensitive(string input)
        {
            var output = _sys.HexDigit.Parse(input);
            Assert.AreEqual(input[0], output);
        }

        [TestCase("0")]
        [TestCase("7")]
        [TestCase("8", ExpectedException = typeof(ParseException))]
        [TestCase("A", ExpectedException = typeof(ParseException))]
        // octalDigit   = "0" … "7"
        public void Lexical_OctalDigit_is_0to7(string input)
        {
            var output = _sys.OctalDigit.Parse(input);
            Assert.AreEqual(input[0], output);
        }

        [TestCase("Aab_z34 ", ExpectedResult = "Aab_z34")]
        [TestCase("A ", ExpectedResult = "A")]
        [TestCase("A asdf", ExpectedResult = "A")]
        [TestCase("3ab_z34 ", ExpectedException = typeof(ParseException))]
        [TestCase("Aab%z34 ", ExpectedResult = "Aab")]
        [TestCase("*ab%z34 ", ExpectedException = typeof(ParseException))]
        // ident = letter { letter | unicodeDigit | "_" }
        public string Lexical_Identifier_is_Letter_Then_LetterNumberOrUnderscore(string input)
        {
            return _sys.Identifier.Parse(input);
        }

        [TestCase(".Aab_z34", ExpectedResult = ".Aab_z34")]
        [TestCase(".A", ExpectedResult = ".A")]
        // fullIdent = ident {"." ident}
        public string Lexical_DotSeparatedIdentifier_is_Dot_and_Identifier(string input)
        {
            return _sys.DotSeparatedIdentifier.Parse(input);
        }

        [TestCase("Aab_z34", ExpectedResult = "Aab_z34")]
        [TestCase("Aab_z34.E4_2", ExpectedResult = "Aab_z34.E4_2")]
        [TestCase("Foo.Bar.Baz", ExpectedResult = "Foo.Bar.Baz")]
        // fullIdent = ident {"." ident}
        public string Lexical_FullIdentifier_is_Identifier_Plus_Multiple_DotSeperated_Identifiers(string input)
        {
            return _sys.FullIdentifier.Parse(input);
        }

        [TestCase(".IdentA.IdentB.MessageName", ExpectedResult = ".IdentA.IdentB.MessageName")]
        [TestCase("IdentA.IdentB.MessageName", ExpectedResult = "IdentA.IdentB.MessageName")]
        [TestCase(".MessageName", ExpectedResult = ".MessageName")]
        [TestCase("MessageName", ExpectedResult = "MessageName")]
        [TestCase(".", ExpectedException = typeof(ParseException))]
        // messageType = ["."] {ident "."} messageName
        public string Lexical_MessageType_is_OptionalDot_Repeated_DotSeperatedIdentifier_and_Identifier(string input)
        {
            return _sys.MessageType.Parse(input);
        }

        [TestCase(".IdentA.IdentB.EnumName", ExpectedResult = ".IdentA.IdentB.EnumName")]
        [TestCase("IdentA.IdentB.EnumName", ExpectedResult = "IdentA.IdentB.EnumName")]
        [TestCase(".EnumName", ExpectedResult = ".EnumName")]
        [TestCase("EnumName", ExpectedResult = "EnumName")]
        [TestCase(".", ExpectedException = typeof(ParseException))]
        // enumType = ["."] {ident "."} enumName
        public string Lexical_EnumType_is_OptionalDot_Repeated_DotSeperatedIdentifier_and_Identifier(string input)
        {
            return _sys.EnumType.Parse(input);
        }

        [TestCase("Aab_z34 ", ExpectedResult = "Aab_z34")]
        [TestCase("aab_z34 ", ExpectedException = typeof(ParseException))]
        // groupName = capital { letter | unicodeDigit | "_" }
        public string Lexical_GroupName_is_CapLetter_then_OptionalSeries_of_Letter_Digit_Underscore(string input)
        {
            return _sys.GroupName.Parse(input);
        }

        [TestCase("1", ExpectedResult = "1")]
        [TestCase("19", ExpectedResult = "19")]
        [TestCase("07", ExpectedResult = "07")]
        [TestCase("0x9", ExpectedResult = "0x9")]
        // intLit     = decimalLit | octalLit | hexLit
        public string Lexical_IntegerLiteral_is_Decimal_Octal_or_Hex_Literal(string input)
        {
            return _sys.IntegerLiteral.Parse(input);
        }

        [TestCase("0x9", ExpectedResult = "0x9")]
        [TestCase("0X9", ExpectedResult = "0X9")]
        [TestCase("0x96FA", ExpectedResult = "0x96FA")]
        [TestCase("0x96G", ExpectedException = typeof(ParseException))]
        [TestCase("0x", ExpectedException = typeof(ParseException))]
        // hexLit     = "0" ( "x" | "X" ) hexDigit { hexDigit } 
        public string Lexical_HexLiteral_is_0_XignoreCase_HexDigit_Then_ZeroOrMany_HexDigits(string input)
        {
            return _sys.HexLiteral.Parse(input);
        }

        [TestCase("e+12", ExpectedResult = "e+12")]
        [TestCase("e-12", ExpectedResult = "e-12")]
        [TestCase("E+12", ExpectedResult = "E+12")]
        [TestCase("e12", ExpectedResult = "e+12")]
        [TestCase("e-a", ExpectedException = typeof(ParseException))]
        [TestCase("G+12", ExpectedException = typeof(ParseException))]
        // exponent  = ( "e" | "E" ) [ "+" | "-" ]decimals 
        public string Lexical_Exponent_is_EignoreCase_then_Optional_Plus_or_Minus_then_Decimals(string input)
        {
            return _sys.Exponent.Parse(input);
        }

        [TestCase("1.2", ExpectedResult = "1.2")]
        [TestCase("1.2e+10", ExpectedResult = "1.2e+10")]
        [TestCase("1.2e10", ExpectedResult = "1.2e+10")]
        [TestCase("1.2e10g", ExpectedException = typeof(ParseException))]
        // decimals "." [ decimals ] [ exponent ] | decimals exponent | "."decimals [exponent ]
        public string Lexical_FloatLiteral_is_Digits_dot_OptionalDigits_OptionalExp_or_Decimals_and_Exp_or_dot_and_decimals_and_ExponentOpt(string input)
        {
            return _sys.FloatLiteral.Parse(input);
        }

        [TestCase("true", ExpectedResult = "true")]
        [TestCase("false", ExpectedResult = "false")]
        [TestCase("TRUE", ExpectedException = typeof(ParseException))]
        [TestCase("True", ExpectedException = typeof(ParseException))]
        [TestCase("treu", ExpectedException = typeof(ParseException))]
        // boolLit = "true" | "false"
        public string Lexical_BooleanLiteral_can_be_True_or_False(string input)
        {
            return _sys.BooleanLiteral.Parse(input);
        }

        [TestCase(";", ExpectedResult = ";")]
        [TestCase("", ExpectedException = typeof(ParseException))]
        // emptyStatement = ";"
        public string Lexical_EmptyStatement_is_a_SemiColon(string input)
        {
            return _sys.EmptyStatement.Parse(input);
        }

        [TestCase(";", ExpectedException = typeof(ParseException))]
        [TestCase("'", ExpectedResult = '\'')]
        [TestCase("\"", ExpectedResult = '\"')]
        // Quote = '"' or "'"
        public char Lexical_Quote_is_Single_Or_Double_Quote(string input)
        {
            return _sys.Quote.Parse(input);
        }

        [TestCase("\\x09", ExpectedResult = "\\x09")]
        [TestCase("\\x0F", ExpectedResult = "\\x0F")]
        [TestCase("\\X0F", ExpectedResult = "\\X0F")]
        [TestCase("X0F", ExpectedException = typeof(ParseException))]
        // hexEscape = `\` ("x" | "X") hexDigit hexDigit
        public string Lexical_HexEscape_is_Backslash_XignoreCase_Two_HexDigits(string input)
        {
            return _sys.HexEscape.Parse(input);
        }

        [TestCase("\\435", ExpectedResult = "\\435")]
        [TestCase("435", ExpectedException = typeof(ParseException))]
        [TestCase("\\439", ExpectedException = typeof(ParseException))]
        [TestCase("\\43A", ExpectedException = typeof(ParseException))]
        // octEscape = `\` octalDigit octalDigit octalDigit
        public string Lexical_OctEscape_is_Backslash_Three_OctalDigits(string input)
        {
            return _sys.OctalEscape.Parse(input);
        }

        [TestCase("\\a", ExpectedResult = "\\a")]
        [TestCase("\\\\", ExpectedResult = "\\\\")]
        [TestCase("\\\"", ExpectedResult = "\\\"")]
        [TestCase("a\\", ExpectedException = typeof(ParseException))]
        // charEscape = `\` ( "a" | "b" | "f" | "n" | "r" | "t" | "v" | `\` | "'" | `"` )
        public string Lexical_CharEscape_is_Backslash_abfnrtw_backslash_or_single_or_double_Quote(string input)
        {
            return _sys.CharEscape.Parse(input);
        }

        [TestCase("\\a", ExpectedResult = "\\a")]
        [TestCase("\\\\", ExpectedResult = "\\\\")]
        [TestCase("\\\"", ExpectedResult = "\\\"")]
        [TestCase("\\435", ExpectedResult = "\\435")]
        [TestCase("\\x09", ExpectedResult = "\\x09")]
        [TestCase("a", ExpectedResult = "a")]
        // charValue = hexEscape | octEscape | charEscape | /[^\0\n\\]/
        public string Lexical_CharValue_is_Either_HexEscape_OctEscape_CharEscape_OrStringFirstChar(string input)
        {
            return _sys.CharValue.Parse(input);
        }

        [TestCase("\"astring\"", ExpectedResult = "astring")]
        [TestCase("'astring'", ExpectedResult = "astring")]
        [TestCase("'astring", ExpectedException = typeof(ParseException))]
        // strLit = ("`" { charValue } "`") |  (`"` { charValue } `"`)
        public string Lexical_StringLiteral_is_oneOrMore_CharValue_between_Double_or_Single_Quotes(string input)
        {
            return _sys.StringLiteral.Parse(input);
        }

        // optionName = (ident | "(" fullIdent ")") {"." ident}
        // example : "com.example.foo"
        public string Lexical_OptionName_is_FullIdentifier(string input)
        {
            return _sys.OptionName.Parse(input);
        }
    }
}
