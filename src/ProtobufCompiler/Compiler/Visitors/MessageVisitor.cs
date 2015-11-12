using ProtobufCompiler.Compiler.Nodes;
using ProtobufCompiler.Interfaces;
using ProtobufCompiler.Types;

namespace ProtobufCompiler.Compiler.Visitors
{
    internal class MessageVisitor : IVisitor
    {
        public MessageDefinition Message { get; internal set; }

        public void Visit(Node node)
        {

        }
    }
}
