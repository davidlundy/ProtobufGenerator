using ProtobufCompiler.Compiler;

namespace ProtobufCompiler.Interfaces
{
    public interface IProtoCompiler
    {
        Compilation Compile(string filePath);
    }
}