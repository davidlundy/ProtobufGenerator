using ProtobufCompiler.Compiler.Nodes;

namespace ProtobufCompiler.Interfaces
{
    internal interface ISyntaxAnalyzer
    {
        RootNode Analyze();
    }
}