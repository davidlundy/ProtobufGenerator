using ProtobufCompiler.Compiler.Errors;
using ProtobufCompiler.Compiler.Extensions;
using ProtobufCompiler.Compiler.Nodes;
using ProtobufCompiler.Extensions;
using ProtobufCompiler.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace ProtobufCompiler.Compiler.SyntaxAnalysis
{
    internal abstract class SyntaxAnalyzerBase : ISyntaxAnalyzer<Node>
    {
        protected Parser _parser = new Parser();

        public abstract NodeResult<Node> Analyze(Queue<Token> tokens);

        protected bool HasTerminator(Queue<Token> tokens)
        {
            if (!tokens.Any()) return false;
            return _parser.IsEmptyStatement(tokens.Peek().Lexeme);
        }

        protected NodeResult<Node> ParseFullIdentifier(Queue<Token> tokens, string forParse)
        {
            var ident = tokens.Peek();
            if (!_parser.IsFullIdentifier(ident.Lexeme))
            {
                return new NodeResult<Node>(null, new[] { new ParseError($"Could not parse full identifier for {forParse} at token ", ident) });
            }
            tokens.Dequeue();
            return new NodeResult<Node>(new Node(NodeType.Identifier, ident.Lexeme));
        }

        protected NodeResult<Node> ParseStringLiteral(Queue<Token> tokens, string forParse)
        {
            var stringLit = tokens.Peek();
            if (!_parser.IsStringLiteral(stringLit.Lexeme))
            {
                return new NodeResult<Node>(null, new[] { new ParseError($"Could not find a string literal for syntax analyzer {forParse} on token ", stringLit) });
            }

            tokens.Dequeue();
            return new NodeResult<Node>(new Node(NodeType.StringLiteral, stringLit.Lexeme.Unquote()));
        }

        protected NodeResult<Node> ParseIdentifier(Queue<Token> tokens, string forParse)
        {
            var ident = tokens.Peek();
            if (!_parser.IsIdentifier(ident.Lexeme))
            {
                return new NodeResult<Node>(null, new[] { new ParseError($"Could not find identifier for syntax analyzer {forParse} on token ", ident) });
            }
            tokens.Dequeue();
            return new NodeResult<Node>(new Node(NodeType.Identifier, ident.Lexeme));
        }

        protected IEnumerable<NodeResult<Node>> ParseStringRange(Queue<Token> tokens)
        {
            var stringRes = new List<string>();
            var token = tokens.Peek();
            while (!token.Lexeme.Equals(";"))
            {
                token = tokens.Dequeue();
                var lexeme = token.Lexeme;
                if (",".Equals(lexeme))
                {
                    token = tokens.Peek();
                    continue;
                }
                if (!_parser.IsStringLiteral(lexeme))
                {
                    if (!stringRes.Any())
                    {
                        return new List<NodeResult<Node>>
                        {
                            new NodeResult<Node>(null, new [] { new ParseError(
                                "Expected string literal before ',' in reserved range ",
                                token) })
                        };
                    }
                    token = tokens.Peek();
                    continue;
                }
                stringRes.Add(lexeme.Unquote());
                token = tokens.Peek();
            }
            return stringRes.Select(t => new NodeResult<Node>(new Node(NodeType.StringLiteral, t)));
        }

        /// <summary>
        /// So we are going to get something like '2,15,9,to,13'
        /// And we need to translate that to 2, 9, 10, 11, 12, 13, 15
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        protected NodeResult<Node> ParseIntegerRange(Queue<Token> tokens)
        {
            var intRes = new Stack<int>();
            var token = tokens.Peek();

            var rangeNode = new Node(NodeType.IntegerRange, token.Lexeme);
            var reservationResult = new NodeResult<Node>(rangeNode);

            // Consume tokens until we hit the endline delimiter
            while (!token.Lexeme.Equals(";"))
            {
                token = tokens.Dequeue();
                var lexeme = token.Lexeme;
                if (",".Equals(lexeme))
                {
                    if (!intRes.Any())
                    {
                        reservationResult.Errors.Add(
                            new ParseError(
                                "Expected integer literal before ',' in reserved range ",
                                token));
                        return reservationResult;
                    }
                    token = tokens.Peek();
                    continue;
                }

                if (_parser.IsDecimalLiteral(lexeme))
                {
                    intRes.Push(int.Parse(lexeme));
                    token = tokens.Peek();
                    continue;
                }

                if (!"to".Equals(lexeme))
                {
                    token = tokens.Peek();
                    continue;
                }

                // So now we are looking ahead at the token after 'to', so we have something like '9 to 11'
                if (!intRes.Any()) // In the case that we found a 'to' but haven't yet found an integer
                {
                    reservationResult.Errors.Add(
                       new ParseError(
                           "Expected integer literal before 'to' in reserved range ",
                           token));
                    return reservationResult;
                }

                var startRangeAt = intRes.Pop(); // Go get the last integer read, e.g. 9
                var nextToken = tokens.Peek(); // Look ahead for the next integer, e.g. 11
                if (!_parser.IsDecimalLiteral(nextToken.Lexeme)) // If the next token isn't an integer create Error
                {
                    reservationResult.Errors.Add(
                        new ParseError(
                            "Expected integer literal after 'to' in reserved range ",
                            nextToken));
                    return reservationResult;
                }

                // If we don't have an error go ahead and remove the token and use it to find the end range.
                nextToken = tokens.Dequeue();
                var endRangeAt = int.Parse(nextToken.Lexeme);

                // Now push all the integers in the range onto the stack.
                var rangeLength = endRangeAt - startRangeAt + 1;
                foreach (var elem in Enumerable.Range(startRangeAt, rangeLength))
                {
                    intRes.Push(elem);
                }

                // If we've got this far, set the token for the While comparison to the next.
                token = tokens.Peek();
            }

            // Now that we've hit an Endline or ';' terminator, return.
            var childNodes = intRes.Select(t => new Node(NodeType.IntegerLiteral, t.ToString())).Reverse();

            rangeNode.Children.AddRange(childNodes);

            return reservationResult;
        }
    }
}
