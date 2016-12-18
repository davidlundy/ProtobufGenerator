using ProtobufCompiler.Compiler.Enumerations;
using ProtobufCompiler.Compiler.Errors;
using ProtobufCompiler.Compiler.Extensions;
using ProtobufCompiler.Compiler.Nodes;
using ProtobufCompiler.Compiler.SyntaxAnalysis;
using ProtobufCompiler.Extensions;
using ProtobufCompiler.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProtobufCompiler.Compiler
{
    internal class RootSyntaxAnalyzer : ISyntaxAnalyzer<RootNode>
    {
        private readonly Queue<Token> _tokens;
        private readonly Parser _parser;
        private readonly ICollection<ParseError> _errors;
        private readonly IDictionary<RootStatementType, ISyntaxAnalyzer<Node>> _analyzers;

        internal RootSyntaxAnalyzer(IDictionary<RootStatementType, ISyntaxAnalyzer<Node>> analyzers = null)
        {
            _parser = new Parser();
            _errors = new List<ParseError>();
            _analyzers = analyzers ?? new Dictionary<RootStatementType, ISyntaxAnalyzer<Node>>
            {
                { RootStatementType.InlineComment, new InlineCommentAnalyzer() },
                { RootStatementType.MultilineComment, new MultilineCommentAnalyzer() },
                { RootStatementType.Syntax, new SyntaxAnalyzer() },
                { RootStatementType.Import, new ImportAnalyzer() },
                { RootStatementType.Enumeration, new EnumAnalyzer() },
                { RootStatementType.Message, new MessageAnalyzer() },
                { RootStatementType.Option, new OptionAnalyzer() },
                { RootStatementType.Package, new PackageAnalyzer() },
                { RootStatementType.Service, new ServiceAnalyzer() }
            };
        }

        public NodeResult<RootNode> Analyze(Queue<Token> tokens)
        {
            var root = new RootNode();
            while (tokens.Any())
            {
                tokens.SkipEmptyLines();
                var token = tokens.Peek();

                if (!(token.Type.Equals(TokenType.Comment) || token.Type.Equals(TokenType.Id)))
                {
                    _errors.Add(new ParseError("Found an invalid top level statement at token ", token));
                    tokens.BurnLine();
                    token = tokens.Peek();
                }

                var statementType = _parser.ParseStatementType(token.Lexeme);

                if(statementType == RootStatementType.NotFound)
                {
                    // In the case that we can't find a valid top level statement burn the line and log the error.
                    _errors.Add(new ParseError($"Found an invalid top level statement", token));
                    tokens.BurnLine();
                }

                var nodeResult = _analyzers[statementType].Analyze(tokens);

                if (nodeResult.Errors.Any())
                {
                    _errors.AddRange(nodeResult.Errors);
                    continue;
                }

                root.AddChild(nodeResult.Node);
            }

            return new NodeResult<RootNode>(root, _errors);
        }

        private void ScoopComment(Node syntaxNode)
        {
            throw new NotImplementedException();
        }

        private Node ParseStringLiteral()
        {
            var stringLit = _tokens.Peek();
            if (!_parser.IsStringLiteral(stringLit.Lexeme)) return null;
            _tokens.Dequeue();
            return new Node(NodeType.StringLiteral, stringLit.Lexeme.Unquote());
        }

        private Node ParseIdentifier()
        {
            var ident = _tokens.Peek();
            if (!_parser.IsIdentifier(ident.Lexeme)) return null;
            _tokens.Dequeue();
            return new Node(NodeType.Identifier, ident.Lexeme);
        }


        private Node ParseEnum()
        {
            var enumTag = _tokens.Peek();
            if (!enumTag.Lexeme.Equals("enum")) return null;
            _tokens.Dequeue();

            var enumNode = new Node(NodeType.Enum, enumTag.Lexeme);

            var msgName = ParseIdentifier();
            if (ReferenceEquals(msgName, null))
            {
                _errors.Add(
                    new ParseError(
                        $"Could not find a message name on line {enumTag.Line} for message token ",
                        enumTag));
                return null;
            }

            enumNode.AddChild(msgName);

            var openBrack = _tokens.Dequeue();
            if (!openBrack.Type.Equals(TokenType.Control) || !openBrack.Lexeme[0].Equals('{'))
            {
                _errors.Add(
                    new ParseError(
                        $"Expected to find open bracket on line {enumTag.Line} for message token ",
                        enumTag));
                return null;
            }

            ScoopComment(enumNode);
            _tokens.DumpEndline();

            var next = _tokens.Peek();
            while (!next.Type.Equals(TokenType.Control) && !next.Lexeme[0].Equals('}'))
            {
                var fieldNode = ParseEnumField();
                enumNode.AddChild(fieldNode);
                if (_tokens.Any()) next = _tokens.Peek();
            }

            _tokens.Dequeue();
            _tokens.DumpEndline();

            return enumNode;
        }

        private Node ParseEnumField()
        {
            if (!_tokens.Any()) return null;
            var openToken = _tokens.Peek();
            var fieldNode = new Node(NodeType.EnumField, openToken.Lexeme);

            var name = ParseIdentifier();
            fieldNode.AddChild(name);

            var assignment = _tokens.Dequeue();
            if (!_parser.IsAssignment(assignment.Lexeme))
            {
                _errors.Add(new ParseError($"Expected an assignment after enum field name token on line {openToken.Line}, found ", assignment));
                return null;
            }

            var fieldValue = _tokens.Dequeue();
            if (!_parser.IsIntegerLiteral(fieldValue.Lexeme))
            {
                _errors.Add(new ParseError($"Expected a field value after assignment token on line {openToken.Line}, found ", fieldValue));
                return null;
            }
            fieldNode.AddChild(new Node(NodeType.FieldNumber, fieldValue.Lexeme));


            if (!HasTerminator(_tokens))
            {
                return null;
            }
            ScoopComment(fieldNode);
            _tokens.DumpEndline();

            return fieldNode;
        }

        private Node ParseService()
        {
            return null;
        }

        private bool HasTerminator(Queue<Token> tokens)
        {
            if (!tokens.Any()) return false;
            return _parser.IsEmptyStatement(tokens.Peek().Lexeme);
        }


        private IEnumerable<Node> ParseStringRange()
        {
            var stringRes = new List<string>();
            var token = _tokens.Peek();
            while (!token.Lexeme.Equals(";"))
            {
                token = _tokens.Dequeue();
                var lexeme = token.Lexeme;
                if (",".Equals(lexeme))
                {
                    token = _tokens.Peek();
                    continue;
                }
                if (!_parser.IsStringLiteral(lexeme))
                {
                    if (!stringRes.Any())
                    {
                        _errors.Add(
                            new ParseError(
                                "Expected string literal before ',' in reserved range ",
                                token));
                        return new List<Node>();
                    }
                    token = _tokens.Peek();
                    continue;
                }
                stringRes.Add(lexeme.Unquote());
                token = _tokens.Peek();
            }
            return stringRes.Select(t => new Node(NodeType.StringLiteral, t));
        }

        private Node ParseOneOfField()
        {
            var oneOfTag = _tokens.Peek();
            if (!oneOfTag.Lexeme.Equals("oneof")) return null;
            _tokens.Dequeue();

            var msgNode = new Node(NodeType.OneOfField, oneOfTag.Lexeme);

            var msgName = ParseIdentifier();
            if (ReferenceEquals(msgName, null))
            {
                _errors.Add(
                    new ParseError(
                        $"Could not find a oneof name on line {oneOfTag.Line} for message token ",
                        oneOfTag));
                return null;
            }

            var openBrack = _tokens.Dequeue();
            if (!openBrack.Type.Equals(TokenType.Control) || !openBrack.Lexeme[0].Equals('{'))
            {
                _errors.Add(
                    new ParseError(
                        $"Expected to find open bracket on line {oneOfTag.Line} for message token ",
                        oneOfTag));
                return null;
            }

            msgNode.AddChild(msgName);

            ScoopComment(msgNode);
            _tokens.DumpEndline();

            var next = _tokens.Peek();
            while (!next.Type.Equals(TokenType.Control) && !next.Lexeme[0].Equals('}'))
            {
                // Some of these may be null returns. That's ok. AddChildren will ignore.
                var fieldNode = ParseMessageField();
                var oneOf = ParseOneOfField();

                msgNode.AddChildren(fieldNode, oneOf);

                if (_tokens.Any()) next = _tokens.Peek();
            }

            _tokens.Dequeue(); // Dump the }
            _tokens.DumpEndline();

            return msgNode;
        }

        private Node ParseMapField()
        {
            var mapTag = _tokens.Peek();
            if (!mapTag.Lexeme.Equals("map")) return null;
            _tokens.Dequeue();

            var mapNode = new Node(NodeType.Map, mapTag.Lexeme);

            var openAngle = _tokens.Dequeue();
            if (!"<".Equals(openAngle.Lexeme))
            {
                _errors.Add(
                   new ParseError(
                       $"Expected open angle bracket at column {openAngle.Column} on line {openAngle.Line} for map definition ",
                       mapTag));
                return null;
            }

            var key = _tokens.Dequeue();
            if (!key.Lexeme.IsMapKeyType())
            {
                _errors.Add(
                   new ParseError(
                       $"Expected valid map key type at column {key.Column} on line {key.Line} for map definition. Found {key.Lexeme} ",
                       mapTag));
                return null;
            }
            var mapKey = new Node(NodeType.MapKey, key.Lexeme);

            var comma = _tokens.Dequeue();
            if (!",".Equals(comma.Lexeme))
            {
                _errors.Add(
                   new ParseError(
                       $"Expected open comma at column {comma.Column} on line {comma.Line} for map definition ",
                       mapTag));
                return null;
            }

            var value = ParseMapValueType();

            if (ReferenceEquals(null, value))
            {
                _errors.Add(
                    new ParseError(
                        "Expected a user or value type for map definition ",
                        mapTag));
                return null;
            }

            var closeAngle = _tokens.Dequeue();
            if (!">".Equals(closeAngle.Lexeme))
            {
                _errors.Add(
                   new ParseError(
                       $"Expected close angle bracket at column {closeAngle.Column} on line {closeAngle.Line} for map definition ",
                       mapTag));
                return null;
            }

            var msgName = ParseIdentifier();
            if (ReferenceEquals(msgName, null))
            {
                _errors.Add(
                    new ParseError(
                        $"Could not find a oneof name on line {mapTag.Line} for message token ",
                        mapTag));
                return null;
            }

            var assignment = _tokens.Dequeue();
            if (!_parser.IsAssignment(assignment.Lexeme))
            {
                _errors.Add(new ParseError($"Expected an assignment after field name token on line {mapTag.Line}, found ", assignment));
                return null;
            }

            var fieldValue = _tokens.Dequeue();
            if (!_parser.IsIntegerLiteral(fieldValue.Lexeme))
            {
                _errors.Add(new ParseError($"Expected a field value after assignment token on line {mapTag.Line}, found ", fieldValue));
                return null;
            }
            var fieldNumber = new Node(NodeType.FieldNumber, fieldValue.Lexeme);

            mapNode.AddChildren(msgName, mapKey, value, fieldNumber);

            if (!HasTerminator(_tokens))
            {
                return null;
            }
            ScoopComment(mapNode);
            _tokens.DumpEndline();

            return mapNode;
        }

        private Node ParseRepeated()
        {
            var repeated = _tokens.Peek();
            if (!_parser.IsRepeated(repeated.Lexeme)) return null;

            _tokens.Dequeue();
            return new Node(NodeType.Repeated, repeated.Lexeme);
        }

        private Node ParseMapValueType()
        {
            var type = _tokens.Peek();
            if (!_parser.IsBasicType(type.Lexeme) && !_parser.IsFullIdentifier(type.Lexeme)) return null;

            _tokens.Dequeue();
            return new Node(NodeType.MapValue, type.Lexeme);
        }

        private Node ParseBasicType()
        {
            var type = _tokens.Peek();
            if (!_parser.IsBasicType(type.Lexeme)) return null;

            _tokens.Dequeue();
            return new Node(NodeType.Type, type.Lexeme);
        }

        private Node ParseUserType()
        {
            var usertype = _tokens.Peek();
            if (!_parser.IsFullIdentifier(usertype.Lexeme)) return null;

            _tokens.Dequeue();
            return new Node(NodeType.UserType, usertype.Lexeme);
        }

        private Node ParseMessageField()
        {
            if (!_tokens.Any()) return null;
            var openToken = _tokens.Peek();
            var lexeme = openToken.Lexeme;
            if (!_parser.IsFieldStart(lexeme)) return null;
            var fieldNode = new Node(NodeType.Field, openToken.Lexeme);

            var repeated = ParseRepeated();
            fieldNode.AddChild(repeated);

            var basicType = ParseBasicType();
            fieldNode.AddChild(basicType);

            if (ReferenceEquals(basicType, null))
            {
                fieldNode.AddChild(ParseUserType());
            }

            var name = ParseIdentifier();
            fieldNode.AddChild(name);

            var assignment = _tokens.Dequeue();
            if (!_parser.IsAssignment(assignment.Lexeme))
            {
                _errors.Add(new ParseError($"Expected an assignment after field name token on line {openToken.Line}, found ", assignment));
                return null;
            }

            var fieldValue = _tokens.Dequeue();
            if (!_parser.IsIntegerLiteral(fieldValue.Lexeme))
            {
                _errors.Add(new ParseError($"Expected a field value after assignment token on line {openToken.Line}, found ", fieldValue));
                return null;
            }
            fieldNode.AddChild(new Node(NodeType.FieldNumber, fieldValue.Lexeme));

            
            if (!HasTerminator(_tokens))
            {
                return null;
            }

            ScoopComment(fieldNode);
            _tokens.DumpEndline();

            return fieldNode;
        }

        
    }
}