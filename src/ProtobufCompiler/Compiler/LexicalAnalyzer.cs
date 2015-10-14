using System.Collections.Generic;
using ProtobufCompiler.Interfaces;
using System.Linq;

namespace ProtobufCompiler.Compiler
{
    internal class LexicalAnalyzer : ILexicalAnalyzer
    {
        public Queue<Token> TokenStream => new Queue<Token>(_tokens);
        private readonly IList<Token> _tokens;
        private readonly IList<char> _buffer;
        private readonly ISource _source;

        private int _tokenLineStart;
        private int _tokenColumnStart;

        internal LexicalAnalyzer(ISource source)
        {
            _source = source;
            _tokens = new List<Token>();
            _buffer = new List<char>();
            _tokenLineStart = 1;
            _tokenColumnStart = 1;
        }

        internal void FlushBuffer()
        {
            if (!_buffer.Any()) return;
            var lexeme = new string(_buffer.ToArray());
            var type = ProtoGrammar.GetType(lexeme);
            _tokens.Add(new Token(type, _tokenLineStart, _tokenColumnStart, lexeme));
            _buffer.Clear();
        }

        public void Tokenize()
        {
            while (!_source.EndStream)
            {
                var character = _source.Next();

                if (!_buffer.Any())
                {
                    _tokenColumnStart = _source.Column;
                    _tokenLineStart = _source.Line;
                }


                if (char.IsWhiteSpace(character) || char.IsControl(character))
                {
                    FlushBuffer();
                    if (ProtoGrammar.LineFeed.Equals(character) || ProtoGrammar.CarriageReturn.Equals(character))
                    {
                        _tokens.Add(new Token(TokenType.EndLine, _source.Line, _source.Column, "EOL"));
                    }
                    continue;
                }

                if (ProtoGrammar.InlineTokens.Contains(character))
                {
                    FlushBuffer();
                    _tokens.Add(new Token(TokenType.Control, _source.Line, _source.Column, character.ToString()));
                    continue;
                }

                // Catches // or /* opening comment
                if (ProtoGrammar.Comment[0].Equals(character))
                {
                    var next = _source.Peek();
                    if (ProtoGrammar.Comment.Contains(next)) // Should handle opening single line or block comment even against preceding text.
                    {
                        FlushBuffer();
                        _tokens.Add(new Token(TokenType.Comment, _source.Line, _source.Column, string.Concat(new [] {character, next})));
                        _source.Next(); // Remove what we peeked. 
                        continue;
                    }
                }

                // Catches */ end comment. 
                if (ProtoGrammar.Comment[1].Equals(character))
                {
                    var next = _source.Peek();
                    if (ProtoGrammar.Comment[0].Equals(next))
                    {
                        FlushBuffer();
                        _tokens.Add(new Token(TokenType.Comment, _source.Line, _source.Column, string.Concat(new[] { character, next })));
                        _source.Next(); // Remove what we peeked.
                        continue;
                    }
                }

                _buffer.Add(character);
            }
        }

    }
}