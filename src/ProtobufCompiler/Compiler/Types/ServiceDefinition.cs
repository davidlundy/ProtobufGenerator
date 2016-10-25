using ProtobufCompiler.Extensions;
using ProtobufCompiler.Polyfill;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProtobufCompiler.Compiler.Types
{
    [ExcludeFromCodeCoverage("Immutable data holder type.")]
    public class ServiceDefinition : IEquatable<ServiceDefinition>
    {
        public string Name { get; }
        public IEnumerable<ServiceMethod> Methods { get; }
        public IEnumerable<Option> Options { get; }

        internal ServiceDefinition(string name, IEnumerable<ServiceMethod> serviceMethods, IEnumerable<Option> serviceOptions)
        {
            Name = Check.NotNull(name, nameof(name));
            Methods = serviceMethods ?? new List<ServiceMethod>();
            Options = serviceOptions ?? new List<Option>();
        }

        public bool Equals(ServiceDefinition other)
        {
            if (other == null) return false;
            return Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase) &&
                   Methods.SequenceEqual(other.Methods);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals(obj as ServiceDefinition);
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 7) + Name.GetHashCode();
            hash = (hash * 7) + Methods.GetHashCode();
            return hash;
        }
    }
}