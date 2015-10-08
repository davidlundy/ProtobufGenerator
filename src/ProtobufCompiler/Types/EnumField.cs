using System;
using System.Collections.Generic;
using System.Linq;
#if DNXCORE50
using System.Globalization;
#endif

namespace ProtobufCompiler.Types
{
    internal class EnumField : IEquatable<EnumField>
    {
        internal string Name { get; }
        internal int FieldNumber { get; }
        internal IEnumerable<Option> FieldOptions { get; }

#if DNXCORE50
        internal StringComparer InvCultIc = CultureInfo.InvariantCulture.CompareInfo.GetStringComparer(CompareOptions.None);
#else
        internal StringComparer InvCultIc = StringComparer.InvariantCultureIgnoreCase;
#endif

        internal EnumField(string name, int fieldNum, IEnumerable<Option> option)
        {
            if(name == null) throw new ArgumentNullException(nameof(name));
            Name = name;
            FieldNumber = fieldNum;
            FieldOptions = option ?? new List<Option>();
        }

        public bool Equals(EnumField other)
        {
            if (other == null) return false;
            return InvCultIc.Equals(Name, other.Name) &&
                   FieldNumber.Equals(other.FieldNumber) &&
                   FieldOptions.SequenceEqual(other.FieldOptions);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals(obj as EnumField);
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 7) + Name.GetHashCode();
            hash = (hash * 7) + FieldNumber.GetHashCode();
            hash = (hash * 7) + FieldOptions.GetHashCode();
            return hash;
        }
    }
}
