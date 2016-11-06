using ProtobufCompiler.Compiler.Errors;
using ProtobufCompiler.Compiler.Extensions;
using ProtobufCompiler.Compiler.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProtobufCompiler.Compiler.SyntaxAnalysis
{
    internal class MessageStatementAnalyzer : SyntaxAnalyzerBase
    {
        public override NodeResult<Node> Analyze(Queue<Token> tokens)
        {
            var msgTag = tokens.Dequeue();

            var msgNode = new Node(NodeType.Message, msgTag.Lexeme);

            var msgName = ParseIdentifier(tokens, nameof(MessageStatementAnalyzer));
            if (msgName.Errors.Any())
            {
                return msgName;
            }

            msgNode.AddChild(msgName.Node);

            var openBrack = tokens.Dequeue();
            if (!openBrack.Type.Equals(TokenType.Control) || !openBrack.Lexeme[0].Equals('{'))
            {
                return new NodeResult<Node>(null, new[] { new ParseError(
                        $"Expected to find open bracket on line {msgTag.Line} for message token ",
                        msgTag) });
            }

            //ScoopComment(msgNode);
            tokens.DumpEndline();

            var next = tokens.Peek();
            while (!next.Type.Equals(TokenType.Control) && !next.Lexeme[0].Equals('}'))
            {
                // Some of these may be null returns. That's ok. AddChildren will ignore.
                var reservation = ParseReservation();
                var fieldNode = ParseMessageField();
                var nestedMessage = ParseMessage();
                var nestedEnum = ParseEnum();
                var oneOf = ParseOneOfField();
                var map = ParseMapField();

                msgNode.AddChildren(reservation, fieldNode, nestedMessage, nestedEnum, oneOf, map);

                if (tokens.Any())
                {
                    next = tokens.Peek();
                    continue;
                }

                return new NodeResult<Node>(null, new[] { new ParseError("Premature end of tokens found while parsing message. Last token ", next) });
            }

            tokens.Dequeue(); // Dump the }
            tokens.DumpEndline();

            return new NodeResult<Node>(msgNode);
        }

        private NodeResult<Node> ParseReservation(Queue<Token> tokens)
        {
            var reserveTag = tokens.Peek();
            if (!reserveTag.Lexeme.Equals("reserved")) return null;
            tokens.Dequeue();

            var reservedNode = new Node(NodeType.Reserved, reserveTag.Lexeme);

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
                    var isTerminated = TerminateSingleLineStatement();
                    if (!isTerminated)
                    {
                        return null;
                    }
                    //ScoopComment(reservedNode);
                    tokens.DumpEndline();
                    return reservedNode;
                }
            }

            if (_parser.IsDecimalLiteral(next.Lexeme))
            {
                var intRange = ParseIntegerRange().ToArray();

                if (intRange.Any())
                {
                    reservedNode.AddChildren(intRange);
                    var isTerminated = TerminateSingleLineStatement();
                    if (!isTerminated)
                    {
                        return null;
                    }
                    ScoopComment(reservedNode);
                    tokens.DumpEndline();
                    return reservedNode;
                }
            }

            var existingString = tokens.DumpStringToEndline();
            _errors.Add(
                    new ParseError(
                        $"Could not find a valid reservation on line {reserveTag.Line}. Found: {existingString}",
                        reserveTag));
            return null;
        }
    }
}
