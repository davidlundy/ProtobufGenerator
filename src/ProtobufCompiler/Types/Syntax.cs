using System;

namespace ProtobufCompiler.Types
{
    internal enum SyntaxType
    {
        Proto2,
        Proto3
    }

    internal class Syntax
    {
        internal SyntaxType Name { get; }

        internal Syntax(string name)
        {
            Name = (SyntaxType)Enum.Parse(typeof(SyntaxType), name, true);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Syntax;
            return other != null && Name.Equals(other.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
