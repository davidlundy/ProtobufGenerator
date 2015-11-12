using ProtobufCompiler.Compiler.Nodes;

namespace ProtobufCompiler.Interfaces
{
    internal interface IVisitor
    {
         void Visit(Node node);
    }

}