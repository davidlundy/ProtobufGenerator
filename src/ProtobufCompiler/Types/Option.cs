using System;

namespace ProtobufCompiler.Types
{
    internal class Option
    {
        internal string Name { get; }
        internal string Value { get; }

        internal Option(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Option;
            if (other == null) return false;

            return Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase) &&
                   Value.Equals(other.Value, StringComparison.InvariantCultureIgnoreCase);
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
