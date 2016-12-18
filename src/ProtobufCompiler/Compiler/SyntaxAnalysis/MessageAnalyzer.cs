using ProtobufCompiler.Compiler.Enumerations;
using ProtobufCompiler.Compiler.Errors;
using ProtobufCompiler.Compiler.Extensions;
using ProtobufCompiler.Compiler.Nodes;
using ProtobufCompiler.Interfaces;
using ProtobufCompiler.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace ProtobufCompiler.Compiler.SyntaxAnalysis
{
    internal class MessageAnalyzer : SyntaxAnalyzerBase
    {
        private IDictionary<MessageStatementType, ISyntaxAnalyzer<Node>> _analyzers;

        public MessageAnalyzer(IDictionary<MessageStatementType, ISyntaxAnalyzer<Node>> analyzers = null)
        {
            _analyzers = analyzers ?? new Dictionary<MessageStatementType, ISyntaxAnalyzer<Node>>
            {
                { MessageStatementType.Field, new MessageFieldAnalyzer() },
                { MessageStatementType.NestedMessage, new MessageAnalyzer() },
                { MessageStatementType.Map, new MessageMapAnalyzer() },
                { MessageStatementType.Enum, new MessageEnumFieldAnalyzer() },
                { MessageStatementType.Reservation, new ReservationAnalyzer() },
                { MessageStatementType.OneOf, new OneOfAnalyzer() }
            };
        }

        public override NodeResult<Node> Analyze(Queue<Token> tokens)
        {
            var msgTag = tokens.Dequeue();

            var msgNode = new Node(NodeType.Message, msgTag.Lexeme);
            var analysisResult = new NodeResult<Node>(msgNode);

            var msgName = ParseIdentifier(tokens, nameof(MessageAnalyzer));
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
                var fieldType = _parser.ParseMessageStatementType(next.Lexeme);

                if(fieldType == MessageStatementType.NotFound)
                {
                    analysisResult.Errors.Add(new ParseError(
                        $"Expected to find a valid field definition on line {next.Line} for token ",
                        next));
                    tokens.BurnLine();
                    continue;
                }

                var childNodeResult = _analyzers[fieldType].Analyze(tokens);

                if (childNodeResult.Errors.Any())
                {
                    analysisResult.Errors.AddRange(childNodeResult.Errors);
                    continue;
                }

                msgNode.AddChild(childNodeResult.Node);

                if (!tokens.Any())
                {
                    analysisResult.Errors.Add(new ParseError("Premature end of tokens found while parsing message.", next));
                }

                next = tokens.Peek();
            }

            tokens.Dequeue(); // Dump the }
            tokens.DumpEndline();

            return analysisResult;
        }

        
    }
}
