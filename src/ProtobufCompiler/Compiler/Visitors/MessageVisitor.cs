using System.Collections.Generic;
using System.Linq;
using ProtobufCompiler.Compiler.Errors;
using ProtobufCompiler.Compiler.Nodes;
using ProtobufCompiler.Types;

namespace ProtobufCompiler.Compiler.Visitors
{
    internal class MessageVisitor : SemanticBaseVisitor
    {
        public MessageDefinition Message { get; internal set; }

        internal MessageVisitor(ICollection<ParseError> errors) : base(errors)
        {

        }

        public override void Visit(Node node)
        {
            var name = node.Children.Single(t => t.NodeType.Equals(NodeType.Identifier)).NodeValue;

            var embMsg = node.Children.Where(t => t.NodeType.Equals(NodeType.Message));
            var messages = new List<MessageDefinition>();
            foreach (var msg in embMsg)
            {
                var msgVisitor = new MessageVisitor(Errors);
                msg.Accept(msgVisitor);
                messages.Add(msgVisitor.Message);
            }

            var embEnum = node.Children.Where(t => t.NodeType.Equals(NodeType.Enum));
            var enums = new List<EnumDefinition>();
            foreach (var en in embEnum)
            {
                var enumVisitor = new EnumVisitor(Errors);
                en.Accept(enumVisitor);
                enums.Add(enumVisitor.EnumDefinition);
            }

            var fieldNodes = node.Children.Where(t => t.NodeType.Equals(NodeType.Field));
            var fields = new List<Field>();
            foreach (var field in fieldNodes)
            {
                var fieldVisitor = new FieldVisitor(Errors);
                field.Accept(fieldVisitor);
                fields.Add(fieldVisitor.Field);
            }

            Message = new MessageDefinition(name, fields, null, null, null, enums, messages);

        }
    }

    internal class FieldVisitor : SemanticBaseVisitor
    {
        public Field Field { get; internal set; }

        public FieldVisitor(ICollection<ParseError> errors) : base(errors)
        {

        }

        public override void Visit(Node node)
        {
            var isRepeated = !ReferenceEquals(null, node.Children.SingleOrDefault(t => t.NodeType.Equals(NodeType.Repeated)));
            var type = node.Children.Single(t => t.NodeType.Equals(NodeType.Type) || t.NodeType.Equals(NodeType.UserType)).NodeValue;
            var name = node.Children.Single(t => t.NodeType.Equals(NodeType.Identifier)).NodeValue;
            var value = node.Children.Single(t => t.NodeType.Equals(NodeType.FieldNumber)).NodeValue;
            var number = int.Parse(value);
            Field = new Field(type, name, number, null, isRepeated);
        }
    }
}
