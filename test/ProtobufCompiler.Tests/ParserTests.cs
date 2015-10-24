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


    }
}
