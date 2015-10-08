using System;

namespace ProtobufCompiler.Types
{
    internal enum SyntaxType
    {
        Proto2,
        Proto3
    }

    internal class Syntax : IEquatable<Syntax>
    {
        internal SyntaxType Name { get; }

        internal Syntax(string name)
        {
            Name = (SyntaxType)Enum.Parse(typeof(SyntaxType), name, true);
        }

        public bool Equals(Syntax other)
        {
            return other != null && Name.Equals(other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals(obj as Syntax);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
