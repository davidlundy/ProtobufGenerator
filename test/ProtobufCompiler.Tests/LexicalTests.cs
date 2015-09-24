using System;
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

        /*
            ident = letter { letter | unicodeDigit | "_" }
            fullIdent = ident {"." ident}
            messageName = ident
            enumName = ident
            fieldName = ident
            oneofName = ident
            mapName = ident
            serviceName = ident
            rpcName = ident
            streamName = ident
            messageType = ["."] {ident "."} messageName
            enumType = ["."] {ident "."} enumName
            groupName = capital { letter | unicodeDigit | "_" }
        */

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

        
    }
}
