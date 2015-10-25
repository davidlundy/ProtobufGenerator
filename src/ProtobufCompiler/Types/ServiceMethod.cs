using System;
#if DNXCORE50
using System.Globalization;
#endif
using ProtobufCompiler.Extensions;

namespace ProtobufCompiler.Types
{
    internal class ServiceMethod : IEquatable<ServiceMethod>
    {
        internal string Name { get; }
        internal ParameterType InputType { get; }
        internal ParameterType OutputType { get; }
#if DNXCORE50
        internal StringComparer InvCultIc = CultureInfo.InvariantCulture.CompareInfo.GetStringComparer(CompareOptions.None);
#else
        internal StringComparer InvCultIc = StringComparer.InvariantCultureIgnoreCase;
#endif

        internal ServiceMethod(string name, ParameterType inputType, ParameterType outputType)
        {
            Name = Check.NotNull(name, nameof(name));
            InputType = Check.NotNull(inputType, nameof(inputType));
            OutputType = Check.NotNull(outputType, nameof(outputType));
        }

        public bool Equals(ServiceMethod other)
        {
            if (other == null) return false;
            return InvCultIc.Equals(Name, other.Name) &&
                   InputType.Equals(other.InputType) &&
                   OutputType.Equals(other.OutputType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals(obj as ServiceMethod);
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 7) + Name.GetHashCode();
            hash = (hash * 7) + InputType.GetHashCode();
            hash = (hash * 7) + OutputType.GetHashCode();
            return hash;
        }
    }
}
