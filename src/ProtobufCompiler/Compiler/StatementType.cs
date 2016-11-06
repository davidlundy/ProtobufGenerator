namespace ProtobufCompiler.Compiler
{
    internal enum StatementType
    {
        InlineComment,
        MultilineComment,
        Syntax,
        Import,
        Package,
        Option,
        Enumeration,
        Service,
        Message,
        NotFound
    }
}
