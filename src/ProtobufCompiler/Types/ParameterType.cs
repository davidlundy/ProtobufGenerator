using System;
#if DNXCORE50
using System.Globalization;
#endif

namespace ProtobufCompiler.Types
{
    public class ParameterType : IEquatable<ParameterType>
    {
        public string Name { get; }
        public bool Streaming { get; }
#if DNXCORE50
        internal StringComparer InvCultIc = CultureInfo.InvariantCulture.CompareInfo.GetStringComparer(CompareOptions.None);
#else
        internal StringComparer InvCultIc = StringComparer.InvariantCultureIgnoreCase;
#endif

        internal ParameterType(string name, bool streaming)
        {
            Name = name;
            Streaming = streaming;
        }

        public bool Equals(ParameterType other)
        {
            if (other == null) return false;
            return InvCultIc.Equals(Name, other.Name) &&
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
