using System;
using System.Collections.Generic;
using System.Linq;

namespace ProtobufCompiler.Types
{
    internal class Field : IEquatable<Field>
    {
        internal string FieldName { get; }
        internal SimpleType SimpleType { get; }
        internal string UserType { get; }
        internal int FieldNumber { get; }
        internal IEnumerable<Option> FieldOptions { get; }
        internal bool Repeated { get; }
        internal bool IsUsertype { get; }

        internal Field(string type, string name, int fieldNum, IEnumerable<Option> fieldOptions, bool isRepeated)
        {
            FieldName = name;
            FieldNumber = fieldNum;
            FieldOptions = fieldOptions;
            Repeated = isRepeated;
            IsUsertype = !Enum.IsDefined(typeof (SimpleType), type);
            SimpleType = IsUsertype ? SimpleType.None : (SimpleType)Enum.Parse(typeof(SimpleType), type);
            UserType = IsUsertype ? type : string.Empty;
        }

        public bool Equals(Field other)
        {
            if (other == null) return false;
            return FieldName.Equals(other.FieldName) &&
                   SimpleType.Equals(other.SimpleType) &&
                   FieldNumber.Equals(other.FieldNumber) &&
                   Repeated.Equals(other.Repeated) &&
                   UserType.Equals(other.UserType) &&
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

    internal enum SimpleType
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
