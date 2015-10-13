using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProtobufCompiler.Interfaces;

namespace ProtobufCompiler.Compiler
{
    internal class SyntaxAnalyzer : ISyntaxAnalyzer
    {
        public Queue<Statement> Statements => new Queue<Statement>(_statements);
        private readonly Queue<Token> _tokens;
        private readonly IList<Token> _buffer;
        private readonly List<Statement> _statements;


        internal SyntaxAnalyzer(Queue<Token> tokens)
        {
            _tokens = tokens;
            _buffer = new List<Token>();
            _statements = new List<Statement>();
        }

        public void Analyze()
        {
            while (_tokens.Any())
            {
                var token = _tokens.Peek();
                if(!TokenType.Id.Equals(token.Type)) throw new ArgumentException("Top level statement invalid");
                if (ProtoGrammar.LineDefinitions.Contains(token.Lexeme))
                {
                    CreateLineStatement();
                }
                if (ProtoGrammar.BlockDefinitions.Contains(token.Lexeme))
                {
                    CreateBlockStatement();
                }
            }
        }

        private void CreateLineStatement()
        {
            Token token = null;
            do
            {
                token = _tokens.Dequeue();
                _buffer.Add(token);
            } while (!token.Type.Equals(TokenType.EndLine));

            if (_tokens.Any() && _tokens.Peek().Type.Equals(TokenType.EndLine))
                _tokens.Dequeue(); // Dump the trailing EOL
            
            _statements.Add(new Statement(StatementType.Line, new List<Token>(_buffer)));
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

            _statements.Add(new Statement(StatementType.Block, new List<Token>(_buffer)));
            _buffer.Clear();
        }
    }
}
