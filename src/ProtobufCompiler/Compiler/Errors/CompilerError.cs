namespace ProtobufCompiler.Compiler.Errors
{
    internal abstract class CompilerError
    {
        internal string Message { get; }

        internal CompilerError(string message)
        {
            Message = message;
        }
    }
}
