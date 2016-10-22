using ProtobufCompiler.Compiler.Errors;
using ProtobufCompiler.Compiler.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProtobufCompiler.Compiler
{
    public class Compilation
    {
        public FileDescriptor FileDescriptor { get; set; }

        public ICollection<CompilerError> Errors { get; set; }
    }
}
