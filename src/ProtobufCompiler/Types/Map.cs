using System;
using ProtobufCompiler.Extensions;

namespace ProtobufCompiler.Types
{
    public class Map : IEquatable<Map>
    {
        public string Name { get; }
        public int FieldNum { get; }
        public string KeyType { get; }
        public string ValueType { get; }

        internal Map(string name, int fieldNum, string keyType, string valueType)
        {
            Name = name;
            FieldNum = fieldNum;
            KeyType = keyType;
            ValueType = valueType;
        }

        public bool Equals(Map other)
        {
            if (other == null) return false;
            return Name.EqualsIgnoreCase(other.Name) &&
                   FieldNum.Equals(other.FieldNum) &&
                   KeyType.EqualsIgnoreCase(other.KeyType) &&
                   ValueType.EqualsIgnoreCase(other.ValueType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals(obj as Map);
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash*7) + Name.GetHashCode();
            hash = (hash*7) + FieldNum.GetHashCode();
            hash = (hash*7) + KeyType.GetHashCode();
            hash = (hash*7) + ValueType.GetHashCode();
            return hash;
        }
    }
}
