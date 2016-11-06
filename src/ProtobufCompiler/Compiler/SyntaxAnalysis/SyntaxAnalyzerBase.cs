using ProtobufCompiler.Compiler.Errors;
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
    }
}
