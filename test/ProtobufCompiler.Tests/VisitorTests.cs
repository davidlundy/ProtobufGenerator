using FluentAssertions;
using ProtobufCompiler.Compiler.Nodes;
using ProtobufCompiler.Compiler.Types;
using ProtobufCompiler.Compiler.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ProtobufCompiler.Tests
{
    public class VisitorTests
    {
        [Fact]
        public void DebugVisitorShouldPrettyPrint()
        {
            var root = new Node(NodeType.Root, "root");
            var one = new Node(NodeType.Field, "one");
            var two = new Node(NodeType.Type, "two");
            var three = new Node(NodeType.Field, "three");
            root.AddChild(one);
            one.AddChild(two);
            two.AddChild(three);

            var expected = "*Root : root" + Environment.NewLine +
                           "-Field : one" + Environment.NewLine +
                           "--Type : two" + Environment.NewLine +
                           "---Field : three" + Environment.NewLine;

            var debugVisitor = new DebugVisitor();
            root.Accept(debugVisitor);
            var result = debugVisitor.ToString();

            result.Should().Be(expected, "because pretty printing should be successful.");
        }

        [Fact]
        public void DebugVisitorShouldTrackDepth()
        {
            var root = new Node(NodeType.Root, "root");
            var one = new Node(NodeType.Field, "one");
            var two = new Node(NodeType.Type, "two");
            var three = new Node(NodeType.Field, "three");
            root.AddChild(one);
            one.AddChild(two);
            two.AddChild(three);

            var debugVisitor = new DebugVisitor();
            root.Accept(debugVisitor);

            var depth = debugVisitor.GetDepth(three.Guid);

            depth.Should().Be(3, "because root starts at 0 and we have a 3 deep tree");
        }

        [Fact]
        public void BuilderVisitorCantVisitNonRoot()
        {
            #region Arrange Bad Root Node Input and Builder Visitor

            var root = new Node(NodeType.Service, "service");
            var sut = new BuilderVisitor();

            #endregion Arrange Bad Root Node Input and Builder Visitor

            var expected = Assert.Throws<InvalidOperationException>(() => root.Accept(sut));
            expected.Message.Should().Be("Cannot use BuilderVisitor on non-root Node");
        }

        [Fact]
        public void BuilderVisitorShouldBuildSyntax()
        {
            #region Arrange Syntax Node Input

            var root = new RootNode();
            var syntax = new Node(NodeType.Syntax, "syntax");
            var proto3 = new Node(NodeType.StringLiteral, "proto3");
            syntax.AddChild(proto3);
            root.AddChild(syntax);

            #endregion Arrange Syntax Node Input

            var expected = new Syntax("proto3");
            var sut = new BuilderVisitor();
            root.Accept(sut);

            sut.FileDescriptor.Syntax.Should().Be(expected);
        }

        [Fact]
        public void BuilderVisitorShouldBuildPackage()
        {
            #region Arrange Package Node Input

            var root = new RootNode();
            var package = new Node(NodeType.Package, "package");
            var packageName = new Node(NodeType.Identifier, "foo.bar.baz");
            package.AddChild(packageName);
            root.AddChild(package);

            #endregion Arrange Package Node Input

            var expected = new Package("foo.bar.baz");
            var sut = new BuilderVisitor();
            root.Accept(sut);

            sut.FileDescriptor.Package.Should().Be(expected);
        }

        [Fact]
        public void BuilderVisitorShouldBuildImport()
        {
            #region Arrange Package Node Input

            var root = new RootNode();
            var import = new Node(NodeType.Import, "import");
            var type = new Node(NodeType.ImportModifier, "public");
            var otherMessage = new Node(NodeType.StringLiteral, "other.message");
            import.AddChildren(type, otherMessage);
            root.AddChild(import);

            #endregion Arrange Package Node Input

            var expected = new Import("public", "other.message");
            var sut = new BuilderVisitor();
            root.Accept(sut);

            sut.FileDescriptor.Imports.First().Should().Be(expected);
        }

        [Fact]
        public void BuilderVisitorShouldBuildOption()
        {
            #region Arrange Package Node Input

            var root = new RootNode();
            var option = new Node(NodeType.Option, "option");
            var optionName = new Node(NodeType.Identifier, "java_package");
            var optionValue = new Node(NodeType.StringLiteral, "com.example.foo");
            option.AddChildren(optionName, optionValue);
            root.AddChild(option);

            #endregion Arrange Package Node Input

            var expected = new Option("java_package", "com.example.foo");
            var sut = new BuilderVisitor();
            root.Accept(sut);

            sut.FileDescriptor.Options.First().Should().Be(expected);
        }

        [Fact]
        public void BuilderVisitorShouldBuildEnum()
        {
            #region Arrange Expected NodeTree Input

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

            #endregion Arrange Expected NodeTree Input

            var expected = new EnumDefinition("EnumName", null, new List<EnumField>
            {
                new EnumField("DEFAULT", 0, null),
                new EnumField("VALONE", 1, null)
            });

            var sut = new BuilderVisitor();
            root.Accept(sut);

            sut.FileDescriptor.Enumerations.First().Should().Be(expected);
        }

        [Fact]
        public void BuilderVisitorShouldBuildMessage()
        {
            #region Arrange Expected NodeTree Input

            var root = new RootNode();
            //  Define base NestedMessage with One Field
            var message = new Node(NodeType.Message, "message");
            var msgName = new Node(NodeType.Identifier, "Outer");

            #region Outer Message Field nodes

            var field = new Node(NodeType.Field, "int32");
            var type = new Node(NodeType.Type, "int32");
            var name = new Node(NodeType.Identifier, "field_name");
            var value = new Node(NodeType.FieldNumber, "2");
            field.AddChildren(type, name, value);

            #endregion Outer Message Field nodes

            #region Nested Message nodes

            var nestedMsg = new Node(NodeType.Message, "message");
            var nestedName = new Node(NodeType.Identifier, "Inner");

            var innerField = new Node(NodeType.Field, "fixed64");
            var innerType = new Node(NodeType.Type, "fixed64");
            var innerName = new Node(NodeType.Identifier, "field_name2");
            var innerValue = new Node(NodeType.FieldNumber, "0");
            innerField.AddChildren(innerType, innerName, innerValue);

            #endregion Nested Message nodes

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

            #endregion Nested Enumeration nodes

            nestedMsg.AddChildren(nestedName, innerField);
            message.AddChildren(msgName, field, enumnode, nestedMsg);
            root.AddChild(message);

            #endregion Arrange Expected NodeTree Input

            #region Arrange Expected Output

            var expFields = new List<Field>
            {
                new Field("int32", "field_name", 2, null, false)
            };
            var inFields = new List<Field>
            {
                new Field("fixed64", "field_name2", 0, null, false)
            };
            var enumDefs = new List<EnumDefinition>
            {
                new EnumDefinition("EnumName", null, new List<EnumField>
                {
                    new EnumField("DEFAULT", 0, null),
                    new EnumField("VALONE", 1, null)
                })
            };
            var msgDefs = new List<MessageDefinition>
            {
                new MessageDefinition("Inner", inFields, null, null, null, null, null)
            };
            var expected = new MessageDefinition("Outer", expFields, null, null, null, enumDefs, msgDefs);

            #endregion Arrange Expected Output

            var sut = new BuilderVisitor();
            root.Accept(sut);

            sut.FileDescriptor.Messages.First().Should().Be(expected);
        }
    }
}