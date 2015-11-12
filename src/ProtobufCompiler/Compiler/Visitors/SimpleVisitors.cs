using System.Linq;
using ProtobufCompiler.Compiler.Nodes;
using ProtobufCompiler.Interfaces;
using ProtobufCompiler.Types;

namespace ProtobufCompiler.Compiler.Visitors
{
    internal class SyntaxVisitor : IVisitor
    {
        public Syntax Syntax { get; internal set; }

        public void Visit(Node node)
        {
            var syntax = node.Children.SingleOrDefault(t => t.NodeType.Equals(NodeType.StringLiteral));
            Syntax = new Syntax(syntax?.NodeValue);
        }
    }

    internal class PackageVisitor : IVisitor
    {
        public Package Package { get; internal set; }

        public void Visit(Node node)
        {
            var package = node.Children.SingleOrDefault(t => t.NodeType.Equals(NodeType.Identifier));
            Package = new Package(package?.NodeValue);
        }
    }

    internal class OptionVisitor : IVisitor
    {
        public Option Option { get; internal set; }

        public void Visit(Node node)
        {
            var name = node.Children.SingleOrDefault(t => t.NodeType.Equals(NodeType.Identifier));
            var value = node.Children.SingleOrDefault(t => t.NodeType.Equals(NodeType.StringLiteral));
            Option = new Option(name?.NodeValue, value?.NodeValue);
        }
    }

    internal class ImportVisitor : IVisitor
    {
        public Import Import { get; internal set; }

        public void Visit(Node node)
        {
            var modifier = node.Children.SingleOrDefault(t => t.NodeType.Equals(NodeType.ImportModifier));
            var clas = node.Children.SingleOrDefault(t => t.NodeType.Equals(NodeType.StringLiteral));
            Import = new Import(modifier?.NodeValue, clas?.NodeValue);
        }
    }

    internal class EnumVisitor : IVisitor
    {
        public EnumDefinition EnumDefinition { get; internal set; }

        public void Visit(Node node)
        {

        }
    }

    internal class ServiceVisitor : IVisitor
    {
        public ServiceDefinition Service { get; internal set; }

        public void Visit(Node node)
        {

        }
    }
}
