using System;
using System.Collections.Generic;
using System.Linq;
using ProtobufCompiler.Extensions;

namespace ProtobufCompiler.Types
{
    public class MessageDefinition : IEquatable<MessageDefinition>
    {
        public string Name { get; }
        public ICollection<Field> Fields { get; }
        public ICollection<OneOf> OneOfs { get; }
        public ICollection<Option> Options { get; }
        public ICollection<Map> Maps { get; }
        public ICollection<EnumDefinition> Enumerations { get; }
        public ICollection<MessageDefinition> Messages { get; }

        internal MessageDefinition(string name, ICollection<Field> fields, ICollection<OneOf> oneOfs, ICollection<Option> options, ICollection<Map> maps, ICollection<EnumDefinition> enumerations, ICollection<MessageDefinition> messages)
        {
            Name = name;
            Fields = fields;
            OneOfs = oneOfs;
            Options = options;
            Maps = maps;
            Enumerations = enumerations;
            Messages = messages;
        }

        public bool Equals(MessageDefinition other)
        {
            if (other == null) return false;
            return Name.EqualsIgnoreCase(other.Name) &&
                   Fields.SequenceEqual(other.Fields) &&
                   OneOfs.SequenceEqual(other.OneOfs) &&
                   Options.SequenceEqual(other.Options) &&
                   Maps.SequenceEqual(other.Maps) &&
                   Enumerations.SequenceEqual(other.Enumerations) &&
                   Messages.SequenceEqual(other.Messages);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals(obj as MessageDefinition);
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 7) + Name.GetHashCode();
            hash = (hash * 7) + Fields.GetHashCode();
            hash = (hash * 7) + OneOfs.GetHashCode();
            hash = (hash * 7) + Options.GetHashCode();
            hash = (hash * 7) + Maps.GetHashCode();
            hash = (hash * 7) + Enumerations.GetHashCode();
            hash = (hash * 7) + Messages.GetHashCode();
            return hash;
        }
    }
}
