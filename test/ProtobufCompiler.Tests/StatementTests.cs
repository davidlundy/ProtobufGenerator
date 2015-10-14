using System;
using System.IO;
using System.Text;
using FluentAssertions;
using ProtobufCompiler.Compiler;
using ProtobufCompiler.Interfaces;
using Xunit;

namespace ProtobufCompiler.Tests
{
    public class StatementTests
    {
        private readonly ISyntaxAnalyzer _sys;

        private readonly string _data = "syntax = \"proto3\";" + Environment.NewLine+
                                        "import public \"other.proto\";"+Environment.NewLine+
                                        "option java_package = \"com.example.foo\";" + Environment.NewLine +
                                        "/*Let's have a block comment for"+Environment.NewLine+
                                        " the message*/"+Environment.NewLine+
                                        "message Outer {" + Environment.NewLine +
                                        "option my_option = true; // some comment" + Environment.NewLine +
                                        "message Inner {" + Environment.NewLine +
                                        "int64 ival = 1;" + Environment.NewLine +
                                        "}" + Environment.NewLine +
                                        "map<int32, string> my_map = 2;" + Environment.NewLine +
                                        "}" + Environment.NewLine +
                                        "option some_file_option = false;" + Environment.NewLine +
                                        "package Google.ProtocolBuffer.Test;" + Environment.NewLine +
                                        "message Data {" + Environment.NewLine +
                                        "int32 test = 1;" + Environment.NewLine +
                                        "}" + Environment.NewLine+ 
                                        "This is a new line that is invalid" + Environment.NewLine;

        public StatementTests()
        {
            var memStream = new MemoryStream(Encoding.UTF8.GetBytes(_data));
            var reader = new StreamReader(memStream);
            var source = new Source(reader);
            ILexicalAnalyzer lexer = new LexicalAnalyzer(source);
            lexer.Tokenize();
            _sys = new SyntaxAnalyzer(lexer.TokenStream);
        }

        [Fact]
        internal void ShouldFindEightTopLevelStatements()
        {
            _sys.Analyze();
            _sys.Statements.Count.Should().Be(8, "because there are only eight statements in this .proto definition. ");
        }

        [Fact]
        internal void ShouldFindOneParseError()
        {
            _sys.Analyze();
            _sys.Errors.Count.Should()
                .Be(1, "because there is an invalid top level statement in the .proto definition. ");
            // Note: We should not expect a throw. Keep parsing and find all necessary errors. 
        }
    }
}
