using ProtobufCompiler.Compiler.Errors;
using ProtobufCompiler.Compiler.Extensions;
using ProtobufCompiler.Compiler.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace ProtobufCompiler.Compiler.SyntaxAnalysis
{
    internal class ImportStatementAnalyzer : SyntaxAnalyzerBase
    {
        public override NodeResult<Node> Analyze(Queue<Token> tokens)
        {
            var importTag = tokens.Dequeue();
            var importNode = new Node(NodeType.Import, importTag.Lexeme);

            var mod = tokens.Peek();

            // Slight detour if we have a modifier. Otherwise continue.
            if (_parser.IsImportModifier(mod.Lexeme))
            {
                tokens.Dequeue();
                var modifier = new Node(NodeType.ImportModifier, mod.Lexeme);
                importNode.AddChild(modifier);
            }

            var importValue = ParseStringLiteral(tokens, nameof(ImportStatementAnalyzer));

            if (importValue.Errors.Any())
            {
                tokens.BurnLine();
                return importValue;
            }
            importNode.AddChild(importValue.Node);

            if (!HasTerminator(tokens))
            {
                tokens.BurnLine();
                return new NodeResult<Node>(null, new[] { new ParseError("Required terminator is missing for statement. Found token ", importTag) });
            }

            tokens.Dequeue(); // The terminator token; i.e. ';'

            tokens.DumpEndline();

            return new NodeResult<Node>(importNode);
        }
    }
}
