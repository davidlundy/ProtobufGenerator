using System.Collections.Generic;
using ProtobufCompiler.Compiler;

namespace ProtobufCompiler.Interfaces
{

    internal interface ISyntaxAnalyzer
    {
        void Analyze();

        Queue<Statement> Statements { get; }

        IList<ParseError> Errors { get; }
    }
}