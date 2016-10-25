using ProtobufCompiler.Polyfill;
using System;

namespace ProtobufCompiler.Compiler.Types
{
    [ExcludeFromCodeCoverage("Immutable data holder type.")]
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
            return Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase) &&
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