using System.Collections.Generic;

namespace ProtobufGenerator.Types
{
    public class FileDescriptor
    {
        public string FileName { get; set; }
        public Syntax Syntax { get; set; }
        public Package Package { get; set; }
        public ICollection<Import> Imports { get; set; } = new List<Import>();
        public ICollection<Option> Options { get; set; } = new List<Option>();

        public ICollection<MessageDefinition> Messages { get; set; } = new List<MessageDefinition>();
        public ICollection<EnumDefinition> Enumerations { get; set; } = new List<EnumDefinition>();
        public ICollection<ServiceDefinition> Services { get; set; } = new List<ServiceDefinition>();


    }
}
