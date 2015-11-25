﻿using System.Collections.Generic;
using ProtobufCompiler.Compiler.Errors;
using ProtobufCompiler.Extensions;

namespace ProtobufCompiler.Compiler.Nodes
{
    internal class RootNode : Node
    {
        internal IList<string> Imports { get; }
        internal IList<string> Exports { get; }
        internal IList<ParseError> Errors { get; }

        internal RootNode() : base(NodeType.Root, string.Empty)
        {
            Imports = new List<string>();
            Exports = new List<string>();
            Errors = new List<ParseError>();
        }

        internal void AddErrors(IEnumerable<ParseError> errors)
        {
            Errors.AddRange(errors);
        }
    }
}
