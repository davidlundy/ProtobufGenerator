using ProtobufCompiler.Compiler;
using ProtobufCompiler.Compiler.Errors;
using ProtobufCompiler.Compiler.Nodes;
using System;
using System.Collections.Generic;
using Xunit;

namespace ProtobufCompiler.Tests
{
    public partial class SyntaxTreeTests
    {
        [Fact]
        public void ShouldErrorInvalidTopLevelStatementAndKeepParsing()
        {
            #region Arrange Invalid Token Input

            var badToken = new Token(TokenType.String, 5, 5, "for");
            var tokenList = new List<Token>
            {
                badToken,
                // and the keep parsing part
                new Token(TokenType.EndLine, 0, 4, Environment.NewLine),
                new Token(TokenType.Id, 0, 0, "syntax"),
                new Token(TokenType.Control, 0, 1, "="),
                new Token(TokenType.String, 0, 2, "\"proto3\""),
                new Token(TokenType.Control, 0, 3, ";"),
                new Token(TokenType.EndLine, 0, 4, Environment.NewLine)
            };

            #endregion Arrange Invalid Token Input

            #region Arrange Expected NodeTree Output

            // Verify we kept parsing.
            var root = new RootNode();
            var errors = new[] { new ParseError("Found an invalid top level statement at token ", badToken) };
            var syntax = new Node(NodeType.Syntax, "syntax");
            var proto3 = new Node(NodeType.StringLiteral, "proto3");
            syntax.AddChild(proto3);
            root.AddErrors(errors);
            root.AddChild(syntax);

            #endregion Arrange Expected NodeTree Output

            AssertSyntax(tokenList, root);
        }

        [Fact]
        public void ShouldErrorWhenSyntaxMissingEquals()
        {
            #region Arrange Syntax Declaration Token Input

            var errorToken = new Token(TokenType.Control, 3, 3, ";");
            var tokenList = new List<Token>
            {
                new Token(TokenType.Id, 0, 0, "syntax"),
                errorToken,
                new Token(TokenType.String, 0, 2, "\"proto3\""),
                new Token(TokenType.Control, 0, 3, ";"),
                new Token(TokenType.EndLine, 0, 4, Environment.NewLine)
            };

            #endregion Arrange Syntax Declaration Token Input

            #region Arrange Expected NodeError Output

            var root = new RootNode();
            root.AddErrors(new[] { new ParseError("Expected an assignment after syntax token, found ", errorToken) });

            #endregion Arrange Expected NodeError Output

            AssertSyntax(tokenList, root);
        }

        [Fact]
        public void ShouldErrorWhenSyntaxNotQuoted()
        {
            #region Arrange Syntax Declaration Token Input

            var errorToken = new Token(TokenType.String, 0, 2, "proto3");
            var tokenList = new List<Token>
            {
                new Token(TokenType.Id, 0, 0, "syntax"),
                new Token(TokenType.Control, 0, 1, "="),
                errorToken,
                new Token(TokenType.Control, 0, 3, ";"),
                new Token(TokenType.EndLine, 0, 4, Environment.NewLine)
            };

            #endregion Arrange Syntax Declaration Token Input

            #region Arrange Expected NodeError Output

            var root = new RootNode();
            root.AddErrors(new[] { new ParseError("Expected a string literal after syntax assignment, found token ", errorToken) });

            #endregion Arrange Expected NodeError Output

            AssertSyntax(tokenList, root);
        }

        [Fact]
        public void ShouldErrorWhenStatementNotTerminatedWithSemicolon()
        {
            #region Arrange Syntax Declaration Token Input
            var errorToken = new Token(TokenType.Control, 0, 3, ":"); // Colon, not semicolon!
            var tokenList = new List<Token>
            {
                new Token(TokenType.Id, 0, 0, "syntax"),
                new Token(TokenType.Control, 0, 1, "="),
                new Token(TokenType.String, 0, 2, "\"proto3\""),
                errorToken,
                new Token(TokenType.EndLine, 0, 4, Environment.NewLine)
            };

            #endregion Arrange Syntax Declaration Token Input

            #region Arrange Expected NodeTree Output

            var root = new RootNode();
            root.AddErrors(new[] { new ParseError("Required terminator is missing for statement. Found token ", errorToken) });

            #endregion Arrange Expected NodeTree Output

            AssertSyntax(tokenList, root);
        }

        [Fact]
        public void ShouldErrorWhenImportNotQuoted()
        {
            #region Arrange Import Declaration Token Input
            var errorToken = new Token(TokenType.String, 0, 1, "other.message");
            var tokenList = new List<Token>
            {
                new Token(TokenType.Id, 0, 0, "import"),
                errorToken,
                new Token(TokenType.Control, 0, 2, ";"),
                new Token(TokenType.EndLine, 0, 4, Environment.NewLine)
            };

            #endregion Arrange Import Declaration Token Input

            #region Arrange Expected NodeTree Output

            var root = new RootNode();
            root.AddErrors(new[] { new ParseError("Could not find a string literal for syntax analyzer ImportStatementAnalyzer on token ", errorToken) });

            #endregion Arrange Expected NodeTree Output

            AssertSyntax(tokenList, root);
        }
    }
}
