using ProtobufCompiler.Compiler.Nodes;

namespace ProtobufCompiler.Interfaces
{
    internal interface ISyntaxAnalyzer<T> where T: Node
    {
        T Analyze();
    }
}