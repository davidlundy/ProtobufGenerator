using System;
using System.Collections.Generic;
using System.Linq;
using ProtobufCompiler.Compiler.Nodes;
using ProtobufCompiler.Extensions;
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

        private void BurnLine()
        {
            
            Token token;
            do
            {
                if (!_tokens.Any()) return;
                token = _tokens.Dequeue();
            } while (!token.Type.Equals(TokenType.EndLine));
        }

        internal Node ParseTopLevelStatement(Node root)
        {
            var token = _tokens.Peek();
            if (!(token.Type.Equals(TokenType.Comment) || token.Type.Equals(TokenType.Id)))
            {
                _errors.Add(new ParseError("Found an invalid top level statement at token ", token));
                BurnLine();
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
            var proto3 = ParseStringLiteral();

            if (ReferenceEquals(proto3, null))
            {
                _errors.Add(new ParseError("Expected a string literal after syntax assignment, found token ", _tokens.Peek()));
                return null;
            }

            TerminateSingleLineStatement();

            var syntaxNode = new Node(NodeType.Syntax, syntax.Lexeme);
            syntaxNode.AddChild(proto3);

            ScoopComment(syntaxNode);
            DumpEndline();

            return syntaxNode;
        }

        private Node ParseImport()
        {
            var importTag = _tokens.Dequeue();

            var modifier = ParseImportModifier();

            var importValue = ParseStringLiteral();

            if (ReferenceEquals(importValue, null))
            {
                _errors.Add(new ParseError("Could not find import location for import at line starting with token ", importTag));
                return null;
            }

            TerminateSingleLineStatement();

            var importNode = new Node(NodeType.Import, importTag.Lexeme);
            importNode.AddChild(modifier);
            importNode.AddChild(importValue);

            ScoopComment(importNode);
            DumpEndline();

            return importNode;
        }

        private void TerminateSingleLineStatement()
        {
            var terminator = _tokens.Dequeue();
            if (!_parser.IsEmptyStatement(terminator.Lexeme))
            {
                _errors.Add(new ParseError("Expected terminating `;` after syntax declaration, found token ", terminator));
            }
        }

        private void ScoopComment(Node parent)
        {
            var trailing = _tokens.Peek();
            if (!_parser.IsInlineComment(trailing.Lexeme)) return;
            var commentNode = ParseInlineComment();
            parent.AddChild(commentNode);
        }

        private void DumpEndline()
        {
            if (!_tokens.Any()) return;
            var trailing = _tokens.Peek();
            if (trailing.Type.Equals(TokenType.EndLine)) _tokens.Dequeue(); // Dump the endline
        }

        private Node ParseImportModifier()
        {
            var mod = _tokens.Peek();
            if (!_parser.IsImportModifier(mod.Lexeme)) return null;
            _tokens.Dequeue();
            return new Node(NodeType.ImportModifier, mod.Lexeme);
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

        private Node ParseFullIdentifier()
        {
            var ident = _tokens.Peek();
            if (!_parser.IsFullIdentifier(ident.Lexeme)) return null;
            _tokens.Dequeue();
            return new Node(NodeType.Identifier, ident.Lexeme);
        }

        private Node ParsePackage()
        {
            var packageTag = _tokens.Dequeue();

            var packageName = ParseFullIdentifier();

            if (ReferenceEquals(packageName, null))
            {
                _errors.Add(new ParseError($"Could not find a package name for package starting at line {packageTag.Line} Found token ", _tokens.Peek()));
                return null;
            }

            TerminateSingleLineStatement();

            var packageNode = new Node(NodeType.Package, packageTag.Lexeme);
            packageNode.AddChild(packageName);

            ScoopComment(packageNode);
            DumpEndline();

            return packageNode;
        }

        private Node ParseOption()
        {
            var optionTag = _tokens.Dequeue();

            var optionName = ParseFullIdentifier();
            if (ReferenceEquals(optionName, null))
            {
                _errors.Add(
                    new ParseError(
                        $"Could not find an option name for option starting at line {optionTag.Line} Found token ",
                        _tokens.Peek()));
            }

            var assignment = _tokens.Dequeue();
            if (!_parser.IsAssignment(assignment.Lexeme))
            {
                _errors.Add(new ParseError($"Expected an assignment after option name token on line {optionTag.Line}, found ", assignment));
                return null;
            }

            var optionValue = ParseStringLiteral();
            if (ReferenceEquals(optionValue, null))
            {
                _errors.Add(
                    new ParseError(
                        $"Could not find an option value for option starting at line {optionTag.Line} Found token ", 
                        _tokens.Peek()));
                return null;
            }

            TerminateSingleLineStatement();

            var optionNode = new Node(NodeType.Option, optionTag.Lexeme);
            optionNode.AddChild(optionName);
            optionNode.AddChild(optionValue);

            ScoopComment(optionNode);
            DumpEndline();

            return optionNode;
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
            var msgTag = _tokens.Dequeue();

            var msgNode = new Node(NodeType.Message, msgTag.Lexeme);

            var msgName = ParseIdentifier();
            if (ReferenceEquals(msgName, null))
            {
                _errors.Add(
                    new ParseError(
                        $"Could not find a message name on line {msgTag.Line} for message token ",
                        msgTag));
                return null;
            }

            msgNode.AddChild(msgName);

            var openBrack = _tokens.Dequeue();
            if (!openBrack.Type.Equals(TokenType.Control) || !openBrack.Lexeme[0].Equals('{'))
            {
                _errors.Add(
                    new ParseError(
                        $"Expected to find open bracket on line {msgTag.Line} for message token ",
                        msgTag));
                return null;
            }

            ScoopComment(msgNode);
            DumpEndline();

            var fieldNode = ParseMessageField();
            msgNode.AddChild(fieldNode);

            var closedBrack = _tokens.Dequeue();
            if (!closedBrack.Type.Equals(TokenType.Control) || !closedBrack.Lexeme[0].Equals('}'))
            {
                _errors.Add(
                    new ParseError(
                        "Expected to find close bracket for message token ",
                        msgTag));
                return null;
            }

            return msgNode;

        }

        private Node ParseRepeated()
        {
            var repeated = _tokens.Peek();
            if (!_parser.IsRepeated(repeated.Lexeme)) return null;

            _tokens.Dequeue();
            return new Node(NodeType.Repeated, repeated.Lexeme);
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

            TerminateSingleLineStatement();
            ScoopComment(fieldNode);
            DumpEndline();

            return fieldNode;
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
