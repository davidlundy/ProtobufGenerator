using ProtobufCompiler.Polyfill;

namespace ProtobufCompiler.Compiler.Errors
{
    [ExcludeFromCodeCoverage("Error classes are immutable data holders.")]
    public abstract class CompilerError
    {
        internal string Message { get; }

        internal CompilerError(string message)
        {
            Message = message;
        }
    }
}