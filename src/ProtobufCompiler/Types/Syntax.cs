using System;

namespace ProtobufCompiler.Types
{
    public enum SyntaxType
    {
        Proto2,
        Proto3
    }

    public class Syntax : IEquatable<Syntax>
    {
        public SyntaxType Name { get; }

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
