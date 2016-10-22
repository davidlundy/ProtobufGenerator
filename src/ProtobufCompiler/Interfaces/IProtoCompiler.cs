using ProtobufCompiler.Compiler;
using ProtobufCompiler.Compiler.Types;

namespace ProtobufCompiler.Interfaces
{
    public interface IProtoCompiler
    {
        Compilation Compile(string filePath);
    }
}
