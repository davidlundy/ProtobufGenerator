using ProtobufCompiler.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProtobufCompiler.Compiler.Types
{
    public class EnumField : IEquatable<EnumField>
    {
        public string Name { get; }
        public int FieldNumber { get; }
        public IEnumerable<Option> FieldOptions { get; }

        internal EnumField(string name, int fieldNum, IEnumerable<Option> option)
        {
            Name = Check.NotNull(name, nameof(name));
            FieldNumber = fieldNum;
            FieldOptions = option ?? new List<Option>();
        }

        public bool Equals(EnumField other)
        {
            if (other == null) return false;
            return Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase) &&
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