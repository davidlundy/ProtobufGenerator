using FluentAssertions;
using ProtobufCompiler.Compiler;
using Xunit;

namespace ProtobufCompiler.Tests
{
    public class ParserTests
    {
        private readonly Parser _sys;

        public ParserTests()
        {
            _sys = new Parser();
        }

        [Theory]
        [InlineData('0', true)]
        [InlineData('9', true)]
        [InlineData('h', false)]
        [InlineData('*', false)]
        // decimalDigit = "0" … "9"
        public void DecimalDigitis0To9(char input, bool expected)
        {
            var output = _sys.IsDecimalDigit(input);
            output.Should().Be(expected, $"Because {input} is{(expected ? string.Empty : " not ")}a valid Decimal Digit");
        }

        [Theory]
        [InlineData('0', true)]
        [InlineData('9', true)]
        [InlineData('A', true)]
        [InlineData('F', true)]
        [InlineData('a', true)]
        [InlineData('f', true)]
        [InlineData('G', false)]
        [InlineData('h', false)]
        [InlineData('*', false)]
        // hexDigit     = "0" … "9" | "A" … "F" | "a" … "f"
        public void HexDigitis0To9AndAtoFCaseInsensitive(char input, bool expected)
        {
            var output = _sys.IsHexDigit(input);
            output.Should().Be(expected, $"Because {input} is{(expected ? string.Empty : " not ")}a valid Hex Digit");
        }


        [Theory]
        [InlineData('0', true)]
        [InlineData('7', true)]
        [InlineData('8', false)]
        [InlineData('A', false)]
        // octalDigit   = "0" … "7"
        public void OctalDigitis0To7(char input, bool expected)
        {
            var output = _sys.IsOctalDigit(input);
            output.Should().Be(expected, $"Because {input} is{(expected ? string.Empty : " not ")}a valid Octal Digit");
        }

        [Theory]
        [InlineData('a', true)]
        [InlineData('Z', true)]
        [InlineData('8', false)]
        [InlineData('*', false)]
        //letter = "A" … "Z" | "a" … "z"
        public void LetterIsAtoZCaseInsensitive(char input, bool expected)
        {
            var output = _sys.IsLetter(input);
            output.Should().Be(expected, $"Because {input} is{(expected ? string.Empty : " not ")}a valid Letter");
        }

        [Theory]
        [InlineData('A', true)]
        [InlineData('Z', true)]
        [InlineData('a', false)]
        [InlineData('8', false)]
        // capitalLetter =  "A" … "Z"
        public void CapitalIsAtoZ(char input, bool expected)
        {
            var output = _sys.IsCapital(input);
            output.Should().Be(expected, $"Because {input} is{(expected ? string.Empty : " not ")}a valid Capital");
        }

        [Theory]
        [InlineData("A", true)]
        [InlineData("a", true)]
        [InlineData("1", true)]
        [InlineData("_", true)]
        [InlineData(" ", false)]
        [InlineData(".", false)]
        // ident = letter { letter | unicodeDigit | "_" }
        public void IdentifierCharIsLetterNumberOrUnderscore(char input, bool expected)
        {
            var output = _sys.IsIdentifierChar(input);
            output.Should().Be(expected, $"Because {input} is{(expected ? string.Empty : " not ")}a valid Identifier Character");
        }

        [Theory]
        [InlineData("Aab_z34", true)]
        [InlineData("A", true)]
        [InlineData("A asdf", false)]
        [InlineData("Aab%z34 ", false)]
        // ident = letter { letter | unicodeDigit | "_" }
        public void IdentifierisLetterThenOptionalManyIdentifierChar(string input, bool expected)
        {
            var output = _sys.IsIdentifier(input);
            output.Should().Be(expected, $"Because {input} is{(expected ? string.Empty : " not ")}a valid Identifier");
        }

        [Theory]
        [InlineData("Aab_z34.Aa_49a", true)]
        [InlineData("A", true)]
        [InlineData("A asdf", false)]
        [InlineData("Aab%z34 ", false)]
        [InlineData("Aab_z34.", false)]
        [InlineData(".Aab_z3h", false)]
        [InlineData("Aab._z3h", false)]
        public void FullIdentifierIsIdentifierThenOptionalDotIdentifier(string input, bool expected)
        {
            var output = _sys.IsFullIdentifier(input);
            output.Should().Be(expected, $"because {input} is a valid Full Identifier");
        }

        [Theory]
        [InlineData(".IdentA.IdentB.Message_Name", true)]
        [InlineData("IdentA.IdentB.Message_Name", true)]
        [InlineData(".MessageName", true)]
        [InlineData("MessageName", true)]
        [InlineData(".MessageName.", false)]
        [InlineData("MessageName.", false)]
        public void MessageOrEnumTypeIsOptionalDotThenRepeatedDotSeparatedIdentifierThenIdentifier(string input, bool expected)
        {
            var output = _sys.IsNamespacedIdentifier(input);
            output.Should().Be(expected, $"because {input} is a valid MessageType");
        }

        [Theory]
        [InlineData(";", true)]
        [InlineData(";a", false)]
        [InlineData("", false)]
        public void SemicolonIsEmptyStatement(string input, bool expected)
        {
            var output = _sys.IsEmptyStatement(input);
            output.Should().Be(expected, $"because {input} is{(expected ? string.Empty : " not ")}a valid Empty Statement");
        }

        [Theory]
        [InlineData("\\\\", true)]
        [InlineData("\\", false)]
        [InlineData("\\*", false)]
        public void InlineCommentIsDoubleBackslash(string input, bool expected)
        {
            var output = _sys.IsInlineComment(input);
            output.Should().Be(expected, $"because {input} is{(expected ? string.Empty : " not ")}a \\\\");
        }

        [Theory]
        [InlineData("\\\\", false)]
        [InlineData("\\*", true)]
        [InlineData("*\\", false)]
        public void MultiLineCommentOpenIsBackslashAsterisk(string input, bool expected)
        {
            var output = _sys.IsMultiLineCommentOpen(input);
            output.Should().Be(expected, $"because {input} is{(expected ? string.Empty : " not ")}a \\*");
        }

        [Theory]
        [InlineData("\\\\", false)]
        [InlineData("\\*", false)]
        [InlineData("*\\", true)]
        [InlineData("A", false)]
        public void MultiLineCommentCloseIsAsteriskBackslash(string input, bool expected)
        {
            var output = _sys.IsMultiLineCommentClose(input);
            output.Should().Be(expected, $"because {input} is{(expected ? string.Empty : " not ")}a *\\");
        }

        [Theory]
        [InlineData("syntax", true)]
        [InlineData("Syntax", false)]
        public void SyntaxTag(string input, bool expected)
        {
            var output = _sys.IsSyntax(input);
            output.Should().Be(expected, $"because {input} is{(expected ? string.Empty : " not ")} \"syntax\"");
        }

        [Theory]
        [InlineData("import", true)]
        public void ImportTag(string input, bool expected)
        {
            var output = _sys.IsImport(input);
            output.Should().Be(expected, $"because {input} is{(expected ? string.Empty : " not ")} \"import\"");
        }

        [Theory]
        [InlineData("weak", true)]
        [InlineData("public", true)]
        [InlineData("private", false)]
        public void ImportModifierIsWeakOrPublic(string input, bool expected)
        {
            var output = _sys.IsImportModifier(input);
            output.Should()
                .Be(expected, $"because {input} is{(expected ? string.Empty : " not ")} a valid import modifier.");
        }

        [Theory]
        [InlineData("package", true)]
        public void PackageTag(string input, bool expected)
        {
            var output = _sys.IsPackage(input);
            output.Should().Be(expected, $"because {input} is{(expected ? string.Empty : " not ")} \"package\"");
        }

        [Theory]
        [InlineData("option", true)]
        public void OptionTag(string input, bool expected)
        {
            var output = _sys.IsOption(input);
            output.Should().Be(expected, $"because {input} is{(expected ? string.Empty : " not ")} \"option\"");
        }

        [Theory]
        [InlineData("enum", true)]
        public void EnumTag(string input, bool expected)
        {
            var output = _sys.IsEnum(input);
            output.Should().Be(expected, $"because {input} is{(expected ? string.Empty : " not ")} \"enum\"");
        }

        [Theory]
        [InlineData("service", true)]
        public void ServiceTag(string input, bool expected)
        {
            var output = _sys.IsService(input);
            output.Should().Be(expected, $"because {input} is{(expected ? string.Empty : " not ")} \"service\"");
        }

        [Theory]
        [InlineData("message", true)]
        public void MessageTag(string input, bool expected)
        {
            var output = _sys.IsMessage(input);
            output.Should().Be(expected, $"because {input} is{(expected ? string.Empty : " not ")} \"message\"");
        }

        [Theory]
        [InlineData("=", true)]
        public void AssignmentIsEqualSign(string input, bool expected)
        {
            var output = _sys.IsAssignment(input);
            output.Should().Be(expected, $"because {input} is{(expected ? string.Empty : " not ")} \"=\"");
        }

        [Theory]
        [InlineData("repeated", true)]
        [InlineData("repeat", false)]
        public void RepeatedTagIsRepeated(string input, bool expected)
        {
            var output = _sys.IsRepeated(input);
            output.Should().Be(expected, $"because {input} is{(expected ? string.Empty : " not ")} \"repeated\"");
        }

        [Theory]
        [InlineData("double", true)]
        [InlineData("float", true)]
        [InlineData("int32", true)]
        [InlineData("int64", true)]
        [InlineData("uint32", true)]
        [InlineData("uint64", true)]
        [InlineData("sint32", true)]
        [InlineData("sint64", true)]
        [InlineData("fixed32", true)]
        [InlineData("fixed64", true)]
        [InlineData("sfixed32", true)]
        [InlineData("sfixed64", true)]
        [InlineData("bool", true)]
        [InlineData("string", true)]
        [InlineData("bytes", true)]
        [InlineData("mytype", false)]
        public void CanParseSimpleTypes(string input, bool expected)
        {
            var output = _sys.IsBasicType(input);
            output.Should().Be(expected, $"because {input} is{(expected ? string.Empty : " not ")} a valid simple type.");
        }

        [Theory]
        [InlineData("3", false)]
        [InlineData("a", false)]
        [InlineData("3.0", true)]
        [InlineData("0.3", true)]
        [InlineData("300.000", true)]
        [InlineData("3.0e23", true)]
        [InlineData("3.0e23a", false)]
        [InlineData("3.034e+2.23", true)]
        [InlineData("3.034E+2.23", true)]
        [InlineData("3.034e-2.23", true)]
        [InlineData("3.034e-2.23e+1.2", false)]
        public void CanParseFloatLiterals(string input, bool expected)
        {
            var output = _sys.IsFloatLiteral(input);
            output.Should()
                .Be(expected, $"because {input} is{(expected ? string.Empty : " not ")} a valid float literal.");
        }

    }
}
