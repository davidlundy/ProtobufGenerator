using System.Collections.Generic;
using System.Linq;
using ProtobufCompiler.Compiler.Nodes;
using ProtobufCompiler.Compiler.Extensions;
using ProtobufCompiler.Compiler.Errors;

namespace ProtobufCompiler.Compiler.SyntaxAnalysis
{
    internal class ReservationAnalyzer : SyntaxAnalyzerBase
    {
        public override NodeResult<Node> Analyze(Queue<Token> tokens)
        {
            var reserveTag = tokens.Peek();
            if (!reserveTag.Lexeme.Equals("reserved")) return null;
            tokens.Dequeue();

            var reservedNode = new Node(NodeType.Reserved, reserveTag.Lexeme);
            var reservationResult = new NodeResult<Node>(reservedNode);

            var next = tokens.Peek();
            if (_parser.IsStringLiteral(next.Lexeme))
            {
                var nameRangeResults = ParseStringRange(tokens);
                var cumulativeErrors = nameRangeResults.Where(t => t.Errors.Any());

                if (cumulativeErrors.Any())
                {
                    return cumulativeErrors.First();
                }

                var nameRange = nameRangeResults.Select(t => t.Node).ToArray();

                if (nameRange.Any())
                {
                    reservedNode.AddChildren(nameRange);
                    if (!HasTerminator(tokens))
                    {
                        var whatsThere = tokens.Peek();
                        reservationResult.Errors.Add(new ParseError("Required terminator is missing for statement. Found token ", whatsThere));
                        tokens.BurnLine();
                    }
                }
            }

            if (_parser.IsDecimalLiteral(next.Lexeme))
            {
                var rangeResult = ParseIntegerRange(tokens);
                var rangeNodes = rangeResult.Node.Children;

                if (!rangeNodes.Any())
                {
                    reservationResult.Errors.Add(new ParseError("Could not find an integer range for reservation ", reserveTag));
                }

                reservedNode.AddChildren(rangeResult.Node);
                if (!HasTerminator(tokens))
                {
                    var whatsThere = tokens.Peek();
                    reservationResult.Errors.Add(new ParseError("Required terminator is missing for statement. Found token ", whatsThere));
                    tokens.BurnLine();
                }
            }

            return reservationResult;
        }
    }
}
