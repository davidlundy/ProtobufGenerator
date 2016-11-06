using ProtobufCompiler.Compiler.Errors;
using System.Collections.Generic;

namespace ProtobufCompiler.Compiler.Nodes
{
    public class NodeResult<T> where T: Node
    {
        public ICollection<ParseError> Errors { get; set; }
        public T Node { get; set; }

        public NodeResult(T node, ICollection<ParseError> errors = null)
        {
            Node = node;
            Errors = errors ?? new List<ParseError>();
        }
    }
}
