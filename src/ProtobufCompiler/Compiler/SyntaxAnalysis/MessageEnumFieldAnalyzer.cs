﻿using ProtobufCompiler.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProtobufCompiler.Compiler.Nodes;

namespace ProtobufCompiler.Compiler.SyntaxAnalysis
{
    internal class MessageEnumFieldAnalyzer : SyntaxAnalyzerBase
    {
        public override NodeResult<Node> Analyze(Queue<Token> tokens)
        {
            throw new NotImplementedException();
        }
    }
}
