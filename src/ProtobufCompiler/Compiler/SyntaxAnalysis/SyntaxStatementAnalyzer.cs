using ProtobufCompiler.Compiler.Errors;
using ProtobufCompiler.Compiler.Extensions;
using ProtobufCompiler.Compiler.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace ProtobufCompiler.Compiler.SyntaxAnalysis
{
    internal class SyntaxStatementAnalyzer : SyntaxAnalyzerBase
    {
        public override NodeResult<Node> Analyze(Queue<Token> tokens)
        {
            var syntax = tokens.Dequeue(); // Pop off syntax token.

            var assignment = tokens.Dequeue();
            if (!_parser.IsAssignment(assignment.Lexeme))
            {
                tokens.BurnLine();
                return new NodeResult<Node>(null, new[] { new ParseError("Expected an assignment after syntax token, found ", assignment) });
            }

            var proto3 = ParseStringLiteral(tokens, nameof(SyntaxStatementAnalyzer));

            if (proto3.Errors.Any())
            {
                tokens.BurnLine();
                return proto3;
            }

            if (!HasTerminator(tokens))
            {
                var whatsThere = tokens.Peek();
                tokens.BurnLine();
                return new NodeResult<Node>(null, new[] { new ParseError("Required terminator is missing for statement. Found token ", whatsThere) });
            }

            tokens.Dequeue(); // The terminator token; i.e. ';'

            var syntaxNode = new Node(NodeType.Syntax, syntax.Lexeme);
            syntaxNode.AddChild(proto3.Node);

            tokens.DumpEndline();

            return new NodeResult<Node>(syntaxNode);
        }
    }
}
