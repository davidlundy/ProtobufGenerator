using System;
using System.Collections.Generic;
using System.Linq;
#if DNXCORE50
using System.Globalization;
#endif

namespace ProtobufCompiler.Types
{
    public class OneOf : IEquatable<OneOf>
    {
        public string Name { get; }
        public IEnumerable<OneOfField> Fields { get; }
#if DNXCORE50
        internal StringComparer InvCultIc = CultureInfo.InvariantCulture.CompareInfo.GetStringComparer(CompareOptions.None);
#else
        internal StringComparer InvCultIc = StringComparer.InvariantCultureIgnoreCase;
#endif

        internal OneOf(string name, IEnumerable<OneOfField> fields)
        {
            Name = name;
            Fields = fields;
        }

        public bool Equals(OneOf other)
        {
            if (other == null) return false;
            return InvCultIc.Equals(Name, other.Name) &&
                   Fields.SequenceEqual(other.Fields);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals(obj as OneOf);
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash*7) + Name.GetHashCode();
            hash = (hash*7) + Fields.GetHashCode();
            return hash;
        }
    }
}
