using System;
using System.Collections.Generic;
using System.Linq;

namespace ProtobufCompiler.Compiler
{
    internal enum StatementType
    {
        Line, 
        Block,
        Comment
    }

    internal class Statement : IEquatable<Statement>
    {
        internal StatementType StatementType { get; }
        internal IEnumerable<Token> TokenList { get; }

        public Statement(StatementType type, IEnumerable<Token> list)
        {
            StatementType = type;
            TokenList = list ?? new List<Token>();
        }
        public bool Equals(Statement other)
        {
            if (other == null) return false;
            return StatementType.Equals(other.StatementType) && TokenList.SequenceEqual(other.TokenList);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals(obj as Statement);
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash*7) + StatementType.GetHashCode();
            hash = (hash*7) + TokenList.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return $"{StatementType} statement : {string.Join(" ", TokenList.Select(t => t.Lexeme))}";
        }
    }
}
