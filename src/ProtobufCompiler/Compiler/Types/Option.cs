using System;
using ProtobufCompiler.Extensions;

namespace ProtobufCompiler.Compiler.Types
{
    public class Option : IEquatable<Option>
    {
        public string Name { get; }
        public string Value { get; }

        internal Option(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public bool Equals(Option other)
        {
            if (other == null) return false;

            return Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase) &&
                   Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals(obj as Option);
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 7) + Name.GetHashCode();
            hash = (hash * 7) + Value.GetHashCode();
            return hash;
        }
    }
}
