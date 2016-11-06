using ProtobufCompiler.Compiler.Errors;
using ProtobufCompiler.Compiler.Extensions;
using ProtobufCompiler.Compiler.Nodes;
using ProtobufCompiler.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProtobufCompiler.Compiler.SyntaxAnalysis
{
    internal class OptionStatementAnalyzer : SyntaxAnalyzerBase
    {
        public override NodeResult<Node> Analyze(Queue<Token> tokens)
        {
            var optionTag = tokens.Dequeue();
            var optionNode = new Node(NodeType.Option, optionTag.Lexeme);

            var optionName = ParseFullIdentifier(tokens, nameof(OptionStatementAnalyzer));

            if (optionName.Errors.Any())
            {
                tokens.BurnLine();
                return optionName;
            }

            optionNode.AddChild(optionName.Node);

            var assignment = tokens.Dequeue();
            if (!_parser.IsAssignment(assignment.Lexeme))
            {
                tokens.BurnLine();
                return new NodeResult<Node>(null, new[] { new ParseError($"Expected an assignment after option name token on line {optionTag.Line}, found ", assignment) });
            }

            var optionValue = ParseStringLiteral(tokens, nameof(OptionStatementAnalyzer));
            if (optionValue.Errors.Any())
            {
                tokens.BurnLine();
                return optionValue;
            }
            optionNode.AddChild(optionValue.Node);

            if (!HasTerminator(tokens))
            {
                tokens.BurnLine();
                return new NodeResult<Node>(null, new[] { new ParseError("Required terminator is missing for statement. Found token ", optionTag) });
            }

            tokens.Dequeue(); // The terminator token; i.e. ';'

            tokens.DumpEndline();

            return new NodeResult<Node>(optionNode);
        }
    }
}
