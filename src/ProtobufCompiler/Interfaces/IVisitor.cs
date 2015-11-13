using System.Collections.Generic;
using ProtobufCompiler.Compiler.Errors;
using ProtobufCompiler.Compiler.Nodes;

namespace ProtobufCompiler.Interfaces
{
    internal interface IVisitor
    {
        void Visit(Node node);
    }

    internal interface IErrorTrackingVisitor : IVisitor
    {
        ICollection<ParseError> Errors { get; }
    }

}