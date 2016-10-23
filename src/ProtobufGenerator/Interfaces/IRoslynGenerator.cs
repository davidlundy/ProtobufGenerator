using ProtobufCompiler.Compiler.Types;
using ProtobufGenerator.Generation;
using System.Collections.Generic;

namespace ProtobufGenerator.Interfaces
{
    public interface IRoslynGenerator
    {
        ICollection<JobResult> Generate(FileDescriptor descriptor);
    }
}