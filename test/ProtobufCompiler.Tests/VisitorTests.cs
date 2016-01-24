using System;
using FluentAssertions;
using ProtobufCompiler.Compiler.Nodes;
using ProtobufCompiler.Compiler.Visitors;
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

        
    }
}
