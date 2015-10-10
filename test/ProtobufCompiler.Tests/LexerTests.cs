using System;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using ProtobufCompiler.Compiler;
using ProtobufCompiler.Interfaces;
using Xunit;

namespace ProtobufCompiler.Tests
{
    public class LexerTests
    {
        private readonly ILexicalAnalyzer _sys;
        private readonly string _data = @"message Outer {" + Environment.NewLine +
                                "option my_option = true; // some comment" + Environment.NewLine +
                                "message Inner {" + Environment.NewLine +
                                "int64 ival = 1;" + Environment.NewLine +
                                "}" + Environment.NewLine +
                                "map<int32, string> my_map = 2;" + Environment.NewLine +
                                "}";

        public LexerTests()
        {
            var memStream = new MemoryStream(Encoding.UTF8.GetBytes(_data));
            var reader = new StreamReader(memStream);
            var source = new Source(reader);
            _sys = new LexicalAnalyzer(source);
        }

        [Fact]
        public void ShouldTokenize37Tokens()
        {
            _sys.Tokenize();
            var tokens = _sys.TokenStream;
            tokens.Count.Should().Be(37, "because there are 37 tokens in the message definition, including EOLs");
        }

        [Fact]
        public void ShouldReturnTokenStream()
        {
            _sys.Tokenize();
            var tokenStream = _sys.TokenStream;
            tokenStream.Peek().Should().Be(new Token(TokenType.Id, 1, 1, "message"));
        }

        [Fact]
        public void ThereAreSixeEndlineTokens()
        {
            _sys.Tokenize();
            var tokenStream = _sys.TokenStream.Where(t => t.Type == TokenType.EndLine);
            tokenStream.Count().Should().Be(6, "because there are 7 lines in the data.");
        }

        [Fact]
        public void ThereAreFourIdTokens()
        {
            _sys.Tokenize();
            var tokenStream = _sys.TokenStream.Where(t => t.Type == TokenType.Id);
            tokenStream.Count().Should().Be(4, "because there are 4 types in the data, message, option, message, and map.");
        }

    }
}
