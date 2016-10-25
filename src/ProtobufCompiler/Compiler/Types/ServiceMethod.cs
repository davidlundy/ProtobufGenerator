using ProtobufCompiler.Extensions;
using ProtobufCompiler.Polyfill;
using System;

namespace ProtobufCompiler.Compiler.Types
{
    [ExcludeFromCodeCoverage("Immutable data holder type.")]
    public class ServiceMethod : IEquatable<ServiceMethod>
    {
        public string Name { get; }
        public ParameterType InputType { get; }
        public ParameterType OutputType { get; }

        internal ServiceMethod(string name, ParameterType inputType, ParameterType outputType)
        {
            Name = Check.NotNull(name, nameof(name));
            InputType = Check.NotNull(inputType, nameof(inputType));
            OutputType = Check.NotNull(outputType, nameof(outputType));
        }

        public bool Equals(ServiceMethod other)
        {
            if (other == null) return false;
            return Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase) &&
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