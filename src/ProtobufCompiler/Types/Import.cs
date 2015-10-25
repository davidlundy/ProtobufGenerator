using System;
#if DNXCORE50
using System.Globalization;
#endif
namespace ProtobufCompiler.Types
{
    public enum ImportType
    {
        None, 
        Weak,
        Public
    }

    public class Import : IEquatable<Import>
    {
        public ImportType ImportType { get; }
        public string ImportClass { get; }

#if DNXCORE50
        internal StringComparer InvCultIc = CultureInfo.InvariantCulture.CompareInfo.GetStringComparer(CompareOptions.None);
#else
        internal StringComparer InvCultIc = StringComparer.InvariantCultureIgnoreCase;
#endif
        internal Import(string type, string clas)
        {
            ImportType = string.IsNullOrWhiteSpace(type) ? 
                ImportType = ImportType.None : 
                (ImportType)Enum.Parse(ImportType.GetType(), type, true);
            ImportClass = clas;
        }

        public bool Equals(Import other)
        {
            if (other == null) return false;
            return ImportType.Equals(other.ImportType) && InvCultIc.Equals(ImportClass, other.ImportClass);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals(obj as Import);
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
