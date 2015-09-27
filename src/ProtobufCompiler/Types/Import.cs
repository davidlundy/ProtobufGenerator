using System;

namespace ProtobufCompiler.Types
{
    internal enum ImportType
    {
        None, 
        Weak,
        Public
    }

    internal class Import
    {
        internal ImportType ImportType { get; }
        internal string ImportClass { get; }

        internal Import(string type, string clas)
        {
            ImportType = (ImportType)Enum.Parse(ImportType.GetType(), type, true);
            ImportClass = clas;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Import;
            if (other == null) return false;

            return (ImportType.Equals(other.ImportType) && ImportClass.Equals(other.ImportClass, StringComparison.InvariantCultureIgnoreCase));
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 7) + ImportType.GetHashCode();
            hash = (hash * 7) + ImportClass.GetHashCode();
            return hash;
        }
    }
}
