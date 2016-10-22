using ProtobufGenerator.Configuration;
using ProtobufGenerator.Interfaces;

namespace ProtobufGenerator.Tests
{
    public class MemoryConfiguration : IConfiguration
    {
        public JobSet JobSet { get; }

        public MemoryConfiguration(JobSet jobSet)
        {
            JobSet = jobSet;
        }
    }
}