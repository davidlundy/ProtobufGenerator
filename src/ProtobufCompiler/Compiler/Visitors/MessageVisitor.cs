using System.Collections.Generic;
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

        }
    }
}
