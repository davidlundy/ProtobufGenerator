using ProtobufCompiler.Polyfill;
using System;

namespace ProtobufCompiler.Compiler.Types
{
    [ExcludeFromCodeCoverage("Immutable data holder type.")]
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
            return Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase) &&
                   FieldNum.Equals(other.FieldNum) &&
                   KeyType.Equals(other.KeyType, StringComparison.OrdinalIgnoreCase) &&
                   ValueType.Equals(other.ValueType, StringComparison.OrdinalIgnoreCase);
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
            hash = (hash * 7) + Name.GetHashCode();
            hash = (hash * 7) + FieldNum.GetHashCode();
            hash = (hash * 7) + KeyType.GetHashCode();
            hash = (hash * 7) + ValueType.GetHashCode();
            return hash;
        }
    }
}