using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProtobufCompiler.Types
{
    internal class EnumField : IEquatable<EnumField>
    {
        internal string Name { get; }
        internal int FieldNumber { get; }
        internal Option FieldOption { get; }

        internal EnumField(string name, int fieldNum, Option option)
        {
            if(name == null) throw new ArgumentNullException(nameof(name));
            Name = name;
            FieldNumber = fieldNum;
            FieldOption = option;
        }

        public bool Equals(EnumField other)
        {
            if (other == null) return false;
            if (!Name.Equals(other.Name)) return false;
            if (!FieldNumber.Equals(other.FieldNumber)) return false;
            if (FieldOption == null)
            {
                if (other.FieldOption != null) return false;
            }
            else
            {
                return FieldOption.Equals(other.FieldOption);
            }
            return true;
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
            if(FieldOption != null) hash = (hash * 7) + FieldOption.GetHashCode();
            return hash;
        }
    }
}
