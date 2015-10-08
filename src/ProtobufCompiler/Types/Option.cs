using System;
#if DNXCORE50
using System.Globalization;
#endif

namespace ProtobufCompiler.Types
{
    internal class Option : IEquatable<Option>
    {
        internal string Name { get; }
        internal string Value { get; }

#if DNXCORE50
        internal StringComparer InvCultIc = CultureInfo.InvariantCulture.CompareInfo.GetStringComparer(CompareOptions.None);
#else
        internal StringComparer InvCultIc = StringComparer.InvariantCultureIgnoreCase;
#endif

        internal Option(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public bool Equals(Option other)
        {
            if (other == null) return false;

            return InvCultIc.Equals(Name, other.Name) &&
                   InvCultIc.Equals(Value, other.Value);
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
