using System;
using ProtobufCompiler.Extensions;

namespace ProtobufCompiler.Types
{
    public class ParameterType : IEquatable<ParameterType>
    {
        public string Name { get; }
        public bool Streaming { get; }

        internal ParameterType(string name, bool streaming)
        {
            Name = name;
            Streaming = streaming;
        }

        public bool Equals(ParameterType other)
        {
            if (other == null) return false;
            return Name.EqualsIgnoreCase(other.Name) &&
                   Streaming.Equals(other.Streaming);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals(obj as ParameterType);
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 7) + Name.GetHashCode();
            hash = (hash * 7) + Streaming.GetHashCode();
            return hash;
        }
    }
}
