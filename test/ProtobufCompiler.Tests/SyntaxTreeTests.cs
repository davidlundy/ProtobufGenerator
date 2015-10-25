using System;
using System.Collections.Generic;
using FluentAssertions;
using ProtobufCompiler.Compiler;
using ProtobufCompiler.Compiler.Nodes;
using ProtobufCompiler.Interfaces;
using Xunit;

namespace ProtobufCompiler.Tests
{
    public class SyntaxTreeTests
    {
        private const string BecauseObjectGraphsEqual = "because Node class implements IComparable<Node> and the object graphs should be equal.";
        private ISyntaxAnalyzer _sys;

        [Fact]
        public void ShouldBuildMultilineComment()
        {
            // Arrange Input
            var tokenList = new List<Token>
            {
                new Token(TokenType.Comment, 0, 0, "\\*"),
                new Token(TokenType.String, 0, 5, "This"),
                new Token(TokenType.String, 0, 6, "is"),
                new Token(TokenType.String, 0, 7, "a"),
                new Token(TokenType.String, 0, 8, "comment."),
                new Token(TokenType.EndLine, 0, 9, Environment.NewLine),
                new Token(TokenType.String, 0, 5, "This"),
                new Token(TokenType.String, 0, 6, "is"),
                new Token(TokenType.String, 0, 7, "a"),
                new Token(TokenType.String, 0, 8, "second"),
                new Token(TokenType.String, 0, 8, "line."),
                new Token(TokenType.Comment, 0, 0, "*\\"),
                new Token(TokenType.EndLine, 0, 4, Environment.NewLine)
            };

            // Arrange Output
            var root = new RootNode();
            var comment = new Node(NodeType.Comment,"\\*");
            // Bit of an issue here, notice the spaces around the NewLine, we'd like to make that go away.
            var text = "This is a comment. " + Environment.NewLine + " This is a second line.";
            var commentText = new Node(NodeType.CommentText, text);

            comment.AddChild(commentText);
            root.AddChild(comment);

            // Act
            _sys = new SyntaxAnalyzer(new Queue<Token>(tokenList));
            var result = _sys.Analyze();

            // Assert
            result.Should().Be(root, BecauseObjectGraphsEqual);

        }

        [Fact]
        public void ShouldBuildSyntaxNodes()
        {
            // Arrange Input
            var tokenList = new List<Token>
            {
                new Token(TokenType.Id, 0, 0, "syntax"),
                new Token(TokenType.Control, 0, 1, "="),
                new Token(TokenType.String, 0, 2, "\"proto3\""),
                new Token(TokenType.Control, 0, 3, ";"),
                new Token(TokenType.EndLine, 0, 4, Environment.NewLine)
            };

            // Arrange Output
            var root = new RootNode();
            var syntax = new Node(NodeType.Syntax, "syntax");
            var proto3 = new Node(NodeType.StringLiteral, "proto3");
            syntax.AddChild(proto3);
            root.AddChild(syntax);

            // Act
            _sys = new SyntaxAnalyzer(new Queue<Token>(tokenList));
            var result = _sys.Analyze();

            // Assert
            result.Should().Be(root, BecauseObjectGraphsEqual);
        }

        [Fact]
        public void ShouldBuildImportNode()
        {
            // Arrange Input
            var tokenList = new List<Token>
            {
                new Token(TokenType.Id, 0, 0, "import"),
                new Token(TokenType.String, 0, 1, "\"other.message\""),
                new Token(TokenType.Control, 0, 2, ";"),
                new Token(TokenType.EndLine, 0, 4, Environment.NewLine)
            };

            // Arrange Output
            var root = new RootNode();
            var import = new Node(NodeType.Import, "import");
            var otherMessage = new Node(NodeType.StringLiteral, "other.message");
            import.AddChild(otherMessage);
            root.AddChild(import);

            // Act
            _sys = new SyntaxAnalyzer(new Queue<Token>(tokenList));
            var result = _sys.Analyze();

            // Assert
            result.Should().Be(root, BecauseObjectGraphsEqual);
        }

        [Fact]
        public void ShouldBuildInlineStatementWithTrailingComment()
        {
            // Arrange Input
            var tokenList = new List<Token>
            {
                new Token(TokenType.Id, 0, 0, "syntax"),
                new Token(TokenType.Control, 0, 1, "="),
                new Token(TokenType.String, 0, 2, "\"proto3\""),
                new Token(TokenType.Control, 0, 3, ";"),
                new Token(TokenType.Comment, 0, 4, "\\\\"),
                new Token(TokenType.String, 0, 5, "This"),
                new Token(TokenType.String, 0, 6, "is"),
                new Token(TokenType.String, 0, 7, "a"),
                new Token(TokenType.String, 0, 8, "comment."),
                new Token(TokenType.EndLine, 0, 9, Environment.NewLine)
            };

            // Arrange Output
            var root = new RootNode();
            var syntax = new Node(NodeType.Syntax, "syntax");
            var proto3 = new Node(NodeType.StringLiteral, "proto3");
            var comment = new Node(NodeType.Comment, "\\\\");
            var commentText = new Node(NodeType.CommentText, "This is a comment.");
            comment.AddChild(commentText);
            syntax.AddChild(proto3);
            syntax.AddChild(comment);
            root.AddChild(syntax);

            // Act
            _sys = new SyntaxAnalyzer(new Queue<Token>(tokenList));
            var result = _sys.Analyze();

            // Assert
            result.Should().Be(root, BecauseObjectGraphsEqual);
        }
    }
}
