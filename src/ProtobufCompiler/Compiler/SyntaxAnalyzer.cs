using System;
using System.Collections.Generic;
using System.Linq;
using ProtobufCompiler.Interfaces;

namespace ProtobufCompiler.Compiler
{
    internal class SyntaxAnalyzer : ISyntaxAnalyzer
    {
        private readonly Queue<Token> _tokens;
        private readonly IList<Token> _buffer;

        public Queue<Statement> Statements { get; }
        public IList<ParseError> Errors { get; }

        internal SyntaxAnalyzer(Queue<Token> tokens)
        {
            _tokens = tokens;
            _buffer = new List<Token>();
            Statements = new Queue<Statement>();
            Errors = new List<ParseError>();
        }

        public void Analyze()
        {
            while (_tokens.Any())
            {
                var token = _tokens.Peek();
                if (ProtoGrammar.LineDefinitions.Contains(token.Lexeme))
                {
                    CreateLineStatement();
                }
                else if (ProtoGrammar.BlockDefinitions.Contains(token.Lexeme))
                {
                    CreateBlockStatement();
                }
                else if (ProtoGrammar.BlockComment.Contains(token.Lexeme))
                {
                    CreateBlockComment();
                }
                else
                {
                    Errors.Add(new ParseError(token, "Line starts with invalid token"));
                    var currentLine = token.Line;
                    int nextLine;
                    do
                    {
                        var next = _tokens.Dequeue();
                        nextLine = next.Line;
                    } while (currentLine == nextLine);
                }
            }
        }

        private void CreateLineStatement()
        {
            Token token;
            do
            {
                token = _tokens.Dequeue();
                _buffer.Add(token);
            } while (!token.Type.Equals(TokenType.EndLine));

            if (_tokens.Any() && _tokens.Peek().Type.Equals(TokenType.EndLine))
                _tokens.Dequeue(); // Dump the trailing EOL
            
            Statements.Enqueue(new Statement(StatementType.Line, new List<Token>(_buffer)));
            _buffer.Clear();
        }

        private void CreateBlockComment()
        {
            var foundClosing = false;
            do
            {
                var token = _tokens.Dequeue();
                _buffer.Add(token);
                if (token.Lexeme.Equals("*/")) foundClosing = true;
            } while (!foundClosing);

            if (_tokens.Any() && _tokens.Peek().Type.Equals(TokenType.EndLine))
                _tokens.Dequeue(); // Dump the trailing EOL

            Statements.Enqueue(new Statement(StatementType.Block, new List<Token>(_buffer)));
            _buffer.Clear();
        }

        private void CreateBlockStatement()
        {
            var firstTokenDone = false;
            var hasFoundOpenBlock = false;
            var openCount = 0;
            var closeCount = 0;
            var blockScopeCompleted = false;

            do
            {
                var token = _tokens.Dequeue();
                if(token.Type.Equals(TokenType.EndLine) && !hasFoundOpenBlock)
                    throw new Exception("Found EOL before open brace for block statement");

                if (token.Lexeme.Equals("{"))
                {
                    if (!hasFoundOpenBlock) hasFoundOpenBlock = true;
                    openCount++;
                }
                if (token.Lexeme.Equals("}"))
                {
                    closeCount++;
                }
                _buffer.Add(token);

                if (token.Lexeme.Equals("}"))
                {
                    blockScopeCompleted = hasFoundOpenBlock && openCount == closeCount;
                }

                if (!firstTokenDone)
                    firstTokenDone = true;

            } while (!blockScopeCompleted);

            if (_tokens.Any() && _tokens.Peek().Type.Equals(TokenType.EndLine))
                _tokens.Dequeue(); // Dump the trailing EOL

            Statements.Enqueue(new Statement(StatementType.Block, new List<Token>(_buffer)));
            _buffer.Clear();
        }
    }
}
