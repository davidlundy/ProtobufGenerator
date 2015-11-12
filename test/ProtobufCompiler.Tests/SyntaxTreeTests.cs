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

        private void AssertSyntax(IEnumerable<Token> tokenList, RootNode root)
        {
            // Act
            _sys = new SyntaxAnalyzer(new Queue<Token>(tokenList));
            var result = _sys.Analyze();

            // Assert
            result.Should().Be(root, BecauseObjectGraphsEqual);
        }

        [Fact]
        public void ShouldBuildMultilineComment()
        {
            #region Arrange Multiline Comment Token Input
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
            #endregion

            #region Arrange Expected NodeTree Output
            var root = new RootNode();
            var comment = new Node(NodeType.Comment,"\\*");
            // Bit of an issue here, notice the spaces around the NewLine, we'd like to make that go away.
            var text = "This is a comment. " + Environment.NewLine + " This is a second line.";
            var commentText = new Node(NodeType.CommentText, text);
            comment.AddChild(commentText);
            root.AddChild(comment);
            #endregion

            AssertSyntax(tokenList, root);
        }

        [Fact]
        public void ShouldBuildSyntaxNodes()
        {
            #region Arrange Syntax Declaration Token Input
            var tokenList = new List<Token>
            {

                new Token(TokenType.Id, 0, 0, "syntax"),
                new Token(TokenType.Control, 0, 1, "="),
                new Token(TokenType.String, 0, 2, "\"proto3\""),
                new Token(TokenType.Control, 0, 3, ";"),
                new Token(TokenType.EndLine, 0, 4, Environment.NewLine)
            };
            #endregion

            #region Arrange Expected NodeTree Output
            var root = new RootNode();
            var syntax = new Node(NodeType.Syntax, "syntax");
            var proto3 = new Node(NodeType.StringLiteral, "proto3");
            syntax.AddChild(proto3);
            root.AddChild(syntax);
            #endregion

            AssertSyntax(tokenList, root);
        }

        [Fact]
        public void ShouldBuildImportNode()
        {
            #region Arrange Import Declaration Token Input
            var tokenList = new List<Token>
            {
                new Token(TokenType.Id, 0, 0, "import"),
                new Token(TokenType.String, 0, 1, "\"other.message\""),
                new Token(TokenType.Control, 0, 2, ";"),
                new Token(TokenType.EndLine, 0, 4, Environment.NewLine)
            };
            #endregion

            #region Arrange Expected NodeTree Output
            var root = new RootNode();
            var import = new Node(NodeType.Import, "import");
            var otherMessage = new Node(NodeType.StringLiteral, "other.message");
            import.AddChild(otherMessage);
            root.AddChild(import);
            #endregion

            AssertSyntax(tokenList, root);
        }

        [Fact]
        public void ShouldBuildPackageNode()
        {
            #region Arrange Package Declaration Token Input
            var tokenList = new List<Token>
            {
                new Token(TokenType.Id, 0, 0, "package"),
                new Token(TokenType.String, 0, 1, "foo.bar.baz"),
                new Token(TokenType.Control, 0, 2, ";"),
                new Token(TokenType.EndLine, 0, 4, Environment.NewLine)
            };
            #endregion

            #region Arrange Expected NodeTree Output
            var root = new RootNode();
            var package = new Node(NodeType.Package, "package");
            var packageName = new Node(NodeType.Identifier, "foo.bar.baz");
            package.AddChild(packageName);
            root.AddChild(package);
            #endregion

            AssertSyntax(tokenList, root);
        }

        [Fact]
        public void ShouldBuildOptionNode()
        {
            #region Arrange Option Declaration Token Input
            var tokenList = new List<Token>
            {
                new Token(TokenType.Id, 0, 0, "option"),
                new Token(TokenType.String, 0, 1, "java_package"),
                new Token(TokenType.Control, 0, 2, "="),
                new Token(TokenType.String, 0, 3, "\"com.example.foo\""),
                new Token(TokenType.Control, 0, 4, ";"),
                new Token(TokenType.EndLine, 0, 5, Environment.NewLine)
            };
            #endregion

            #region Arrange Expected NodeTree Output
            var root = new RootNode();
            var option = new Node(NodeType.Option, "option");
            var optionName = new Node(NodeType.Identifier, "java_package");
            var optionValue = new Node(NodeType.StringLiteral, "com.example.foo");
            option.AddChildren(optionName, optionValue);
            root.AddChild(option);
            #endregion

            AssertSyntax(tokenList, root);
        }


        [Fact]
        public void ShouldParseEnumDefinition()
        {
            #region Arrange Enum Definition Token Input
            var tokenList = new List<Token>
            {
                new Token(TokenType.Id, 0, 0, "enum"),
                new Token(TokenType.String, 0, 1, "EnumName"),
                new Token(TokenType.Control, 0, 2, "{"),
                new Token(TokenType.EndLine, 0, 5, Environment.NewLine),
                new Token(TokenType.String, 0, 6, "DEFAULT"),
                new Token(TokenType.Control, 0, 7, "="),
                new Token(TokenType.Numeric, 0, 8, "0"),
                new Token(TokenType.Control, 0, 9, ";"),
                new Token(TokenType.EndLine, 0, 11, Environment.NewLine),
                new Token(TokenType.String, 0, 6, "VALONE"),
                new Token(TokenType.Control, 0, 9, "="),
                new Token(TokenType.Numeric, 0, 10, "1"),
                new Token(TokenType.Control, 0, 9, ";"),
                new Token(TokenType.EndLine, 0, 11, Environment.NewLine),
                new Token(TokenType.Control, 0, 12, "}")
            };
            #endregion

            #region Arrange Expected NodeTree Output
            var root = new RootNode();
            var enumnode = new Node(NodeType.Enum, "enum");
            var enumname = new Node(NodeType.Identifier, "EnumName");
            var field = new Node(NodeType.EnumField, "DEFAULT");
            var name = new Node(NodeType.Identifier, "DEFAULT");
            var value = new Node(NodeType.FieldNumber, "0");
            var field1 = new Node(NodeType.EnumField, "VALONE");
            var name1 = new Node(NodeType.Identifier, "VALONE");
            var value1 = new Node(NodeType.FieldNumber, "1");
            field.AddChildren(name, value);
            field1.AddChildren(name1, value1);
            enumnode.AddChildren(enumname, field, field1);
            root.AddChild(enumnode);
            #endregion

            AssertSyntax(tokenList, root);
        }

        [Fact]
        public void ShouldParseMessageDefinition()
        {
            #region Arrange Message Definition Token Input
            var tokenList = new List<Token>
            {
                // Outer Message Start
                new Token(TokenType.Id, 0, 0, "message"),
                new Token(TokenType.String, 0, 1, "Outer"),
                new Token(TokenType.Control, 0, 2, "{"),

                #region Message Fields
                new Token(TokenType.EndLine, 0, 5, Environment.NewLine),
                new Token(TokenType.String, 0, 0, "int32"),
                new Token(TokenType.String, 0, 1, "field_name"),
                new Token(TokenType.Control, 0, 2, "="),
                new Token(TokenType.Numeric, 0, 3, "2"),
                new Token(TokenType.Control, 0, 4, ";"),
                new Token(TokenType.EndLine, 0, 5, Environment.NewLine),
                #endregion

                #region Nested Enumeration Definition
                new Token(TokenType.Id, 0, 0, "enum"),
                new Token(TokenType.String, 0, 1, "EnumName"),
                new Token(TokenType.Control, 0, 2, "{"),
                new Token(TokenType.EndLine, 0, 5, Environment.NewLine),
                new Token(TokenType.String, 0, 6, "DEFAULT"),
                new Token(TokenType.Control, 0, 7, "="),
                new Token(TokenType.Numeric, 0, 8, "0"),
                new Token(TokenType.Control, 0, 9, ";"),
                new Token(TokenType.EndLine, 0, 11, Environment.NewLine),
                new Token(TokenType.String, 0, 6, "VALONE"),
                new Token(TokenType.Control, 0, 9, "="),
                new Token(TokenType.Numeric, 0, 10, "1"),
                new Token(TokenType.Control, 0, 9, ";"),
                new Token(TokenType.EndLine, 0, 11, Environment.NewLine),
                new Token(TokenType.Control, 0, 12, "}"),
                #endregion

                #region Nested Message Definition
                new Token(TokenType.EndLine, 0, 11, Environment.NewLine),
                new Token(TokenType.Id, 0, 0, "message"),
                new Token(TokenType.String, 0, 1, "Inner"),
                new Token(TokenType.Control, 0, 2, "{"),
                new Token(TokenType.EndLine, 0, 5, Environment.NewLine),
                new Token(TokenType.String, 0, 0, "fixed64"),
                new Token(TokenType.String, 0, 1, "field_name2"),
                new Token(TokenType.Control, 0, 2, "="),
                new Token(TokenType.Numeric, 0, 3, "0"),
                new Token(TokenType.Control, 0, 4, ";"),
                new Token(TokenType.EndLine, 0, 5, Environment.NewLine),
                new Token(TokenType.Control, 0, 4, "}"),
                #endregion

                new Token(TokenType.Control, 0, 4, "}")
            };
            #endregion

            #region Arrange Expected NodeTree Output
            var root = new RootNode();
            //  Define base Message with One Field
            var message = new Node(NodeType.Message, "message");
            var msgName = new Node(NodeType.Identifier, "Outer");

            #region Outer Message Field nodes
            var field = new Node(NodeType.Field, "int32");
            var type = new Node(NodeType.Type, "int32");
            var name = new Node(NodeType.Identifier, "field_name");
            var value = new Node(NodeType.FieldNumber, "2");
            field.AddChildren(type, name, value);
            #endregion

            #region Nested Message nodes
            var nestedMsg = new Node(NodeType.Message, "message");
            var nestedName = new Node(NodeType.Identifier, "Inner");

            var innerField = new Node(NodeType.Field, "fixed64");
            var innerType = new Node(NodeType.Type, "fixed64");
            var innerName = new Node(NodeType.Identifier, "field_name2");
            var innerValue = new Node(NodeType.FieldNumber, "0");
            innerField.AddChildren(innerType, innerName, innerValue);
            #endregion

            #region Nested Enumeration nodes
            var enumnode = new Node(NodeType.Enum, "enum");
            var enumname = new Node(NodeType.Identifier, "EnumName");
            var enumfield0 = new Node(NodeType.EnumField, "DEFAULT");
            var enumfieldname0 = new Node(NodeType.Identifier, "DEFAULT");
            var enumfieldvalue0 = new Node(NodeType.FieldNumber, "0");
            var enumfield1 = new Node(NodeType.EnumField, "VALONE");
            var enumfieldname1 = new Node(NodeType.Identifier, "VALONE");
            var enumfieldvalue1 = new Node(NodeType.FieldNumber, "1");
            enumnode.AddChildren(enumname, enumfield0, enumfield1);
            enumfield0.AddChildren(enumfieldname0, enumfieldvalue0);
            enumfield1.AddChildren(enumfieldname1, enumfieldvalue1);
            #endregion


            nestedMsg.AddChildren(nestedName, innerField);
            message.AddChildren(msgName, field, enumnode, nestedMsg);
            root.AddChild(message);
            #endregion

            AssertSyntax(tokenList, root);
        }

        [Fact]
        public void ShouldBuildMessageTypeFieldNode()
        {
            #region Arrange Message Type Field Token Input
            var tokenList = new List<Token>
            {
                new Token(TokenType.Id, 0, 0, "message"),
                new Token(TokenType.String, 0, 1, "Outer"),
                new Token(TokenType.Control, 0, 2, "{"),
                new Token(TokenType.EndLine, 0, 5, Environment.NewLine),
                new Token(TokenType.String, 0, 0, "field.type"),
                new Token(TokenType.String, 0, 1, "field_name"),
                new Token(TokenType.Control, 0, 2, "="),
                new Token(TokenType.Numeric, 0, 3, "2"),
                new Token(TokenType.Control, 0, 4, ";"),
                new Token(TokenType.EndLine, 0, 5, Environment.NewLine),
                new Token(TokenType.Control, 0, 4, "}")
            };
            #endregion

            #region Arrange Expected NodeTree Output
            var root = new RootNode();
            var message = new Node(NodeType.Message, "message");
            var msgName = new Node(NodeType.Identifier, "Outer");
            var field = new Node(NodeType.Field, "field.type");
            var type = new Node(NodeType.UserType, "field.type");
            var name = new Node(NodeType.Identifier, "field_name");
            var value = new Node(NodeType.FieldNumber, "2");
            field.AddChildren(type, name, value);
            message.AddChildren(msgName, field);
            root.AddChild(message);
            #endregion

            AssertSyntax(tokenList, root);
        }

        [Fact]
        public void ShouldBuildRepeatedFieldNode()
        {
            #region Arrange Repeated Field Token Input
            var tokenList = new List<Token>
            {
                new Token(TokenType.Id, 0, 0, "message"),
                new Token(TokenType.String, 0, 1, "Outer"),
                new Token(TokenType.Control, 0, 2, "{"),
                new Token(TokenType.EndLine, 0, 5, Environment.NewLine),
                new Token(TokenType.String, 0, 0 ,"repeated"),
                new Token(TokenType.String, 0, 0, "int32"),
                new Token(TokenType.String, 0, 1, "field_name"),
                new Token(TokenType.Control, 0, 2, "="),
                new Token(TokenType.Numeric, 0, 3, "2"),
                new Token(TokenType.Control, 0, 4, ";"),
                new Token(TokenType.EndLine, 0, 5, Environment.NewLine),
                new Token(TokenType.Control, 0, 4, "}")
            };
            #endregion

            #region Arrange Expected NodeTree Output
            var root = new RootNode();
            var message = new Node(NodeType.Message, "message");
            var msgName = new Node(NodeType.Identifier, "Outer");
            var field = new Node(NodeType.Field, "repeated");
            var repeated = new Node(NodeType.Repeated, "repeated");
            var type = new Node(NodeType.Type, "int32");
            var name = new Node(NodeType.Identifier, "field_name");
            var value = new Node(NodeType.FieldNumber, "2");
            field.AddChildren(repeated, type, name, value);
            message.AddChildren(msgName,field);
            root.AddChild(message);
            #endregion

            AssertSyntax(tokenList, root);
        }

        [Fact]
        public void ShouldBuildOneOfFieldNode()
        {
            #region Arrange OneOf Field Token Input
            var tokenList = new List<Token>
            {
                new Token(TokenType.Id, 0, 0, "message"),
                new Token(TokenType.String, 0, 0, "Outer"),
                new Token(TokenType.Control, 0, 0, "{"),
                new Token(TokenType.EndLine, 0, 0, Environment.NewLine),
                new Token(TokenType.Id, 0, 0, "oneof"),
                new Token(TokenType.String, 0, 0, "test_oneof"),
                new Token(TokenType.Control, 0, 0, "{"),
                new Token(TokenType.EndLine, 0, 0, Environment.NewLine),
                new Token(TokenType.String, 0, 0, "string"),
                new Token(TokenType.String, 0, 0, "name"),
                new Token(TokenType.Control, 0, 0, "="),
                new Token(TokenType.Numeric, 0, 0, "4"),
                new Token(TokenType.Control, 0, 4, ";"),
                new Token(TokenType.EndLine, 0, 0, Environment.NewLine),
                new Token(TokenType.String, 0, 0, "SubMessage"),
                new Token(TokenType.String, 0, 0, "sub_message"),
                new Token(TokenType.Control, 0, 0, "="),
                new Token(TokenType.Numeric, 0, 0, "9"),
                new Token(TokenType.Control, 0, 4, ";"),
                new Token(TokenType.EndLine, 0, 0, Environment.NewLine),
                new Token(TokenType.Control, 0, 0, "}"),
                new Token(TokenType.EndLine, 0, 0, Environment.NewLine),
                new Token(TokenType.Control, 0, 0, "}")
            };
            #endregion

            #region Arrange Expected NodeTree Output
            var root = new RootNode();
            var message = new Node(NodeType.Message, "message");
            var msgName = new Node(NodeType.Identifier, "Outer");
            var field = new Node(NodeType.OneOfField, "oneof");
            var oneofName = new Node(NodeType.Identifier, "test_oneof");

            var stringField = new Node(NodeType.Field, "string");
            var stringType = new Node(NodeType.Type, "string");
            var stringName = new Node(NodeType.Identifier, "name");
            var stringValue = new Node(NodeType.FieldNumber, "4");
            stringField.AddChildren(stringType, stringName, stringValue);

            var customField = new Node(NodeType.Field, "SubMessage");
            var customType = new Node(NodeType.UserType, "SubMessage");
            var customName = new Node(NodeType.Identifier, "sub_message");
            var customValue = new Node(NodeType.FieldNumber, "9");
            customField.AddChildren(customType, customName, customValue);

            field.AddChildren(oneofName, stringField, customField);
            message.AddChildren(msgName, field);
            root.AddChild(message);
            #endregion

            AssertSyntax(tokenList, root);
        }

        [Fact]
        public void ShouldBuildMapFieldNode()
        {
            #region Arrange Map Field Token Input
            var tokenList = new List<Token>
            {
                new Token(TokenType.Id, 0, 0, "message"),
                new Token(TokenType.String, 0, 0, "Outer"),
                new Token(TokenType.Control, 0, 0, "{"),
                new Token(TokenType.EndLine, 0, 0, Environment.NewLine),
                new Token(TokenType.Id, 0, 0, "map"),
                new Token(TokenType.Control, 0, 0, "<"),
                new Token(TokenType.String, 0, 0, "string"),
                new Token(TokenType.Control, 0, 0, ","),
                new Token(TokenType.String, 0, 0, "Project"),
                new Token(TokenType.Control, 0, 0, ">"),
                new Token(TokenType.String, 0, 0, "projects"),
                new Token(TokenType.Control, 0, 0, "="),
                new Token(TokenType.Numeric, 0, 0, "3"),
                new Token(TokenType.Control, 0, 4, ";"),
                new Token(TokenType.EndLine, 0, 0, Environment.NewLine),
                new Token(TokenType.Control, 0, 0, "}")
            };
            #endregion

            #region Arrange Expected NodeTree Output
            var root = new RootNode();
            var message = new Node(NodeType.Message, "message");
            var msgName = new Node(NodeType.Identifier, "Outer");
            var field = new Node(NodeType.Map, "map");
            var key = new Node(NodeType.MapKey, "string");
            var value = new Node(NodeType.MapValue, "Project");
            var mapName = new Node(NodeType.Identifier, "projects");
            var mapValue = new Node(NodeType.FieldNumber, "3");

            field.AddChildren(mapName, key, value, mapValue);
            message.AddChildren(msgName, field);
            root.AddChild(message);
            #endregion

            AssertSyntax(tokenList, root);
        }

        [Fact]
        public void ShouldBuildInlineStatementWithTrailingComment()
        {
            #region Arrange Inline Trailing Comment Token Input
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
            #endregion

            #region Arrange Expected NodeTree Output
            var root = new RootNode();
            var syntax = new Node(NodeType.Syntax, "syntax");
            var proto3 = new Node(NodeType.StringLiteral, "proto3");
            var comment = new Node(NodeType.Comment, "\\\\");
            var commentText = new Node(NodeType.CommentText, "This is a comment.");
            comment.AddChild(commentText);
            syntax.AddChildren(proto3, comment);
            root.AddChild(syntax);
            #endregion

            AssertSyntax(tokenList, root);
        }
    }
}
