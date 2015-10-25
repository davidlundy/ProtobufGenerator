using System;
using System.Collections.Generic;
using System.Linq;
using ProtobufCompiler.Compiler.Nodes;
using ProtobufCompiler.Interfaces;

namespace ProtobufCompiler.Compiler
{
    internal class SyntaxAnalyzer : ISyntaxAnalyzer
    {
        private readonly Queue<Token> _tokens;
        private readonly Parser _parser;
        private readonly IList<ParseError> _errors;

        internal SyntaxAnalyzer(Queue<Token> tokens)
        {
            _tokens = tokens;
            _parser = new Parser();
            _errors = new List<ParseError>();
        }

        public RootNode Analyze()
        {
            var root = new RootNode();
            while (_tokens.Any())
            {
                var topLevelStatement = ParseTopLevelStatement(root);
                if (topLevelStatement != null) root.AddChild(topLevelStatement);
            }
            return root;
        }

        internal Node ParseTopLevelStatement(Node root)
        {
            var token = _tokens.Peek();
            if (!(token.Type.Equals(TokenType.Comment) || token.Type.Equals(TokenType.Id)))
            {
                _errors.Add(new ParseError("Found an invalid top level statement at token ", token));
                return null;
            }

            var lexeme = token.Lexeme;

            if (_parser.IsInlineComment(lexeme)) return ParseInlineComment();

            if (_parser.IsMultiLineCommentOpen(lexeme)) return ParseMultiLineComment();

            if (_parser.IsSyntax(lexeme)) return ParseSyntax();

            if (_parser.IsImport(lexeme)) return ParseImport();

            if (_parser.IsPackage(lexeme)) return ParsePackage();

            if (_parser.IsOption(lexeme)) return ParseOption();

            if (_parser.IsEnum(lexeme)) return ParseEnum();

            if (_parser.IsService(lexeme)) return ParseService();

            if (_parser.IsMessage(lexeme)) return ParseMessage();

            _errors.Add(new ParseError("Found an invalid top level statement at token ", token));
            return null;
        }

        private Node ParseInlineComment()
        {
            var token = _tokens.Dequeue(); // Remove the line comment token
            
            var commentNode = new Node(NodeType.Comment, token.Lexeme);
            var commentText = new Node(NodeType.CommentText, DumpStringToEndLine());
            commentNode.AddChild(commentText);
            return commentNode;
        }

        private Node ParseMultiLineComment()
        {
            var token = _tokens.Dequeue(); // Remove the open block comment token
            var commentNode = new Node(NodeType.Comment, token.Lexeme);
            var commentText = new Node(NodeType.CommentText, CollectBlockComment());
            commentNode.AddChild(commentText);
            return commentNode;
        }

        private Node ParseSyntax()
        {
            var syntax = _tokens.Dequeue(); // Pop off syntax token. 

            var assignment = _tokens.Dequeue();
            if (!_parser.IsAssignment(assignment.Lexeme))
            {
                _errors.Add(new ParseError("Expected an assignment after syntax token, found ", assignment));
                return null;
            }
            var proto3 = _tokens.Dequeue();

            if (!_parser.IsStringLiteral(proto3.Lexeme))
            {
                _errors.Add(new ParseError("Expected a string literal after syntax assignment, found token ", proto3));
                return null;
            }

            var terminator = _tokens.Dequeue();

            if (!_parser.IsEmptyStatement(terminator.Lexeme))
            {
                _errors.Add(new ParseError("Expected terminating `;` after syntax declaration, found token ", terminator));
                return null;
            }

            var syntaxNode = new Node(NodeType.Syntax, syntax.Lexeme);
            var syntaxValue = new Node(NodeType.StringLiteral, proto3.Lexeme);
            syntaxNode.AddChild(syntaxValue);

            var trailing = _tokens.Peek();
            if (_parser.IsInlineComment(trailing.Lexeme))
            {
                var commentNode = ParseInlineComment();
                syntaxNode.AddChild(commentNode);
            }
            else if (trailing.Type.Equals(TokenType.EndLine)) _tokens.Dequeue(); // Dump the endline

            return syntaxNode;
        }

        private Node ParseImport()
        {
            DumpStringToEndLine();
            return null;
        }

        private Node ParsePackage()
        {
            DumpStringToEndLine();
            return null;
        }

        private Node ParseOption()
        {
            DumpStringToEndLine();
            return null;
        }

        private Node ParseEnum()
        {

            return null;
        }

        private Node ParseService()
        {
            return null;
        }

        private Node ParseMessage()
        {
            return null;
        }



        private string DumpStringToEndLine()
        {
            Token token;
            var buffer = new List<Token>();
            do
            {
                token = _tokens.Dequeue();
                buffer.Add(token);
                token = _tokens.Peek();
            } while (!token.Type.Equals(TokenType.EndLine));

            if (_tokens.Any() && _tokens.Peek().Type.Equals(TokenType.EndLine))
                _tokens.Dequeue(); // Dump the trailing EOL

            return string.Join(" ", buffer.Select(l => l.Lexeme));
        }

        private string CollectBlockComment()
        {
            var foundClosing = false;
            var buffer = new List<Token>();
            do
            {
                var token = _tokens.Dequeue();
                if (_parser.IsMultiLineCommentClose(token.Lexeme))
                {
                    foundClosing = true;
                }
                else
                {
                    buffer.Add(token);
                }
            } while (!foundClosing);

            if (_tokens.Any() && _tokens.Peek().Type.Equals(TokenType.EndLine))
                _tokens.Dequeue(); // Dump the trailing EOL

            return string.Join(" ", buffer.Select(token =>
            {
                if (token.Type.Equals(TokenType.String)) return token.Lexeme;
                return token.Type.Equals(TokenType.EndLine) ? Environment.NewLine : string.Empty;
            }));
        }
    }
}
