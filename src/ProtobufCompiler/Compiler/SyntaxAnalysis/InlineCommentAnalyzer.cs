using ProtobufCompiler.Compiler.Extensions;
using ProtobufCompiler.Compiler.Nodes;
using System.Collections.Generic;

namespace ProtobufCompiler.Compiler.SyntaxAnalysis
{
    internal class InlineCommentAnalyzer : SyntaxAnalyzerBase
    {
        public override NodeResult<Node> Analyze(Queue<Token> tokens)
        {
            var token = tokens.Dequeue(); // Remove the line comment token

            var commentNode = new Node(NodeType.Comment, token.Lexeme);
            var commentText = new Node(NodeType.CommentText, tokens.DumpStringToEndline());
            commentNode.AddChild(commentText);
            return new NodeResult<Node>(commentNode);
        }
    }
}
