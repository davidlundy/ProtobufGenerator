using System;
using ProtobufCompiler.Extensions;

namespace ProtobufCompiler.Types
{
    public class Package : IEquatable<Package>
    {
        public string Name { get; }

        internal Package(string name)
        {
            Name = name;
        }

        public bool Equals(Package other)
        {
            return other != null && Name.EqualsIgnoreCase(other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals(obj as Package);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
