using System;
using System.Collections.Generic;
using System.Linq;
#if DNXCORE50
using System.Globalization;
#endif

namespace ProtobufCompiler.Compiler.Nodes
{
    internal class Node : IEquatable<Node>
    {
        internal Node Parent { get; set; }
        internal NodeType NodeType { get; }
        internal string NodeValue { get; }
        internal IList<Node> Children { get; }

#if DNXCORE50
        internal StringComparer InvCultIc = CultureInfo.InvariantCulture.CompareInfo.GetStringComparer(CompareOptions.None);
#else
        internal StringComparer InvCultIc = StringComparer.InvariantCultureIgnoreCase;
#endif

        internal Node(NodeType nodeType, string value)
        {
            NodeType = nodeType;
            NodeValue = value ?? string.Empty;
            Children = new List<Node>();
        }

        internal void AddChild(Node node)
        {
            Children.Add(node);
            node.Parent = this;
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
                       InvCultIc.Equals(NodeValue, other.NodeValue);
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

    internal enum NodeType
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
        Option,
        OptionName,
        Enum,
        EnumConstant,
        Message,
        Field,
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
