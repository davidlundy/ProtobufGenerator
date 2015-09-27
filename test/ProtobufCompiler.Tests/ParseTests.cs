using System;
using NUnit.Framework;
using ProtobufCompiler.Types;
using Sprache;

namespace ProtobufCompiler.Tests
{
    [TestFixture]
    public class ParseTests
    {
        private ProtoGrammar _sys;

        [SetUp]
        public void Setup()
        {
            _sys = new ProtoGrammar();
        }

        [TestCase("syntax = \"proto3\";","proto3")]
        [TestCase("syntax = \"proto2\";", "proto2")]
        [TestCase("syntax = \"proto\";", "willthrow", ExpectedException = typeof(ArgumentException))]
        [TestCase("syntax \"proto\";", "willthrow", ExpectedException = typeof(ParseException))]
        // syntax = "syntax" "=" quote "proto3" quote ";"
        public void Parse_Syntax_Declaration(string input, string expected)
        {
            var syntax = _sys.Syntax.Parse(input);
            Assert.That(syntax, Is.EqualTo(new Syntax(expected)));
        }

        // import = "import" [ "weak" | “public”] strLit ";" 
        public void Parse_Import_Declaration(string input, string expMod, string expValue)
        {
            var import = _sys.Import.Parse(input);
            Assert.That(import, Is.EqualTo(new Import(expMod, expValue)));
        }

        [TestCase("package Test.Proto.Classes;", "Test.Proto.Classes")]
        // package = "package" fullIdent ";"
        public void Parse_Package_Declaration(string input, string expected)
        {
            var package = _sys.Package.Parse(input);
            Assert.That(package, Is.EqualTo(new Package(expected)));
        }

        // option = "option" optionName  "=" constant ";"
        // optionName = (ident | "(" fullIdent ")") {"." ident}
        public void Parse_Option_Declaration(string input, string expKey, string expValue)
        {
            var option = _sys.Option.Parse(input);
            Assert.That(option, Is.EqualTo(new Option(expKey, expValue)));
        }
    }
}
