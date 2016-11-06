using ProtobufCompiler.Compiler.Nodes;
using ProtobufCompiler.Interfaces;
using System;
using System.Collections.Generic;

namespace ProtobufCompiler.Compiler.SyntaxAnalysis
{
    internal class ServiceStatementAnalyzer : ISyntaxAnalyzer<Node>
    {
        public NodeResult<Node> Analyze(Queue<Token> tokens)
        {
            throw new NotImplementedException();
        }
    }
}
