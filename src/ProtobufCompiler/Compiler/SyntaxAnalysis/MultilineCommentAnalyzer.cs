using ProtobufCompiler.Compiler.Errors;
using ProtobufCompiler.Compiler.Extensions;
using ProtobufCompiler.Compiler.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProtobufCompiler.Compiler.SyntaxAnalysis
{
    internal class MultilineCommentAnalyzer : SyntaxAnalyzerBase
    {
        public override NodeResult<Node> Analyze(Queue<Token> tokens)
        {
            var token = tokens.Dequeue(); // Remove the open block comment token
            var commentNode = new Node(NodeType.Comment, token.Lexeme);

            var foundClosing = false;
            var buffer = new List<Token>();
            do
            {
                if (!tokens.Any()) // Case where we run out of tokens before finding.
                {
                    return new NodeResult<Node>(null, new[] { new ParseError("No matching '*\\' found to end comment! Unexpected end of token stream after token : ", token) });
                }

                token = tokens.Dequeue();
                if (_parser.IsMultiLineCommentClose(token.Lexeme))
                {
                    foundClosing = true;
                }
                else
                {
                    buffer.Add(token);
                }
            } while (!foundClosing);

            tokens.DumpEndline();

            var commentString = string.Join(" ", buffer.Select(t =>
            {
                if (t.Type.Equals(TokenType.String)) return t.Lexeme;
                return t.Type.Equals(TokenType.EndLine) ? Environment.NewLine : string.Empty;
            }));

            var commentText = new Node(NodeType.CommentText, commentString);
            commentNode.AddChild(commentText);
            return new NodeResult<Node>(commentNode);
        }
    }
}
