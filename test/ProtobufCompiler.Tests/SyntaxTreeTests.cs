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
        public void ShouldBuildPackageNode()
        {
            // Arrange Input
            var tokenList = new List<Token>
            {
                new Token(TokenType.Id, 0, 0, "package"),
                new Token(TokenType.String, 0, 1, "foo.bar.baz"),
                new Token(TokenType.Control, 0, 2, ";"),
                new Token(TokenType.EndLine, 0, 4, Environment.NewLine)
            };

            // Arrange Output
            var root = new RootNode();
            var package = new Node(NodeType.Package, "package");
            var packageName = new Node(NodeType.Identifier, "foo.bar.baz");
            package.AddChild(packageName);
            root.AddChild(package);

            // Act
            _sys = new SyntaxAnalyzer(new Queue<Token>(tokenList));
            var result = _sys.Analyze();

            // Assert
            result.Should().Be(root, BecauseObjectGraphsEqual);
        }

        [Fact]
        public void ShouldBuildOptionNode()
        {
            // Arrange Input
            var tokenList = new List<Token>
            {
                new Token(TokenType.Id, 0, 0, "option"),
                new Token(TokenType.String, 0, 1, "java_package"),
                new Token(TokenType.Control, 0, 2, "="),
                new Token(TokenType.String, 0, 3, "\"com.example.foo\""),
                new Token(TokenType.Control, 0, 4, ";"),
                new Token(TokenType.EndLine, 0, 5, Environment.NewLine)
            };

            // Arrange Output
            var root = new RootNode();
            var option = new Node(NodeType.Option, "option");
            var optionName = new Node(NodeType.Identifier, "java_package");
            var optionValue = new Node(NodeType.StringLiteral, "com.example.foo");
            option.AddChild(optionName);
            option.AddChild(optionValue);
            root.AddChild(option);

            // Act
            _sys = new SyntaxAnalyzer(new Queue<Token>(tokenList));
            var result = _sys.Analyze();

            // Assert
            result.Should().Be(root, BecauseObjectGraphsEqual);
        }

        [Fact]
        public void ShouldParseSimpleEnumDefinition()
        {
            // Arrange Input
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

            // Arrange Output
            var root = new RootNode();
            var enumnode = new Node(NodeType.Enum, "enum");
            var enumname = new Node(NodeType.Identifier, "EnumName");
            var field = new Node(NodeType.EnumField, "DEFAULT");
            var name = new Node(NodeType.Identifier, "DEFAULT");
            var value = new Node(NodeType.FieldNumber, "0");
            var field1 = new Node(NodeType.EnumField, "VALONE");
            var name1 = new Node(NodeType.Identifier, "VALONE");
            var value1 = new Node(NodeType.FieldNumber, "1");
            field.AddChild(name);
            field.AddChild(value);
            field1.AddChild(name1);
            field1.AddChild(value1);
            enumnode.AddChild(enumname);
            enumnode.AddChild(field);
            enumnode.AddChild(field1);
            root.AddChild(enumnode);

            // Act
            _sys = new SyntaxAnalyzer(new Queue<Token>(tokenList));
            var result = _sys.Analyze();

            // Assert
            result.Should().Be(root, BecauseObjectGraphsEqual);
        }

        [Fact]
        public void ShouldParseSimpleMessage()
        {
            // Arrange Input
            var tokenList = new List<Token>
            {
                new Token(TokenType.Id, 0, 0, "message"),
                new Token(TokenType.String, 0, 1, "Outer"),
                new Token(TokenType.Control, 0, 2, "{"),
                new Token(TokenType.EndLine, 0, 5, Environment.NewLine),
                new Token(TokenType.String, 0, 0, "int32"),
                new Token(TokenType.String, 0, 1, "field_name"),
                new Token(TokenType.Control, 0, 2, "="),
                new Token(TokenType.Numeric, 0, 3, "2"),
                new Token(TokenType.Control, 0, 4, ";"),
                new Token(TokenType.EndLine, 0, 5, Environment.NewLine),
                new Token(TokenType.Control, 0, 4, "}")
            };

            // Arrange Output
            var root = new RootNode();
            var message = new Node(NodeType.Message, "message");
            var msgName = new Node(NodeType.Identifier, "Outer");
            var field = new Node(NodeType.Field, "int32");
            var type = new Node(NodeType.Type, "int32");
            var name = new Node(NodeType.Identifier, "field_name");
            var value = new Node(NodeType.FieldNumber, "2");
            field.AddChild(type);
            field.AddChild(name);
            field.AddChild(value);
            message.AddChild(msgName);
            message.AddChild(field);
            root.AddChild(message);

            // Act
            _sys = new SyntaxAnalyzer(new Queue<Token>(tokenList));
            var result = _sys.Analyze();

            // Assert
            result.Should().Be(root, BecauseObjectGraphsEqual);
        }

        [Fact]
        public void ShouldParseNestedMessage()
        {
            // Arrange Input
            var tokenList = new List<Token>
            {
                new Token(TokenType.Id, 0, 0, "message"),
                new Token(TokenType.String, 0, 1, "Outer"),
                new Token(TokenType.Control, 0, 2, "{"),
                new Token(TokenType.EndLine, 0, 5, Environment.NewLine),
                new Token(TokenType.String, 0, 0, "int32"),
                new Token(TokenType.String, 0, 1, "field_name"),
                new Token(TokenType.Control, 0, 2, "="),
                new Token(TokenType.Numeric, 0, 3, "2"),
                new Token(TokenType.Control, 0, 4, ";"),
                new Token(TokenType.EndLine, 0, 5, Environment.NewLine),
                new Token(TokenType.Id, 0, 0, "message"),
                new Token(TokenType.String, 0, 1, "Inner"),
                new Token(TokenType.Control, 0, 2, "{"),
                new Token(TokenType.EndLine, 0, 5, Environment.NewLine),
                new Token(TokenType.String, 0, 0, "int32"),
                new Token(TokenType.String, 0, 1, "field_name"),
                new Token(TokenType.Control, 0, 2, "="),
                new Token(TokenType.Numeric, 0, 3, "2"),
                new Token(TokenType.Control, 0, 4, ";"),
                new Token(TokenType.EndLine, 0, 5, Environment.NewLine),
                new Token(TokenType.Control, 0, 4, "}"),
                new Token(TokenType.Control, 0, 4, "}")
            };

            // Arrange Output
            var root = new RootNode();
            var message = new Node(NodeType.Message, "message");
            var nestedMsg = new Node(NodeType.Message, "message");
            var msgName = new Node(NodeType.Identifier, "Outer");
            var nestedName = new Node(NodeType.Identifier, "Inner");
            var field = new Node(NodeType.Field, "int32");
            var type = new Node(NodeType.Type, "int32");
            var name = new Node(NodeType.Identifier, "field_name");
            var value = new Node(NodeType.FieldNumber, "2");
            field.AddChild(type);
            field.AddChild(name);
            field.AddChild(value);
            nestedMsg.AddChild(nestedName);
            nestedMsg.AddChild(field);
            message.AddChild(msgName);
            message.AddChild(field);
            message.AddChild(nestedMsg);
            root.AddChild(message);

            // Act
            _sys = new SyntaxAnalyzer(new Queue<Token>(tokenList));
            var result = _sys.Analyze();

            // Assert
            result.Should().Be(root, BecauseObjectGraphsEqual);
        }

        [Fact]
        public void ShouldParseNestedEnum()
        {
            // Arrange Input
            var tokenList = new List<Token>
            {
                new Token(TokenType.Id, 0, 0, "message"),
                new Token(TokenType.String, 0, 1, "Outer"),
                new Token(TokenType.Control, 0, 2, "{"),
                new Token(TokenType.EndLine, 0, 5, Environment.NewLine),
                new Token(TokenType.String, 0, 0, "int32"),
                new Token(TokenType.String, 0, 1, "field_name"),
                new Token(TokenType.Control, 0, 2, "="),
                new Token(TokenType.Numeric, 0, 3, "2"),
                new Token(TokenType.Control, 0, 4, ";"),
                new Token(TokenType.EndLine, 0, 5, Environment.NewLine),
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
                new Token(TokenType.EndLine, 0, 11, Environment.NewLine),
                new Token(TokenType.Control, 0, 4, "}")
            };

            // Arrange Output
            var root = new RootNode();
            var message = new Node(NodeType.Message, "message");
            var msgName = new Node(NodeType.Identifier, "Outer");
            var field = new Node(NodeType.Field, "int32");
            var type = new Node(NodeType.Type, "int32");
            var name = new Node(NodeType.Identifier, "field_name");
            var value = new Node(NodeType.FieldNumber, "2");
            var enumnode = new Node(NodeType.Enum, "enum");
            var enumname = new Node(NodeType.Identifier, "EnumName");
            var enumfield0 = new Node(NodeType.EnumField, "DEFAULT");
            var enumfieldname0 = new Node(NodeType.Identifier, "DEFAULT");
            var enumfieldvalue0 = new Node(NodeType.FieldNumber, "0");
            var enumfield1 = new Node(NodeType.EnumField, "VALONE");
            var enumfieldname1 = new Node(NodeType.Identifier, "VALONE");
            var enumfieldvalue1 = new Node(NodeType.FieldNumber, "1");

            field.AddChild(type);
            field.AddChild(name);
            field.AddChild(value);
            enumnode.AddChild(enumname);
            enumnode.AddChild(enumfield0);
            enumnode.AddChild(enumfield1);
            enumfield0.AddChild(enumfieldname0);
            enumfield0.AddChild(enumfieldvalue0);
            enumfield1.AddChild(enumfieldname1);
            enumfield1.AddChild(enumfieldvalue1);
            message.AddChild(msgName);
            message.AddChild(field);
            message.AddChild(enumnode);

            root.AddChild(message);

            // Act
            _sys = new SyntaxAnalyzer(new Queue<Token>(tokenList));
            var result = _sys.Analyze();

            // Assert
            result.Should().Be(root, BecauseObjectGraphsEqual);
        }

        [Fact]
        public void ShouldBuildMessageTypeFieldNode()
        {
            // Arrange Input
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

            // Arrange Output
            var root = new RootNode();
            var message = new Node(NodeType.Message, "message");
            var msgName = new Node(NodeType.Identifier, "Outer");
            var field = new Node(NodeType.Field, "field.type");
            var type = new Node(NodeType.UserType, "field.type");
            var name = new Node(NodeType.Identifier, "field_name");
            var value = new Node(NodeType.FieldNumber, "2");
            field.AddChild(type);
            field.AddChild(name);
            field.AddChild(value);
            message.AddChild(msgName);
            message.AddChild(field);
            root.AddChild(message);

            // Act
            _sys = new SyntaxAnalyzer(new Queue<Token>(tokenList));
            var result = _sys.Analyze();

            // Assert
            result.Should().Be(root, BecauseObjectGraphsEqual);
        }

        [Fact]
        public void ShouldBuildSimpleTypeFieldNode()
        {
            // Arrange Input
            var tokenList = new List<Token>
            {
                new Token(TokenType.Id, 0, 0, "message"),
                new Token(TokenType.String, 0, 1, "Outer"),
                new Token(TokenType.Control, 0, 2, "{"),
                new Token(TokenType.EndLine, 0, 5, Environment.NewLine),
                new Token(TokenType.String, 0, 0, "int32"),
                new Token(TokenType.String, 0, 1, "field_name"),
                new Token(TokenType.Control, 0, 2, "="),
                new Token(TokenType.Numeric, 0, 3, "2"),
                new Token(TokenType.Control, 0, 4, ";"),
                new Token(TokenType.EndLine, 0, 5, Environment.NewLine),
                new Token(TokenType.Control, 0, 4, "}")
            };

            // Arrange Output
            var root = new RootNode();
            var message = new Node(NodeType.Message, "message");
            var msgName = new Node(NodeType.Identifier, "Outer");
            var field = new Node(NodeType.Field, "int32");
            var type = new Node(NodeType.Type, "int32");
            var name = new Node(NodeType.Identifier, "field_name");
            var value = new Node(NodeType.FieldNumber, "2");
            field.AddChild(type);
            field.AddChild(name);
            field.AddChild(value);
            message.AddChild(msgName);
            message.AddChild(field);
            root.AddChild(message);

            // Act
            _sys = new SyntaxAnalyzer(new Queue<Token>(tokenList));
            var result = _sys.Analyze();

            // Assert
            result.Should().Be(root, BecauseObjectGraphsEqual);
        }

        [Fact]
        public void ShouldBuildRepeatedFieldNode()
        {
            // Arrange Input
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

            // Arrange Output
            var root = new RootNode();
            var message = new Node(NodeType.Message, "message");
            var msgName = new Node(NodeType.Identifier, "Outer");
            var field = new Node(NodeType.Field, "repeated");
            var repeated = new Node(NodeType.Repeated, "repeated");
            var type = new Node(NodeType.Type, "int32");
            var name = new Node(NodeType.Identifier, "field_name");
            var value = new Node(NodeType.FieldNumber, "2");
            field.AddChild(repeated);
            field.AddChild(type);
            field.AddChild(name);
            field.AddChild(value);
            message.AddChild(msgName);
            message.AddChild(field);
            root.AddChild(message);

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
