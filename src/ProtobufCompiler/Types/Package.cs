using System;
#if DNXCORE50
using System.Globalization;
#endif

namespace ProtobufCompiler.Types
{
    internal class Package : IEquatable<Package>
    {
        internal string Name { get; }

#if DNXCORE50
        internal StringComparer InvCultIc = CultureInfo.InvariantCulture.CompareInfo.GetStringComparer(CompareOptions.None);
#else
        internal StringComparer InvCultIc = StringComparer.InvariantCultureIgnoreCase;
#endif

        internal Package(string name)
        {
            Name = name;
        }

        public bool Equals(Package other)
        {
            return other != null && InvCultIc.Equals(Name, other.Name);
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
