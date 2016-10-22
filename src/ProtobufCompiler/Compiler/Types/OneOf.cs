using System;
using System.Collections.Generic;
using System.Linq;

namespace ProtobufCompiler.Compiler.Types
{
    public class OneOf : IEquatable<OneOf>
    {
        public string Name { get; }
        public IEnumerable<OneOfField> Fields { get; }

        internal OneOf(string name, IEnumerable<OneOfField> fields)
        {
            Name = name;
            Fields = fields;
        }

        public bool Equals(OneOf other)
        {
            if (other == null) return false;
            return Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase) &&
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
            hash = (hash * 7) + Name.GetHashCode();
            hash = (hash * 7) + Fields.GetHashCode();
            return hash;
        }
    }
}