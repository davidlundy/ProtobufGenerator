using System;
using System.Net.Sockets;
using Xunit;
using ProtobufCompiler.Types;
using Sprache;

namespace ProtobufCompiler.Tests
{
    public class ParseTests
    {
        private readonly ProtoGrammar _sys;

        public ParseTests()
        {
            _sys = new ProtoGrammar();
        }

        [Theory]
        [InlineData("syntax = \"proto3\";", "proto3")]
        [InlineData("syntax = \"proto2\";", "proto2")]
        // syntax = "syntax" "=" quote "proto3" quote ";"
        public void Parse_Syntax_Declaration(string input, string expected)
        {
            var syntax = _sys.Syntax.Parse(input);
            Assert.Equal(syntax, new Syntax(expected));
        }

        [Theory]
        [InlineData("syntax = \"proto\";")]
        [InlineData("syntax \"proto\";")]
        // syntax = "syntax" "=" quote "proto3" quote ";"
        public void Parse_Syntax_Declaration_Throws_on_BadGrammar(string input)
        {
            Assert.Throws<ArgumentException>(() => _sys.Syntax.Parse(input));
        }

        [Theory]
        [InlineData("import public Test.Proto.OtherClass", "public", "Test.Proto.OtherClass")]
        [InlineData("import public Test.Proto.OtherClass", "", "Test.Proto.OtherClass")]
        [InlineData("import public Test.Proto.OtherClass", "None", "Test.Proto.OtherClass")]
        //import = "import" [ "weak" | “public”] strLit ";" 
        public void Parse_Import_Declaration(string input, string expMod, string expValue)
        {
            Assert.Equal(new Import(expMod, expValue), _sys.Import.Parse(input));
        }

        [Theory]
        [InlineData("package Test.Proto.Classes;", "Test.Proto.Classes")]
        // package = "package" fullIdent ";"
        public void Parse_Package_Declaration(string input, string expected)
        {
            Assert.Equal(new Package(expected), _sys.Package.Parse(input));
        }

        [Theory]
        [InlineData("option SOME_KEY = SOME_VALUE;", "SOME_KEY", "SOME_VALUE")]
        // option = "option" optionName  "=" constant ";"
        // optionName = (ident | "(" fullIdent ")") {"." ident}
        public void Parse_Option_Declaration(string input, string expKey, string expValue)
        {
            Assert.Equal(new Option(expKey, expValue), _sys.Option.Parse(input));
        }
    }
}
