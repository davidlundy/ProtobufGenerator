using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProtobufCompiler.Types
{
    internal class EnumDefinition
    {
        internal string Name { get; }
        internal Option EnumOption { get; }
        internal IEnumerable<EnumField> EnumFields { get; }

        internal EnumDefinition(string name, Option option, IEnumerable<EnumField> fields)
        {
            Name = name;
            EnumOption = option;
            EnumFields = fields;
        }

        public bool Equals(EnumDefinition other)
        {
            if (other == null) return false;
            return Name.Equals(other.Name) &&
                   EnumOption.Equals(other.EnumOption) &&
                   EnumFields.SequenceEqual(other.EnumFields);

        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals(obj as EnumDefinition);
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 7) + Name.GetHashCode();
            hash = (hash * 7) + EnumOption.GetHashCode();
            hash = (hash * 7) + EnumFields.GetHashCode();
            return hash;
        }
    }
}
