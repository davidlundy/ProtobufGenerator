using System;
#if DNXCORE50
using System.Globalization;
#endif

namespace ProtobufCompiler.Compiler.Errors
{
    internal class SemanticError : CompilerError, IEquatable<SemanticError>
    {

#if DNXCORE50
        internal StringComparer InvCultIc = CultureInfo.InvariantCulture.CompareInfo.GetStringComparer(CompareOptions.None);
#else
        internal StringComparer InvCultIc = StringComparer.InvariantCultureIgnoreCase;
#endif

        internal SemanticError(string message) : base(message)
        {

        }

        public override string ToString()
        {
            return Message;
        }

        public bool Equals(SemanticError other)
        {
            if (other == null) return false;
            return InvCultIc.Equals(Message, other.Message);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals(obj as SemanticError);
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 7) + Message.GetHashCode();
            return hash;
        }

    }
}
