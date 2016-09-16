using System.Collections.Generic;
using ProtobufCompiler.Compiler.Errors;
using ProtobufCompiler.Compiler.Nodes;
using ProtobufCompiler.Interfaces;
using ProtobufCompiler.Compiler.Types;
using System.Linq;

namespace ProtobufCompiler.Compiler.Types
{
    internal abstract class SemanticBaseVisitor : IErrorTrackingVisitor
    {
        public ICollection<ParseError> Errors { get; internal set; }

        internal SemanticBaseVisitor(ICollection<ParseError> errors)
        {
            Errors = errors;
        }

        public abstract void Visit(Node node);
    }

    internal class SyntaxVisitor : SemanticBaseVisitor
    {
        public Syntax Syntax { get; internal set; }

        internal SyntaxVisitor(ICollection<ParseError> errors) : base(errors)
        {
            
        }

        public override void Visit(Node node)
        {
            var syntax = node.Children.SingleOrDefault(t => t.NodeType.Equals(NodeType.StringLiteral));
            Syntax = new Syntax(syntax?.NodeValue);
        }
    }

    internal class PackageVisitor : SemanticBaseVisitor
    {
        public Package Package { get; internal set; }

        internal PackageVisitor(ICollection<ParseError> errors) : base(errors) { }

        public override void Visit(Node node)
        {
            var package = node.Children.SingleOrDefault(t => t.NodeType.Equals(NodeType.Identifier));
            Package = new Package(package?.NodeValue);
        }
    }

    internal class OptionVisitor : SemanticBaseVisitor
    {
        public Option Option { get; internal set; }

        internal OptionVisitor(ICollection<ParseError> errors ) : base(errors) { }

        public override void Visit(Node node)
        {
            var name = node.Children.SingleOrDefault(t => t.NodeType.Equals(NodeType.Identifier));
            var value = node.Children.SingleOrDefault(t => t.NodeType.Equals(NodeType.StringLiteral));
            Option = new Option(name?.NodeValue, value?.NodeValue);
        }
    }

    internal class ImportVisitor : SemanticBaseVisitor
    {
        public Import Import { get; internal set; }

        internal ImportVisitor(ICollection<ParseError> errors ) : base(errors) { }

        public override void Visit(Node node)
        {
            var modifier = node.Children.SingleOrDefault(t => t.NodeType.Equals(NodeType.ImportModifier));
            var clas = node.Children.SingleOrDefault(t => t.NodeType.Equals(NodeType.StringLiteral));
            Import = new Import(modifier?.NodeValue, clas?.NodeValue);
        }
    }

    internal class EnumVisitor : SemanticBaseVisitor
    {
        public EnumDefinition EnumDefinition { get; internal set; }

        internal EnumVisitor(ICollection<ParseError> errors) : base(errors)
        {
            
        }

        public override void Visit(Node node)
        {
            var name = node.Children.Single(t => t.NodeType.Equals(NodeType.Identifier)).NodeValue;
            var enumFieldNodes = node.Children.Where(t => t.NodeType.Equals(NodeType.EnumField));
            var enumFields = new List<EnumField>();
            foreach(var ef in enumFieldNodes)
            {
                var vis = new EnumFieldVisitor(Errors);
                ef.Accept(vis);
                enumFields.Add(vis.EnumField);
            }
            
            EnumDefinition = new EnumDefinition(name, null, enumFields);
        }
    }

    internal class EnumFieldVisitor : SemanticBaseVisitor
    {
        public EnumField EnumField { get; internal set; }

        internal EnumFieldVisitor(ICollection<ParseError> errors) : base(errors)
        {
            
        }

        public override void Visit(Node node)
        {
            var name = node.Children.Single(t => t.NodeType.Equals(NodeType.Identifier)).NodeValue;
            var value = node.Children.Single(t => t.NodeType.Equals(NodeType.FieldNumber)).NodeValue;
            var number = int.Parse(value);
            EnumField = new EnumField(name, number, null);
        }
    }

    internal class ServiceVisitor : SemanticBaseVisitor
    {
        public ServiceDefinition Service { get; internal set; }

        internal ServiceVisitor(ICollection<ParseError> errors) : base(errors)
        {
        }

        public override void Visit(Node node)
        {

        }
    }
}
