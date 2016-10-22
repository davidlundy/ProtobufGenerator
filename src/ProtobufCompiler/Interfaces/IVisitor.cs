using ProtobufCompiler.Compiler.Errors;
using ProtobufCompiler.Compiler.Nodes;
using System.Collections.Generic;

namespace ProtobufCompiler.Interfaces
{
    public interface IVisitor
    {
        void Visit(Node node);
    }

    public interface IErrorTrackingVisitor : IVisitor
    {
        ICollection<CompilerError> Errors { get; }
    }
}