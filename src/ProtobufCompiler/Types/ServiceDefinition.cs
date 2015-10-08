using System;
using System.Collections.Generic;
using System.Linq;
#if DNXCORE50
using System.Globalization;
#endif

namespace ProtobufCompiler.Types
{
    internal class ServiceDefinition : IEquatable<ServiceDefinition>
    {
        internal string Name { get; }
        internal IEnumerable<ServiceMethod> Methods { get; }
        internal IEnumerable<Option> Options { get; }
#if DNXCORE50
        internal StringComparer InvCultIc = CultureInfo.InvariantCulture.CompareInfo.GetStringComparer(CompareOptions.None);
#else
        internal StringComparer InvCultIc = StringComparer.InvariantCultureIgnoreCase;
#endif

        internal ServiceDefinition(string name, IEnumerable<ServiceMethod> serviceMethods, IEnumerable<Option> serviceOptions )
        {
            if(name == null) throw new ArgumentNullException(nameof(name));
            Name = name;
            Methods = serviceMethods ?? new List<ServiceMethod>();
            Options = serviceOptions ?? new List<Option>();
        }

        public bool Equals(ServiceDefinition other)
        {
            if (other == null) return false;
            return InvCultIc.Equals(Name, other.Name) &&
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
