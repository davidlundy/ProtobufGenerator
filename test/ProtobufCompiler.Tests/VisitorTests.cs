using System;
using System.Linq;
using FluentAssertions;
using ProtobufCompiler.Compiler.Nodes;
using ProtobufCompiler.Compiler.Visitors;
using ProtobufCompiler.Types;
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
            #endregion

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
            #endregion

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
            #endregion

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
            #endregion

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
            #endregion

            var expected = new Option("java_package", "com.example.foo");
            var sut = new BuilderVisitor();
            root.Accept(sut);

            sut.FileDescriptor.Options.First().Should().Be(expected);
        }
    }
}
