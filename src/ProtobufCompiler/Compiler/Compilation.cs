using ProtobufCompiler.Compiler.Errors;
using ProtobufCompiler.Compiler.Types;
using System.Collections.Generic;

namespace ProtobufCompiler.Compiler
{
    public class Compilation
    {
        public FileDescriptor FileDescriptor { get; set; }

        public ICollection<CompilerError> Errors { get; set; }
    }
}