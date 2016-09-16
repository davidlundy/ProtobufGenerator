using System;
using System.Collections.Generic;
using System.Linq;
using ProtobufCompiler.Compiler.Visitors;
using ProtobufCompiler.Extensions;
using ProtobufCompiler.Interfaces;

namespace ProtobufCompiler.Compiler.Nodes
{
    public class Node : IEquatable<Node>
    {
        public Node Parent { get; set; }
        public NodeType NodeType { get; }
        public string NodeValue { get; }
        public IList<Node> Children { get; }

        public Guid Guid { get; } = Guid.NewGuid();

        public Node(NodeType nodeType, string value)
        {
            NodeType = nodeType;
            NodeValue = value ?? string.Empty;
            Children = new List<Node>();
        }

        internal void AddChild(Node node)
        {
            if (ReferenceEquals(node, null)) return;
            Children.Add(node);
            node.Parent = this;
        }

        internal void AddChildren(params Node[] nodes)
        {
            foreach (var node in nodes)
            {
                AddChild(node);
            }
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString()
        {
            var debugVisitor = new DebugVisitor();
            Accept(debugVisitor);
            return debugVisitor.ToString();
        }

        public bool Equals(Node other)
        {
            if (other == null) return false;

            if (NodeType.Equals(NodeType.Root) && other.NodeType.Equals(NodeType.Root))
            {
                return Children.SequenceEqual(other.Children);
            }
            if (!NodeType.Equals(NodeType.Root) && !other.NodeType.Equals(NodeType.Root))
            {
                return NodeType.Equals(other.NodeType) && Children.SequenceEqual(other.Children) &&
                       NodeValue.Equals(other.NodeValue, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals(obj as Node);
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 7) + NodeType.GetHashCode();
            hash = (hash * 7) + NodeValue.GetHashCode();
            hash = (hash * 7) + Children.GetHashCode();
            return hash;
        }
    }

    public enum NodeType
    {
        Root,
        Comment,
        CommentText,
        Identifier,
        Assignment,
        StringLiteral,
        IntegerLiteral,
        FloatLiteral,
        BooleanLiteral,
        Syntax,
        Package,
        Import,
        ImportModifier,
        Option,
        Enum,
        EnumConstant,
        Message,
        OneOfField,
        Field,
        FieldNumber,
        Type,
        UserType,
        Repeated,
        EnumField,
        Map,
        MapKey,
        MapValue,
        Service,
        Streaming,
        ServiceReturnType,
        ServiceInputType,
        Reserved
    }
}
