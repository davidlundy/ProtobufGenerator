using System;
using System.Collections.Generic;
using System.Linq;

namespace ProtobufCompiler.Compiler.Types
{
    public class Field : IEquatable<Field>
    {
        public string FieldName { get; }
        public SimpleType SimpleType { get; }
        public string UserType { get; }
        public int FieldNumber { get; }
        public IEnumerable<Option> FieldOptions { get; }
        public bool Repeated { get; }
        public bool IsUsertype { get; }

        internal Field(string type, string name, int fieldNum, IEnumerable<Option> fieldOptions, bool isRepeated)
        {
            FieldName = name;
            FieldNumber = fieldNum;
            FieldOptions = fieldOptions ?? new List<Option>();
            Repeated = isRepeated;
            IsUsertype = !Enum.IsDefined(typeof(SimpleType), type);
            SimpleType = IsUsertype ? SimpleType.None : (SimpleType)Enum.Parse(typeof(SimpleType), type);
            UserType = IsUsertype ? type : string.Empty;
        }

        public bool Equals(Field other)
        {
            if (other == null) return false;
            return FieldName.Equals(other.FieldName, StringComparison.OrdinalIgnoreCase) &&
                   SimpleType.Equals(other.SimpleType) &&
                   FieldNumber.Equals(other.FieldNumber) &&
                   Repeated.Equals(other.Repeated) &&
                   UserType.Equals(other.UserType, StringComparison.OrdinalIgnoreCase) &&
                   FieldOptions.SequenceEqual(other.FieldOptions);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals(obj as Field);
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 7) + FieldName.GetHashCode();
            hash = (hash * 7) + SimpleType.GetHashCode();
            hash = (hash * 7) + UserType.GetHashCode();
            hash = (hash * 7) + FieldNumber.GetHashCode();
            hash = (hash * 7) + FieldOptions.GetHashCode();
            hash = (hash * 7) + Repeated.GetHashCode();
            return hash;
        }
    }

    public enum SimpleType
    {
        None,
        Double,
        Float,
        Int32,
        Int64,
        Uint32,
        Uint64,
        Sint32,
        Sint64,
        Fixed32,
        Fixed64,
        Sfixed32,
        Sfixed64,
        Bool,
        String,
        Bytes
    }
}