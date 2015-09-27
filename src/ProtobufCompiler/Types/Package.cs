using System;

namespace ProtobufCompiler.Types
{
    internal class Package
    {
        internal string Name { get; }

        internal Package(string name)
        {
            Name = name;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Package;
            return other != null && Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
