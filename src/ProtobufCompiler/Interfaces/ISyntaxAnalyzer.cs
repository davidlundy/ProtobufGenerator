using ProtobufCompiler.Compiler;
using ProtobufCompiler.Compiler.Nodes;
using System.Collections.Generic;

namespace ProtobufCompiler.Interfaces
{
    internal interface ISyntaxAnalyzer<T> where T: Node
    {
        NodeResult<T> Analyze(Queue<Token> tokens);
    }
}