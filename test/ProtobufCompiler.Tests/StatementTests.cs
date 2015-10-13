using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using ProtobufCompiler.Compiler;
using ProtobufCompiler.Interfaces;
using Xunit;

namespace ProtobufCompiler.Tests
{
    public class StatementTests
    {
        private readonly ISyntaxAnalyzer _sys;

        private readonly string _data = @"message Outer {" + Environment.NewLine +
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
                                        "}";

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
        internal void ShouldFindFourTopLevelStatements()
        {
            _sys.Analyze();
            _sys.Statements.Count.Should().Be(4, "because there are only four statements in this .proto definition. ");
        }
    }
}
